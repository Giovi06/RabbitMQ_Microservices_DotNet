namespace ArticleService.Model
{
    public class Category
    {
        public Guid Id { get; set; }

        public string Name{ get; set; }

        public string? Description { get; set; }

        public List<Article> Articles { get; set; }
    }
}