using System.ComponentModel.DataAnnotations;
namespace MVCProject.Models
{
public class Order
{
    [Key]
    public int OrderId { get; set; }

    // USER INFO
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    // PRODUCT INFO
    public string ProductName { get; set; } // optional
    public string ProductListJson { get; set; } // MAIN SYSTEM
   // optional

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalProductPrice { get; set; }

    // PAYMENT
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }

    // ORDER
    public string OrderNumber { get; set; }
    public decimal GrandTotal { get; set; }
    public string OrderStatus { get; set; }
    public DateTime OrderTime { get; set; } = DateTime.Now;
    public DateTime? DeliveryDate { get; set; }
}
}