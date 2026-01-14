using System.ComponentModel.DataAnnotations;

namespace Restaurent.Core.DTO
{
    public class CategoryStatusUpdateRequest
    {
        [Required(ErrorMessage = "Category Id is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public bool Status { get; set; }
    }
}
