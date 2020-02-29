using GigHub.Contracts;
using GigHub.Models;
using GigHub.Security;
using GigHub.Services;
using GigHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GigHub.Controllers
{
    [Authorize]
    public class GigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IDataProtector _gigIdDataProtector;
        private readonly IDataProtector _imageValueDataProtector;
        private const string NoEncryption = "Select Encryption Type";
        private const string DataProtection = "1";
        private const string TripleDES = "2";
        private string _uniqueImageName = string.Empty;
        public GigsController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager, IHostEnvironment hostEnvironment,
            IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _gigIdDataProtector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.GigIdRouteValue);
            _imageValueDataProtector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.ImageValue);
            
        }

        public IActionResult Create()
        {
            var model = new GigsFormViewModel
            {
                Genres = _unitOfWork.Genres.GetGenres(),
                EncryptionTechniques = GetEncryptionTechniques()
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(GigsFormViewModel model)
        {
            var key = _userManager.GetUserId(User); 
            var signature = User.Identity.Name; 

           // Check if the model state is valid with correct inputs from the user.
            if (!ModelState.IsValid)
            {
                return ReturnEntryToUser(model);
            }

            // Check to see if a photo is selected by the user.
            if (model.Photo != null)
            {
                using (var readStream = model.Photo.OpenReadStream())
                {
                    var myImage = new Bitmap(Image.FromStream(readStream));
                    var getEncodedTextInPicture =
                        Steganography.GetTextFromPicture(myImage); // Container for saving encoded text in picture

                    // If the picture doesn't contains a text?
                    if (string.IsNullOrEmpty(getEncodedTextInPicture))
                    {
                        if (model.EncryptionType == NoEncryption) // The user chooses not to sign the picture.
                            SaveImage(myImage, out _uniqueImageName);
                        else
                            _uniqueImageName =
                                EncodeAndSaveImage(model, myImage, signature,
                                    key); // User chooses to sign the picture
                    }
                    else // The picture contains text.
                    {
                        var encryptionMethod = getEncodedTextInPicture.Substring(0, 1);
                        var decryptedSignature = string.Empty;

                        switch (encryptionMethod)
                        {
                            //Decrypt picture using Data Protection.
                            case DataProtection:
                            {
                                decryptedSignature = _imageValueDataProtector.Unprotect(getEncodedTextInPicture.Substring(1));
                                if (signature == decryptedSignature)
                                    SaveImage(myImage, out _uniqueImageName);
                                else
                                    return ReturnPrivacyWarning(model); // picture doesn't belongs to the uploader of the image.
                                break;
                            }
                            //Decrypt picture using Triple DES
                            case TripleDES:
                            {
                                var checker = getEncodedTextInPicture.Substring(1);
                                decryptedSignature = TripleDes.Decrypt(getEncodedTextInPicture.Substring(1), key);
                                if (signature == decryptedSignature)
                                    SaveImage(myImage, out _uniqueImageName);
                                else return ReturnPrivacyWarning(model);
                                break;
                            }
                            default:
                                return ReturnPrivacyWarning(model);
                        }
                    }
                }
            }


            // Create a new gig object.
            var gig = new Gig
            {
                ArtistId = _userManager.GetUserId(User),
                DateTime = model.GetDateTime(),
                GenreId = model.Genre,
                Venue = model.Venue,
                ImageUrl = _uniqueImageName
            };

            // Persist the gig object into the database.
            _unitOfWork.Gigs.AddAGig(gig);
            _unitOfWork.Complete();

            // Redirect the user to the list of his/her upcoming gigs.
            return View("MyUpcomingGigs", GetMyUpcomingModel(key));
        }

        public IActionResult Edit(string id)
        {
            var empId = Convert.ToInt32(_gigIdDataProtector.Unprotect(id));
            var gig = _unitOfWork.Gigs.FindGigById(empId);

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
            return View(_unitOfWork.Gigs.GetUpcomingGigs()
                .Select(g =>
                {
                    g.EncryptedGigId = _gigIdDataProtector.Protect(g.Id.ToString());
                    return g;
                }));
        }

        public IActionResult MyUpcomingGigs()
        {
            var artistId = _userManager.GetUserId(User);
            return View(GetMyUpcomingModel(artistId));
        }

        private IEnumerable<Gig> GetMyUpcomingModel(string artistId)
        {
            return _unitOfWork.Gigs.GetMyUpcomingGigs(artistId)
                .Select(g =>
                {
                    g.EncryptedGigId = _gigIdDataProtector.Protect(g.Id.ToString());
                    return g;
                });
        }

        private IActionResult ReturnEntryToUser(GigsFormViewModel model)
        {
            model.Genres = _unitOfWork.Genres.GetGenres();
            model.EncryptionTechniques = GetEncryptionTechniques();

            return View("Create", model);
        }

        private IActionResult ReturnPrivacyWarning(GigsFormViewModel model)
        {
            ModelState.AddModelError("PrivacyWarning", "You are trying to upload image that belongs to someone else. The image has privacy and cannot be uploaded. Please change a picture");
            model.Genres = _unitOfWork.Genres.GetGenres();
            model.EncryptionTechniques = GetEncryptionTechniques();

            return View("Create", model);
        }
        private static IEnumerable<SelectListItem> GetEncryptionTechniques()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem("Data Protection", "1"),
                new SelectListItem("Triple DES", "2")
            };
        }

        private string EncodeAndSaveImage(GigsFormViewModel model, Bitmap myImage, string signature, string key)
        {
            string uniqueImageName;
            string encodedSignature; // Container for encoded signature.

            switch (model.EncryptionType)
            {
                // If the user is not interested in signing the picture, save the picture.
                case "Select Encryption Type":
                {
                    SaveImage(myImage, out uniqueImageName);
                    break;
                }
                case "1": // If user selected to sign his picture using Data Protection API
                    encodedSignature = _imageValueDataProtector.Protect(signature);
                    // Embed signature with 1 added to indicate Data Protection API Encryption was applied.
                    var dataProtectionSignedImage = Steganography
                        .EmbedTextToPicture($"1{encodedSignature}", myImage);
                    SaveImage(dataProtectionSignedImage, out uniqueImageName);
                    break;
                default: // If the user selected to sign his picture using Triple DES Encryption.
                {
                    encodedSignature = Security.TripleDes.Encrypt(signature, key);

                    // Embed signature with 2 added to indicate Triple DES Encryption was applied.
                    var tripleDesSignedImage = Steganography
                        .EmbedTextToPicture($"2{encodedSignature}", myImage);
                    SaveImage(tripleDesSignedImage, out uniqueImageName);
                    break;
                }
            }

            return uniqueImageName;
        }
        private void SaveImage(Bitmap myImage, out string imageName)
        {
            var imagesFolder = Path.Combine(_hostEnvironment.ContentRootPath, @"wwwroot\images");
            imageName = Guid.NewGuid() + ".jpg";
            var filePath = Path.Combine(imagesFolder, imageName);

            myImage.Save(filePath);
        }

        //private async Task<bool> IsSignatureAvailable(string decryptedSignature)
        //{
        //    var user = await _userManager.FindByEmailAsync(decryptedSignature);
        //    return user != null;
        //}
    }
}