namespace TheBestBean.Services
{
    public class PayPalAccountOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string? BuyerEmail { get; set; }
    }

    public class PayPalOptions
    {
        public string Mode { get; set; } = "Live";
        public string ClientId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public PayPalAccountOptions? Sandbox { get; set; }
        public PayPalAccountOptions? Live { get; set; }

        public bool IsSandbox => Mode.Equals("Sandbox", StringComparison.OrdinalIgnoreCase);
        public string ResolvedClientId =>
            IsSandbox && !string.IsNullOrWhiteSpace(Sandbox?.ClientId) ? Sandbox.ClientId
            : !IsSandbox && !string.IsNullOrWhiteSpace(Live?.ClientId) ? Live.ClientId
            : ClientId;
        public string ResolvedSecret =>
            IsSandbox && !string.IsNullOrWhiteSpace(Sandbox?.Secret) ? Sandbox.Secret
            : !IsSandbox && !string.IsNullOrWhiteSpace(Live?.Secret) ? Live.Secret
            : Secret;
        public bool IsConfigured => !string.IsNullOrWhiteSpace(ResolvedClientId) && !string.IsNullOrWhiteSpace(ResolvedSecret);
        public string ApiBase => IsSandbox ? "https://api-m.sandbox.paypal.com" : "https://api-m.paypal.com";
    }
}
