﻿using GigHub.Models;
using System.Collections.Generic;

namespace GigHub.Contracts
{
    public interface IAttendancesRepository
    {
        void AddAttendance(Attendance attendance);
        void RemoveAttendance(Attendance attendance);
        List<Gig> GigsIAmAttending(string userId);
        IEnumerable<Attendance> AllAttendances();
    }
}
