using System.ComponentModel.DataAnnotations;

namespace MVCProject.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public String? ProductName { get; set; }
        public String? ImageUrl { get; set; }
    }
}
