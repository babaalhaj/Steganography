using GigHub.Models;
using System.Collections.Generic;

namespace GigHub.Services
{
    public interface IGenresRepository
    {
        IEnumerable<Genre> GetGenres();
        Genre FindGenreById(byte id);
    }
}
