using System.Threading.Tasks;

namespace GigHub.Services
{
    public interface IUnitOfWork
    {
        IGenresRepository Genres { get; }
        Task<int> AsyncComplete();
        int Complete();
    }
}
