using GigHub.Models;

namespace GigHub.Services
{
    public interface IGigsRepository
    {
        void AddAGig(Gig gig);
        //IEnumerable<Gig> GetAllGigs();
        //Gig FindGigById(int id);
        //bool DeleteAGig(Gig gig);
    }
}
