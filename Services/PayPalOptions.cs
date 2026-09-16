namespace TheBestBean.Services
{
    public class PayPalOptions
    {
        public string Mode { get; set; } = "Live";
        public string ClientId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;

        public bool IsSandbox => Mode.Equals("Sandbox", StringComparison.OrdinalIgnoreCase);
        public bool IsConfigured => !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(Secret);
        public string ApiBase => IsSandbox ? "https://api-m.sandbox.paypal.com" : "https://api-m.paypal.com";
    }
}
