using Microsoft.AspNetCore.Mvc;
using JwtAuth.Models;
namespace JwtAuth.Repositories
{
    public interface IProductRepository
    {
        public Task<Products> Create(Products products);
        public Task<IEnumerable<Products>> GetProducts();  

        public Task<Products> GetById(int id); 
        public Task update(Products product);       
    }
}