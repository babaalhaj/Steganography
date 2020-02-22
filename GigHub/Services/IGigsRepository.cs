using GigHub.Models;
using System.Collections.Generic;

namespace GigHub.Services
{
    public interface IGigsRepository
    {
        void AddAGig(Gig gig);
        IEnumerable<Gig> GetUpcomingGigs();
        //Gig FindGigById(int id);
        //bool DeleteAGig(Gig gig);
    }
}
