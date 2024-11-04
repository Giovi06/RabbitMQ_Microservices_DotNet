namespace ArticleService.Model
{
    public class Article
    {
        public Guid Id { get; set; }

        public string ArticleNumber { get; set; }

        public string ShortDesc { get; set; }

        public int QuantityInStock { get; set; }

        public Category? Category { get; set; }

        /// <summary>
        /// Price in 0.01 CHF
        /// </summary>
        public int Price { get; set; }

        public string? LongDesc { get; set; }

        /// <summary>
        /// Weight in g (= 0.001kg)
        /// </summary>
        public int? Weight { get; set; }

        /// <summary>
        /// Package width in mm
        /// </summary>
        public int? PackageDimensionW { get; set; }

        /// <summary>
        /// Package length in mm
        /// </summary>
        public int? PackageDimensionL { get; set; }

        /// <summary>
        /// Package height in mm
        /// </summary>
        public int? PackageDimensionH { get; set; }
    }
}