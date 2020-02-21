using GigHub.Models;
using GigHub.Services;
using GigHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GigHub.Controllers
{
    [Authorize]
    public class GigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public GigsController(IUnitOfWork unitOfWork, 
            UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Create()
        {
            var model = new GigsFormViewModel
            {
                Genres = _unitOfWork.Genres.GetGenres()
            };
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(GigsFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var gig = new Gig
            {
                ArtistId = _userManager.GetUserId(User), 
                DateTime = DateTime.Parse($"{model.Date} {model.Time}"), GenreId = model.Genre, Venue = model.Venue
            };

            _unitOfWork.Gigs.AddAGig(gig);
            _unitOfWork.Complete();

            return RedirectToAction("Index", "Home");
        }

    }
}