using System.ComponentModel.DataAnnotations;
using static Enhanced_Order_Management_System.Helper.SD;

namespace Enhanced_Order_Management_System.Models.VMs
{
    public class UpdateOrderStatus
    {
        public Guid Id { get; set; }
        [MinLength(1)]
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; }
    }
}
