using GigHub.Data;
using System.Threading.Tasks;

namespace GigHub.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenresRepository _genres;
        private IGigsRepository _gigs;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenresRepository Genres => _genres ??= new GenresRepository(_context);
        public IGigsRepository Gigs => _gigs ??= new GigsRepository(_context);

        public async Task<int> AsyncComplete()
        {
            return await _context.SaveChangesAsync();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
