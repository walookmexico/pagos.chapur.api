using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PagosGranChapur.Entities.Request
{

    public class SavePaymentRequest {

        [Required]
        public string NoCreditCard { get; set; }
        [Required]
        public string NameCreditCard { get; set; }
        [Required]
        public string CVV { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int IdStore { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CreateDate { get; set; }
        [Required]
        public string IdPurchaseOrder { get; set; }
        [Required]
        public int PlatformId { get; set; }
        [Required]
        public string UserPlatform { get; set; }
        [Required]
        public string PasswordPlatform { get; set; }
        [Required]
        public int? Months { get; set; }
        [Required]
        public int? WithInterest { get; set; }

    }
    
    public class SaveOrderPaymentRequest: SavePaymentRequest{ }

    public class SaveDetailPaymentRequest: SavePaymentRequest
    {       
        public List<DetailOrden> Detail { get; set; }
    }

    public class DetailOrden {

        public int ItemId            { get; set; }
        public float PriceWithoutTax { get; set; }
        public float IVA             { get; set; }
        public float IEPS            { get; set; }
        public float AverageCost     { get; set; }
        public int Quantity          { get; set; }
        public float Discount        { get; set; }
    }
}
