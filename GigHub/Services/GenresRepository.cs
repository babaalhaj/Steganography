using GigHub.Data;
using GigHub.Models;
using System.Collections.Generic;
using System.Linq;

namespace GigHub.Services
{
    public class GenresRepository : IGenresRepository
    {
        private readonly ApplicationDbContext _context;

        public GenresRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Genre> GetGenres()
        {
            return _context.Genres;
        }

        public Genre FindGenreById(byte id)
        {
            return _context.Genres.Single(g => g.Id == id);
        }
    }
}
