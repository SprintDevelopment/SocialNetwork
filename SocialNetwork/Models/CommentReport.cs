using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SocialNetwork.Models
{
    public class CommentReport : HasUserId
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int ReportedCommentId { get; set; }

        [Required]
        public bool Checked { get; set; }
    }

    public class CommentReportCuOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }

        [Required]
        [BindProperty(Name = "reply_to")] 
        public int ReportedCommentId { get; set; }
    }
}
