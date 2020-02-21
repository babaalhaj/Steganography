using GigHub.Data;
using GigHub.Models;

namespace GigHub.Services
{
    public class GigsRepository : IGigsRepository
    {
        private readonly ApplicationDbContext _context;

        public GigsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddAGig(Gig gig)
        {
            _context.Gigs.Add(gig);
        }
    }
}
