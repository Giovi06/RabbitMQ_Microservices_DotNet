using JwtLibrary;
using Microsoft.EntityFrameworkCore;
using OrderService.Model;

namespace OrderService
{
    public class JwtClientLookup(OrderDb clientDb) : IJwtSubjectLookup<Client?>
    {
        private readonly OrderDb _db = clientDb;

        public Task<Client?> LookupAsync(string subject) => _db.Clients.Where(x => x.Email == subject).SingleOrDefaultAsync();
        
    }
}
