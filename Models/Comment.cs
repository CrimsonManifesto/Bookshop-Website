using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookshop_Website.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string ProfilePictureUrl { get; set; } = "/images/profiles/default-profile.png";

        public required int BookId { get; set; }

        [BindNever]
        public string? UserName { get; set; }

        public required string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("BookId")]
        [BindNever]
        public Books? Book { get; set; }
    }
}
