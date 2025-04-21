using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace SmartShopAPI.Entities
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
