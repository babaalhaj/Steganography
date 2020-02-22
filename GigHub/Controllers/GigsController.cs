using GigHub.Models;
using GigHub.Services;
using GigHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(GigsFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = _unitOfWork.Genres.GetGenres();
                return View(model);
            }

            var gig = new Gig
            {
                ArtistId = _userManager.GetUserId(User), 
                DateTime = model.GetDateTime(), GenreId = model.Genre, Venue = model.Venue
            };

            _unitOfWork.Gigs.AddAGig(gig);
            _unitOfWork.Complete();

            return RedirectToAction("Index", "Home");
        }

    }
}