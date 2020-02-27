using GigHub.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace GigHub.ViewModels
{
    public class EditGigFormViewModel
    {
        public EditGigFormViewModel(Gig gig)
        {
            if (gig.DateTime == null)
             throw new ArgumentNullException(gig.DateTime.ToString(CultureInfo.InvariantCulture));

            Gig = gig;
            Date = gig.DateTime.ToShortDateString();
            Time = gig.DateTime.ToShortTimeString();
        }
        public Gig Gig { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public IFormFile Photo { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string Time { get; set; }

    }
}
