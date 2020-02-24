using System.Threading.Tasks;

namespace GigHub.Contracts
{
    public interface IUnitOfWork
    {
        IGenresRepository Genres { get; }
        IGigsRepository Gigs { get; }
        Task<int> AsyncComplete();
        int Complete();
    }
}
