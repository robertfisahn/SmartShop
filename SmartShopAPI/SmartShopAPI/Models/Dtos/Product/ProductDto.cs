﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartShopAPI.Models.Dtos.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryId { get; set; }
        public string ImagePath { get; set; }
    }
}
