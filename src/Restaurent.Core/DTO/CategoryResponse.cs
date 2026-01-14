using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to return the category details
    /// </summary>
    public class CategoryResponse
    {
        public Guid CategoryId { get; set; }

        public string Cat_Name { get; set; }

        public bool Status { get; set; }
        public string Cat_Image { get; set; }
    }

    public static class CategoryExtensions
    {
        public static CategoryResponse ToCategoryResponse(this Category category)
        {
            return new CategoryResponse()
            {
                CategoryId = category.Id,
                Cat_Name = category.CategoryName,
                Status = category.Status,
                Cat_Image = category.Cat_Image
            };
        }
    }
}
