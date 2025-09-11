using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{
    public class Payment
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string StatusCode { get; set; }
    }
}
