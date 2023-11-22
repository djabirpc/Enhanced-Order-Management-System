using System.ComponentModel.DataAnnotations;
using static Enhanced_Order_Management_System.Helper.SD;

namespace Enhanced_Order_Management_System.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [MinLength(1)]
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; }

    }
}
