using GigHub.Models;
using System.Collections.Generic;

namespace GigHub.Contracts
{
    public interface IGigsRepository
    {
        void AddAGig(Gig gig);
        IEnumerable<Gig> GetUpcomingGigs();
        IEnumerable<Gig> GetMyUpcomingGigs(string artistId);
        Gig FindGigById(int id);
        //bool DeleteAGig(Gig gig);
    }
}
