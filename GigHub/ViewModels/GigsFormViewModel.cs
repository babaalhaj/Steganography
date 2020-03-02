using GigHub.BusinessRules;
using GigHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GigHub.ViewModels
{
    public class GigsFormViewModel
    {
        [Required]
        public string Venue { get; set; }

        [Required, FutureDate]
        public string Date { get; set; }

        [Required, ValidTime]
        public string Time { get; set; }

        [Required, Display(Name = "Genre")]
        public byte Genre { get; set; }

        public int GigId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public DateTime GetDateTime() => DateTime.Parse($"{Date} {Time}");
        public IFormFile Photo { get; set; }
        public IEnumerable<SelectListItem> EncryptionTechniques { get; set; }
        public string EncryptionType { get; set; }
        public string UserAction { get; set; }
        
    }
}
