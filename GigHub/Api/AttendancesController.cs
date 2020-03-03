using GigHub.Contracts;
using GigHub.Dtos;
using GigHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GigHub.Api
{
    [Route("api/[controller]"), Authorize]
    public class AttendancesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendancesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        } 

        // POST api/<controller>
        [HttpPost]
        public IActionResult Post(GigsDto gigsDto)
        {
            var userId = _userManager.GetUserId(User);
            var attendanceInDb = _unitOfWork.Attendances.AllAttendances()
                .SingleOrDefault(a=>a.AttendeeId == userId && a.GigId == gigsDto.GigId);

            if (attendanceInDb == null) // If attendance doesn't exists add attendance else remove attendance.
            {
                var attendance = new Attendance{GigId = gigsDto.GigId, AttendeeId = userId};
                _unitOfWork.Attendances.AddAttendance(attendance);
            }
            else _unitOfWork.Attendances.RemoveAttendance(attendanceInDb);
                
            _unitOfWork.Complete();

            return Ok();
        }

    }
}
