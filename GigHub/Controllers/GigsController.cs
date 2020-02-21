using GigHub.Services;
using GigHub.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GigHub.Controllers
{
    public class GigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GigsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Create()
        {
            var model = new GigsFormViewModel
            {
                Genres = _unitOfWork.Genres.GetGenres()
            };
            
            return View(model);
        }
    }
}