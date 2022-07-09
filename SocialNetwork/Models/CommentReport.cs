using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class CommentReport : HasUserId
    {
        [Key]
        public int Id { get; set; }

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
        public int ReportedCommentId { get; set; }
    }
}
