using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Restaurent.Core.Domain.Entities
{
    [Index(nameof(CategoryName), IsUnique = true)]
    public class Category
    {
        [Key] 
        public Guid Id { get; set; }

        [StringLength(200)]
        public string CategoryName { get; set; }
        public bool Status { get; set; } 
        public string? Cat_Image { get; set; }

        public ICollection<Dish> Dishes { get; set; } = new List<Dish>(); 

    }
}
