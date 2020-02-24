using System.Collections.Generic;
using GigHub.Models;

namespace GigHub.Contracts
{
    public interface IGigsRepository
    {
        void AddAGig(Gig gig);
        IEnumerable<Gig> GetUpcomingGigs();
        //Gig FindGigById(int id);
        //bool DeleteAGig(Gig gig);
    }
}
