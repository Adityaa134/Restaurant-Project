using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.Domain.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; } 
        public DateTime RefershTokenExpirationDateTime { get; set; }
        [StringLength(1000)]
        public string? ProfileImagePath { get; set; }
        public ICollection<Carts> Carts { get; set; } = new List<Carts>();
    }
}
