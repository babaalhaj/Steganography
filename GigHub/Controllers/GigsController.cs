using GigHub.Contracts;
using GigHub.Models;
using GigHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace GigHub.Controllers
{
    [Authorize]
    public class GigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork; private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostEnvironment _hostEnvironment;

        public GigsController(IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager, IHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork; _userManager = userManager; _hostEnvironment = hostEnvironment;
        }

        public IActionResult Create()
        {
            var model = new GigsFormViewModel { Genres = _unitOfWork.Genres.GetGenres()};
            
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
            
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                uniqueFileName = this.GetFileName(model, out var filePath);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            var gig = new Gig
            {
                ArtistId = _userManager.GetUserId(User), DateTime = model.GetDateTime(), GenreId = model.Genre,
                Venue = model.Venue, ImageUrl = uniqueFileName
            };

            _unitOfWork.Gigs.AddAGig(gig);
            _unitOfWork.Complete();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Edit(int id)
        {
            var gig = _unitOfWork.Gigs.FindGigById(id);
            var model = new EditGigFormViewModel(gig)
            {
                Genres = _unitOfWork.Genres.GetGenres()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditGigFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = _unitOfWork.Genres.GetGenres();
                return View(model);
            }

            var artistId = _userManager.GetUserId(User);
            return RedirectToAction("MyUpcomingGigs", _unitOfWork.Gigs.GetMyUpcomingGigs(artistId));
        }

        [AllowAnonymous]
        public IActionResult AllUpcomingGigs()
        {
            return View(_unitOfWork.Gigs.GetUpcomingGigs());
        }

        public IActionResult MyUpcomingGigs()
        {
            var artistId = _userManager.GetUserId(User);
            return View(_unitOfWork.Gigs.GetMyUpcomingGigs(artistId));
        }

        private string GetFileName(GigsFormViewModel model, out string filePath)
        {
            var imagesFolder = Path.Combine(_hostEnvironment.ContentRootPath, @"wwwroot\images");
            var uniqueFileName = Guid.NewGuid() + "_" + model.Photo.FileName;
            filePath = Path.Combine(imagesFolder, uniqueFileName);
            return uniqueFileName;
        }

    }
}