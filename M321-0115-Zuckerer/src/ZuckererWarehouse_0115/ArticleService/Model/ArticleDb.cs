using Microsoft.EntityFrameworkCore;

namespace ArticleService.Model
{
    public class ArticleDb : DbContext
    {
        public ArticleDb(DbContextOptions<ArticleDb> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
    }
}
