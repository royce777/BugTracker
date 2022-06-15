﻿using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class AttachmentDTO
    {
        public Attachment Attachment { get; set; }
        public List<Attachment> AttachmentList { get; set; }

        [Required(ErrorMessage = "Please attach a file ! ")]
        public IFormFile FormFile { get; set; }
    }
}
