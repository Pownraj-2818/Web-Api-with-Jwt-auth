using System.Security.Claims;
using System.Security.Cryptography;
using JwtAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using JwtAuth.Services;

namespace JwtAuth.Controller
{

    [ApiController]
    [Route("api/user/")]
    public class UserController : ControllerBase
    {
        private readonly JWTAUTHContext _context;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configure;
        public User _user = new();
        public UserController(JWTAUTHContext context, IConfiguration configure, IMailService mailService)
        {
            _context = context;
            _configure = configure;
            _mailService = mailService;
        }


        [AllowAnonymous]
        [HttpPost("register")]

        public async Task<ActionResult<User>> Register([FromBody] UserRegister request)
        {

            var exisistingUser = await _context.Users!.FirstOrDefaultAsync(username => request.username == username.username || request.email == username.email);

            if (exisistingUser != null)
            {
                return BadRequest("User Already Exists");
            }
            else
            {
                HashPassword(request.password, out byte[] passwordHash, out byte[] passwordSalt);

                _user.username = request.username;
                _user.passwordHash = passwordHash;
                _user.passwordSalt = passwordSalt;
                _user.email = request.email;
                _user.role = request.role;
                _user.verficationCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(6));
                var subject = "Verification Code";
                var body = $"<div><h1>Verification Code</h1><p>Dear {_user.username}</p><p>Please verify your account by using this verification code <b>{_user.verficationCode}</b></p></div>";
                await _mailService.SendMailAsync(_user, subject, body);
                _context.Users!.Add(_user);
                await _context.SaveChangesAsync();


                return Ok(_user);
            }


        }
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            var exisistingUser = _context.Users!.FirstOrDefault(username => user.username == username.username);

            if (exisistingUser == null)
            {
                return NotFound("Username not found");
            }
            else if (!verifyPassword(user.password, exisistingUser.passwordHash, exisistingUser.passwordSalt))
            {
                return BadRequest("Incorrect Password");
            }
            else if (exisistingUser.verifiedat == null)
            {
                return Ok("User not verified");
            }
            else
            {
                var userToken = CreateToken(exisistingUser);
                var refreshToken = GenerateRefreshToken();
                await SetRefreshToken(refreshToken);

                exisistingUser.RefreshToken = refreshToken.Token;
                exisistingUser.DateCreated = refreshToken.Created;
                exisistingUser.TokenExpires = refreshToken.Expires;

                updateUser(exisistingUser);
                return Ok(new
                {
                    message = "Success",
                    token = userToken
                });
            }
        }

        [NonAction]
        public void HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        [NonAction]
        public bool verifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {

            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }

        }

        [NonAction]
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name,user.username),
               new Claim("role",user.role)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configure.GetSection("JwtConfig:Key").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        private RefreshToken GenerateRefreshToken()
        {

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };


            return refreshToken;
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var existinguser = await _context.Users!.FirstOrDefaultAsync(user => user.RefreshToken == refreshToken);


            if (existinguser == null)
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (existinguser.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token Expired");
            }
            else
            {
                string token = CreateToken(existinguser);
                var newRefreshToken = GenerateRefreshToken();
                await SetRefreshToken(newRefreshToken);
                return Ok(token);
            }
        }


        private async Task SetRefreshToken(RefreshToken newRefreshToken)
        {

            var cookie = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookie);
        }
        private void updateUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> verify([FromForm] string verficationcode)
        {

            var user = await _context.Users!.FirstOrDefaultAsync(user => user.verficationCode == verficationcode);

            if (user == null)
            {
                return BadRequest("Invalid Verification code.Please Try again");
            }
            user.verifiedat = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok("User verified");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var exisistingUser = await _context.Users!.FirstOrDefaultAsync(user => user.email == email);
            if (exisistingUser == null)
            {
                return BadRequest("User not found");
            }
            exisistingUser.resetToken = CreateRandomToken();
            var body = $"<div><h1>Resest Your Password</h1><p>Hi {exisistingUser.username}</p><p>Your request to reset your password is approved.Use the following token {exisistingUser.resetToken}</p></div>";
            await _mailService.SendMailAsync(exisistingUser, "Reset Your Password", body);
            await _context.SaveChangesAsync();
            return Ok("Your request approved.Please check your mail");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword request)
        {
            var exisistingUser=await _context.Users!.FirstOrDefaultAsync(user=>user.resetToken==request.token);

            if(exisistingUser==null)
            {
                return BadRequest("Inavlid Token");
            }

            HashPassword(request.password,out byte[] passwordHash, out byte[] passwordSalt);
            exisistingUser.passwordHash=passwordHash;
            exisistingUser.passwordSalt=passwordSalt;
            await _context.SaveChangesAsync();
            return Ok("Password change successful");
        }
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }



}