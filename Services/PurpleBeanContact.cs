namespace TheBestBean.Services
{
    public static class PurpleBeanContact
    {
        public const string WhatsAppDigits = "51913779574";
        public const string WhatsAppDisplay = "+51 913 779 574";
        public const string WhatsAppLocal = "913 779 574";
        public const string WhatsAppUrl = "https://wa.me/" + WhatsAppDigits;
        public const string TelE164 = "+51913779574";

        public static string WhatsAppResetUrl =>
            WhatsAppUrl + "?text=" + Uri.EscapeDataString("I need to reset the Purple Bean admin password.");
    }
}
