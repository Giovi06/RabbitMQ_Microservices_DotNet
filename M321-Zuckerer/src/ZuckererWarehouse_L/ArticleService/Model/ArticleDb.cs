using Microsoft.EntityFrameworkCore;

namespace ArticleService.Model
{
    public class ArticleDb : DbContext
    {
        public ArticleDb(DbContextOptions<ArticleDb> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Article> Articles { get; set; }
    }
}
