namespace TheBestBean.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Bean" or "Equipment"
        public decimal Price { get; set; }
        public decimal BasePriceUSD { get; set; } = 24.00m;
        public decimal BasePricePEN { get; set; } = 90.00m;
        public string Unit { get; set; } = "12oz";
        public decimal Rating { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FlavorProfile { get; set; } = string.Empty;
        public string ProcessingMethod { get; set; } = string.Empty;
        public string ProducerImageUrl { get; set; } = string.Empty;
        public string ProducerDescription { get; set; } = string.Empty;
        public string ProcessingDescription { get; set; } = string.Empty;
        public string OriginDescription { get; set; } = string.Empty;
        public string MapImageUrl { get; set; } = string.Empty;
        public string Variety { get; set; } = string.Empty;
        public string Altitude { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public decimal ScaScore { get; set; }
        public string GetStarRating()
        {
            var stars = "";
            var fullStars = (int)Math.Floor(Rating);
            var hasHalfStar = Rating % 1 >= 0.5m;

            for (int i = 0; i < fullStars; i++)
            {
                stars += "★";
            }

            if (hasHalfStar)
            {
                stars += "☆";
            }

            // Fill remaining stars as empty
            var emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);
            for (int i = 0; i < emptyStars; i++)
            {
                stars += "☆";
            }

            return stars;
        }
    }
}
