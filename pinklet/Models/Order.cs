using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }
        public string OrderId { get; set; }
        public double TotalAmount { get; set; }
        public string Address { get; set; }
        public string RecipientName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string RecipientPhoneNumber { get; set; }
        public string Progress { get; set; }
        public DateTime OrderedDate { get; set; }
        public DateTime RequiredDate { get; set; }  
        public DateTime? DeliveredDate { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
        public string? DeliveryNote { get; set; }
        public string? OrderNote { get; set; }
        public string ClientEmailAddress { get; set; }
        public string ClientFullName { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}

