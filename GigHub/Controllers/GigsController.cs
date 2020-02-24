using GigHub.Models;
using GigHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using GigHub.Contracts;

namespace GigHub.Controllers
{
    [Authorize]
    public class GigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostEnvironment _hostEnvironment;

        public GigsController(IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager, IHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
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
            
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                var imagesFolder = Path.Combine(_hostEnvironment.ContentRootPath, @"wwwroot\images");
                uniqueFileName = Guid.NewGuid() + "_" + model.Photo.FileName;
                var filePath = Path.Combine(imagesFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            var gig = new Gig
            {
                ArtistId = _userManager.GetUserId(User),
                DateTime = model.GetDateTime(),
                GenreId = model.Genre,
                Venue = model.Venue,
                ImageUrl = uniqueFileName
            };

            _unitOfWork.Gigs.AddAGig(gig);
            _unitOfWork.Complete();

            return RedirectToAction("Index", "Home");
        }

    }
}