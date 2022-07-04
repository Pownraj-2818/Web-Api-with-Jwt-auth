using Microsoft.EntityFrameworkCore;
using System;

namespace JwtAuth.Models
{
    public class JWTAUTHContext:DbContext
    {
        public JWTAUTHContext(DbContextOptions<JWTAUTHContext> options):base(options)
        {

        }

        public DbSet<User>? Users{get;set;}
        public DbSet<Products>? Products {get;set;}
    }
}