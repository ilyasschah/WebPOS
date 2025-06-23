using WebPOS.Data;

namespace WebPOS.Services
{
    public class BusinessHelper
    {
        private readonly AppDbContext _context;

        public BusinessHelper(AppDbContext context)
        {
            _context = context;
        }

        public int GetBusinessId()
        {
            // Always return the first business (since only one is used)
            return _context.Businesses.Select(b => b.BusinessId).FirstOrDefault();
        }
    }
}
