using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using pinklet.data;
using pinklet.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IConfiguration configuration, ApplicationDbContext context, ILogger<PaymentController> logger)
        {
            _context = context;
            this.configuration = configuration;
            _logger = logger;
        }

        [HttpPost("generate-hash")]
        public IActionResult GenerateHash([FromBody] PaymentRequestDto dto)
        {
            _logger.LogInformation("Generate Hash");
            string merchantId = configuration["PayHere:MerchantId"];
            string merchantSecret = configuration["PayHere:MerchantSecret"];

            string amountFormatted = dto.Amount.ToString("0.00"); // ensure 2 decimals
            string currency = dto.Currency.ToUpper();

            // Inner hash of merchant secret
            using var md5 = MD5.Create();
            string secretMd5 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(merchantSecret)))
                                .Replace("-", "").ToUpper();

            string raw = merchantId + dto.OrderId + amountFormatted + currency + secretMd5;

            string hash = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(raw)))
                          .Replace("-", "").ToUpper();

            return Ok(new { hash });
        }

        //[HttpPost("notify")]
        //public async Task<IActionResult> Notify([FromForm] PayHereNotifyDto dto)
        //{
        //    string merchantSecret = configuration["PayHere:MerchantSecret"];

        //    // Verify checksum
        //    using var md5 = MD5.Create();
        //    string secretMd5 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(merchantSecret)))
        //                        .Replace("-", "").ToUpper();

        //    string raw = dto.merchant_id + dto.order_id + dto.payhere_amount + dto.payhere_currency + dto.status_code + secretMd5;
        //    string localMd5sig = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(raw)))
        //                          .Replace("-", "").ToUpper();
        //    _logger.LogTrace("payment id", dto.payment_id);
        //    if (localMd5sig == dto.md5sig && dto.status_code == "2")
        //    {
        //        _logger.LogTrace("payment id", dto.payment_id);
        //        var payment = new Payment
        //        {
        //            OrderId = dto.order_id,
        //            PaymentId = dto.payment_id,
        //            StatusCode = dto.status_code
        //        };

        //        _context.Payment.Add(payment);
        //        await _context.SaveChangesAsync();

        //        return Ok();
        //    }

        //    // ❌ Invalid or failed
        //    return BadRequest();
        //}

        [HttpPost("notify")]
        public async Task<IActionResult> Notify([FromForm] PayHereNotifyDto dto)
        {
            _logger.LogInformation("Notify called with OrderId={OrderId}, PaymentId={PaymentId}, Status={Status}, Md5sig={Md5}",
                dto.order_id, dto.payment_id, dto.status_code, dto.md5sig);

            string merchantSecret = configuration["PayHere:MerchantSecret"];

            using var md5 = MD5.Create();
            string secretMd5 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(merchantSecret)))
                                .Replace("-", "").ToUpper();

            string raw = dto.merchant_id + dto.order_id + dto.payhere_amount + dto.payhere_currency + dto.status_code + secretMd5;
            string localMd5sig = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(raw)))
                                  .Replace("-", "").ToUpper();

            _logger.LogInformation("Local MD5: {LocalMd5}, Incoming MD5: {IncomingMd5}", localMd5sig, dto.md5sig);
            Console.WriteLine("Before");
            if (localMd5sig == dto.md5sig && dto.status_code.Trim() == "2")
            {

                var payment = new Payment
                {
                    OrderId = dto.order_id,
                    PaymentId = dto.payment_id,
                    StatusCode = dto.status_code
                };

                _context.Payment.Add(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Payment saved. OrderId={OrderId}, PaymentId={PaymentId}", dto.order_id, dto.payment_id);

                return Ok();
            }

            _logger.LogWarning("❌ Payment notify failed. OrderId={OrderId}, PaymentId={PaymentId}", dto.order_id, dto.payment_id);
            return BadRequest();
        }


        public class PayHereNotifyDto
        {
            public string merchant_id { get; set; }
            public string order_id { get; set; }
            public string payment_id { get; set; }
            public string payhere_amount { get; set; }
            public string payhere_currency { get; set; }
            public string status_code { get; set; }
            public string md5sig { get; set; }
            public string method { get; set; }
            public string status_message { get; set; }
        }
        public class PaymentRequestDto
        {
            public string OrderId { get; set; }
            public double Amount { get; set; }
            public string Currency { get; set; }
        }
    }
}
