using Microsoft.EntityFrameworkCore;

namespace OrderService.Model
{
    public class OrderDb(DbContextOptions<OrderDb> options) : DbContext(options)
    {
        public DbSet<Client> Clients { get; set; }

        public DbSet<Order> Orders { get; set; }
        
        public DbSet<OrderItem> OrderItems { get; set; }

        // save enum values as strings
        // see https://jonathancrozier.com/blog/a-better-way-to-convert-enums-to-strings-with-entity-framework-core
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<Enum>().HaveConversion<string>();
        }
    }
}
