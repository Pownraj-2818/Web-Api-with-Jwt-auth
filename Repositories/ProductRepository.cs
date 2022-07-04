using System;
using JwtAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Repositories
{
    public class ProductRepository : IProductRepository
    {

        private readonly JWTAUTHContext _context;
        public ProductRepository(JWTAUTHContext context)
        {
            _context = context;
        }
        public async Task<Products> Create(Products products)
        {
             _context.Products!.Add(products);
            await _context.SaveChangesAsync();
            return products;  
        }

        public async Task<IEnumerable<Products>> GetProducts()
        {
           return await _context.Products!.ToListAsync();
        }

        public async Task<Products> GetById(int id)
        {
            var result = await _context.Products!.FindAsync(id);
            return result!;

        } 
        public async Task update(Products product)
        {
            _context.Entry(product).State= EntityState.Modified;
            await _context.SaveChangesAsync();
        }

       
    }
}