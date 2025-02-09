﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GigHub.Models
{
    public class Gig
    {
        public int Id { get; set; }

        [NotMapped]
        public string EncryptedGigId { get; set; }

        public ApplicationUser Artist { get; set; }

        [Required]
        public string ArtistId { get; set; }

        public DateTime DateTime { get; set; }

        [Required, StringLength(255)]
        public string Venue { get; set; }

        public Genre Genre { get; set; }

        [Required, Display(Name = "Genre")]
        public byte GenreId { get; set; }

        public string ImageUrl { get; set; }

        public bool IsCanceled { get; set; }
    }
}
