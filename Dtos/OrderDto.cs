using System.ComponentModel.DataAnnotations;
namespace MVCProject.Dtos
{
    public class OrderDto
    {
        // ======================
        // USER INFO
        // ======================
        public string FullName { get; set; }
        public string Address { get; set; }

        // ======================
        // PRODUCT INFO
        // ======================
       public string ProductImage { get; set; }
        // ======================
        // PAYMENT INFO
        // ======================
        public string PaymentMethod { get; set; }

        // ======================
        // ORDER INFO
        // ======================
      public string PhoneNumber { get; set; }

    }
}