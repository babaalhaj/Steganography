using GigHub.Contracts;
using GigHub.Data;

namespace GigHub.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenresRepository _genres;
        private IGigsRepository _gigs;
        private IAttendancesRepository _attendances;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenresRepository Genres => _genres ??= new GenresRepository(_context);
        public IGigsRepository Gigs => _gigs ??= new GigsRepository(_context);
        public IAttendancesRepository Attendances => _attendances ??= new AttendancesRepository(_context);

        //public async Task<int> AsyncComplete()
        //{
        //    return await _context.SaveChangesAsync();
        //}

        public int Complete()
        {
            return _context.SaveChanges();
        }
    }
}
