using System.ComponentModel.DataAnnotations;

namespace MVCProject.Dtos
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int Id { get; set; }
    }
}
