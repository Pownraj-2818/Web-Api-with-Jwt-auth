using System;
using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Models
{
    public class Products
    {
        [Key]
        public int ProductId {get;set;}
        public string ProductName {get;set;} = string.Empty;
        public int Price {get;set;}
        public int Quantity {get;set;}

    }
}