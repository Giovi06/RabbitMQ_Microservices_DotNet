using Microsoft.EntityFrameworkCore;

namespace OrderService.Model
{
    [Index(nameof(Email), IsUnique = true)]
    public class Client
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
