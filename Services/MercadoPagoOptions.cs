namespace TheBestBean.Services
{
    public class MercadoPagoOptions
    {
        public string AccessToken { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public bool IsConfigured => !string.IsNullOrWhiteSpace(AccessToken)
            && !AccessToken.Contains("paste-", StringComparison.OrdinalIgnoreCase);
        public bool IsSandbox => AccessToken.StartsWith("TEST-", StringComparison.OrdinalIgnoreCase);
    }
}
