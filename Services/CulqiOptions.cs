namespace TheBestBean.Services
{
    public class CulqiOptions
    {
        public string PublicKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string RsaId { get; set; } = string.Empty;
        public string RsaPublicKey { get; set; } = string.Empty;

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(PublicKey) && !string.IsNullOrWhiteSpace(SecretKey);
    }
}
