using System.Collections.Generic;
using GigHub.Models;

namespace GigHub.Contracts
{
    public interface IGenresRepository
    {
        IEnumerable<Genre> GetGenres();
        Genre FindGenreById(byte id);
    }
}
