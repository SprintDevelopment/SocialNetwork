using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class PostReport : HasUserId
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }
        
        [Required]
        public string Text { get; set; }

        [Required]
        public int ReportedPostId { get; set; }

        [Required]
        public bool Checked { get; set; }
    }

    public class PostReportCuOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public int ReportedPostId { get; set; }
    }
}
