using System.ComponentModel.DataAnnotations;

namespace TobimSystem.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        public string Department { get; set; } // معماري، إنشائي، إلخ
        public string Round { get; set; }
        public string Status { get; set; } // Rest, Full Pay, Hold, Return
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => TotalPrice - PaidAmount;
        public string SalesName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
