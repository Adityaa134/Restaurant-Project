using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restaurent.Core.Domain.Identity;

namespace Restaurent.Core.Domain.Entities
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public  string AddressLine { get; set; }
        public  string City { get; set; }
        public  string Area { get; set; }
        public  string? Landmark { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
