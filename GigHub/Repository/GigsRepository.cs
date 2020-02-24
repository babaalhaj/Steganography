using System;
using System.Collections.Generic;
using System.Linq;
using GigHub.Contracts;
using GigHub.Data;
using GigHub.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
