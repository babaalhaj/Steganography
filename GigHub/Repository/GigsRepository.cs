using GigHub.Contracts;
using GigHub.Data;
using GigHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GigHub.Repository
{
    public class GigsRepository : IGigsRepository
    {
        private readonly ApplicationDbContext _context;

        public GigsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddAGig(Gig gig)
        {
            _context.Gigs.Add(gig);
        }

        public IEnumerable<Gig> GetUpcomingGigs()
        {
            return _context.Gigs.Include(g => g.Artist).Include(g=>g.Genre)
                .Where(g => g.DateTime > DateTime.Now);
        }

        public IEnumerable<Gig> GetMyUpcomingGigs(string artistId)
        {
            return _context.Gigs.Include(g => g.Artist).Include(g=>g.Genre)
                .Where(g => g.DateTime > DateTime.Now && g.ArtistId == artistId);
        }
    }
}
