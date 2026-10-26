namespace MyShop.BLL.Models.Common
{
    public class PaymobSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public int IntegrationId { get; set; }
        public string HmacSecret { get; set; } = string.Empty;
        public string CallbackBaseUrl { get; set; } = string.Empty;
    }
}
