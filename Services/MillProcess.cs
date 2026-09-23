namespace TheBestBean.Services
{
    /// <summary>
    /// Shared mill trail. SL09 washed is the template; other lots reuse it
    /// unless the process itself is different (natural, honey, SL28 72h).
    /// Dates stay on BeanInventory so they are not typed onto each shop card.
    /// </summary>
    public static class MillProcess
    {
        public const string WashedEn =
            "Fully washed: cherries are selectively picked, depulped, fermented 24–36 hours, washed clean, then dried on raised beds. Parchment is sorted before the lot moves to the Cusco warehouse.";

        public const string WashedEs =
            "Totalmente lavado: las cerezas se cosechan de forma selectiva, se despulpan, fermentan 24–36 horas, se lavan y se secan en camas africanas. El pergamino se clasifica antes de que el lote llegue al almacén en Cusco.";

        public const string Sl28En =
            "Ripe cherries are sorted, then held for a 72-hour fermentation before drying. The extended fermentation builds tropical fruit and berry intensity on top of SL28’s classic blackcurrant acidity. After the tank, the lot is washed and dried on raised beds.";

        public const string Sl28Es =
            "Las cerezas maduras se seleccionan y fermentan durante 72 horas antes del secado. La fermentación prolongada suma intensidad a fruta tropical y frutos rojos sobre la acidez clásica a grosella negra del SL28. Después del tanque, el lote se lava y se seca en camas africanas.";

        public const string NaturalEn =
            "Naturally processed: cherries dry with the fruit intact, then are hulled and sorted. The fruit stays on the seed longer, so sweetness and body read louder in the cup.";

        public const string NaturalEs =
            "Proceso natural: las cerezas se secan con la fruta intacta, luego se descascaran y se clasifican. La fruta permanece más tiempo sobre el grano, así que el dulzor y el cuerpo se leen más en taza.";

        public const string HoneyEn =
            "Honey process: cherries are pulped, then dried with mucilage still on the parchment. The mill keeps some fruit on the seed without a full natural.";

        public const string HoneyEs =
            "Proceso honey: las cerezas se despulpan y se secan con mucílago sobre el pergamino. El molino deja algo de fruta en el grano sin llegar a un natural completo.";

        public static string[] Steps(string? process, string? varietyOrName = null, string? type = null)
        {
            if (IsCacao(type))
            {
                return new[] { "Harvest", "Ferment", "Dry", "Sort" };
            }

            if (IsSl28(varietyOrName))
            {
                return new[] { "Selective pick", "Ferment 72h", "Wash", "Raised beds" };
            }

            var key = (process ?? "").ToLowerInvariant();
            if (key.Contains("natural"))
            {
                return new[] { "Selective pick", "Dry in cherry", "Hull", "Sort" };
            }

            if (key.Contains("honey"))
            {
                return new[] { "Selective pick", "Pulp", "Mucilage dry", "Mill" };
            }

            if (key.Contains("anaerobic"))
            {
                return new[] { "Selective pick", "Sealed ferment", "Wash", "Raised beds" };
            }

            return new[] { "Selective pick", "Ferment 24–36h", "Wash", "Raised beds" };
        }

        public static string Note(string? process, string? varietyOrName = null, string? type = null, string? stored = null)
        {
            if (!string.IsNullOrWhiteSpace(stored))
            {
                return stored.Trim();
            }

            if (IsCacao(type))
            {
                return "Carefully fermented and dried using traditional methods. Fermentation develops the cup; drying keeps the lot sound.";
            }

            return NoteFor(process, varietyOrName);
        }

        public static string NoteFor(string? process, string? varietyOrName = null)
        {
            if (IsSl28(varietyOrName))
            {
                return Sl28En;
            }

            var key = (process ?? "washed").ToLowerInvariant();
            if (key.Contains("natural"))
            {
                return NaturalEn;
            }

            if (key.Contains("honey"))
            {
                return HoneyEn;
            }

            return WashedEn;
        }

        public static string NoteForEs(string? process, string? varietyOrName = null)
        {
            if (IsSl28(varietyOrName))
            {
                return Sl28Es;
            }

            var key = (process ?? "washed").ToLowerInvariant();
            if (key.Contains("natural"))
            {
                return NaturalEs;
            }

            if (key.Contains("honey"))
            {
                return HoneyEs;
            }

            return WashedEs;
        }

        public static bool IsSl28(string? value) =>
            !string.IsNullOrWhiteSpace(value)
            && value.Contains("SL28", StringComparison.OrdinalIgnoreCase);

        private static bool IsCacao(string? type) =>
            string.Equals(type, "Cacao", StringComparison.OrdinalIgnoreCase);
    }
}
