using System.ComponentModel.DataAnnotations;

namespace Payment.Domain
{
    public class BankResponse
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
