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
        private const string TripleDes = "2";
        private const string New = "New";
        private const string Modify = "Modify";
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
                EncryptionTechniques = GetEncryptionTechniques(), UserAction = New
            };

            return View("GigForm", model);
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
                if (ProcessImageBeforeSaving(model, signature, key, out var actionResult)) return actionResult;
            }

            if (model.UserAction == New)
            {
                // Create a new gig object.
                var gig = new Gig
                { ArtistId = _userManager.GetUserId(User), DateTime = model.GetDateTime(), GenreId = model.Genre,
                    Venue = model.Venue, ImageUrl = _uniqueImageName};
                
                // Add gig object into gig collections.
                _unitOfWork.Gigs.AddAGig(gig);
            }
            else
            {
                // Modify a gig object.
                var gigInDb = _unitOfWork.Gigs.FindGigById(model.GigId);
                gigInDb.DateTime = model.GetDateTime(); 
                gigInDb.Venue = model.Venue;
                gigInDb.GenreId = model.Genre;
                if (model.Photo != null) gigInDb.ImageUrl = _uniqueImageName;
            }
            
            _unitOfWork.Complete();

            // Redirect the user to the list of his/her upcoming gigs.
            return View("MyUpcomingGigs", GetMyUpcomingModel(key));
        }

        public IActionResult Edit(string id)
        {
            var gigId = Convert.ToInt32(_gigIdDataProtector.Unprotect(id));
            var gig = _unitOfWork.Gigs.FindGigById(gigId);

            //var model = new EditGigFormViewModel(gig)
            //{
            //    Genres = _unitOfWork.Genres.GetGenres()
            //};

            var model = new GigsFormViewModel()
            {
                Genres = _unitOfWork.Genres.GetGenres(), UserAction = Modify, EncryptionTechniques = GetEncryptionTechniques(), GigId = gig.Id,
                Venue = gig.Venue, Genre = gig.GenreId, Date = gig.DateTime.ToShortDateString(), Time = gig.DateTime.ToShortTimeString()
            };

            return View("GigForm", model);
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

        private bool ProcessImageBeforeSaving(GigsFormViewModel model, string signature, string key,
            out IActionResult actionResult)
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
                    string decryptedSignature;

                    switch (encryptionMethod)
                    {
                        //Decrypt picture using Data Protection.
                        case DataProtection:
                        {
                            decryptedSignature = _imageValueDataProtector.Unprotect(getEncodedTextInPicture.Substring(1));
                            if (signature == decryptedSignature)
                                SaveImage(myImage, out _uniqueImageName);
                            else
                            {
                                actionResult = ReturnPrivacyWarning(model);
                                return true;
                            }

                            break;
                        }
                        //Decrypt picture using Triple DES
                        case TripleDes:
                        {
                            decryptedSignature = Security.TripleDes.Decrypt(getEncodedTextInPicture.Substring(1), key);
                            if (signature == decryptedSignature)
                                SaveImage(myImage, out _uniqueImageName);
                            else
                            {
                                actionResult = ReturnPrivacyWarning(model);
                                return true;
                            }

                            break;
                        }
                        default:
                        {
                            actionResult = ReturnPrivacyWarning(model);
                            return true;
                        }
                    }
                }
            }

            actionResult = null;
            return false;
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

            return View("GigForm", model);
        }

        private IActionResult ReturnPrivacyWarning(GigsFormViewModel model)
        {
            ModelState.AddModelError("PrivacyWarning", "A copyright image detected, please replace image and try again.");
            model.Genres = _unitOfWork.Genres.GetGenres();
            model.EncryptionTechniques = GetEncryptionTechniques();

            return View("GigForm", model);
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
        
    }
}