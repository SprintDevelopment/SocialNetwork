using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SocialNetwork.Models
{
    public class CommentReport : HasAuthor
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int ReportedCommentId { get; set; }

        [ForeignKey(nameof(ReportedCommentId))]
        public Comment ReportedComment { get; set; }

        [Required]
        public bool Checked { get; set; }
    }

    public class CommentReportCuOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }

        [Required]
        [BindProperty(Name = "reported_comment")] 
        public int ReportedCommentId { get; set; }
    }

    public class SearchCommentReportDto : ShouldReadAuthorData
    {
        [JsonProperty("time")]
        public DateTime CreateTime { get; set; }

        public string Text { get; set; }

        [JsonProperty("reporter")]
        new public string Username { get; set; }

        [JsonProperty("comment")]
        public SimpleCommentDto ReportedComment { get; set; }
    }
}
