using GigHub.Contracts;
using GigHub.Data;
using GigHub.Models;
using System.Collections.Generic;
using System.Linq;

namespace GigHub.Repository
{
    public class AttendancesRepository : IAttendancesRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendancesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddAttendance(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
        }

        public void RemoveAttendance(Attendance attendance)
        {
            _context.Attendances.Remove(attendance);
        }

        public IEnumerable<Attendance> AllAttendances()
        {
            return _context.Attendances.ToList();
        }
    }
}
