namespace ArticleService.Model
{
    public class Article
    {
        public Guid Id { get; set; }

        public string ArticleNumber { get; set; }

        public string ShortDesc { get; set; }
        
        public string LongDesc { get; set; }

        public int QuantityInStock { get; set; }
        

        /// <summary>
        /// Price in 0.01 CHF
        /// </summary>
        public int Price { get; set; }
    }
}