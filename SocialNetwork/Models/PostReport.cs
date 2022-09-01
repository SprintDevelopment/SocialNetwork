using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SocialNetwork.Models
{
    public class PostReport : HasAuthor
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }
        
        [Required]
        public string Text { get; set; }

        [Required]
        public int ReportedPostId { get; set; }

        [ForeignKey(nameof(ReportedPostId))]
        public Post ReportedPost { get; set; }

        [Required]
        public bool Checked { get; set; }
    }

    public class PostReportCuOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }

        [BindProperty(Name = "reported_post")]
        [Required]
        public int ReportedPostId { get; set; }
    }

    public class SearchPostReportDto : ShouldReadAuthorData
    {
        [JsonProperty("time")]
        public DateTime CreateTime { get; set; }

        public string Text { get; set; }

        [JsonProperty("reporter")]
        new public string Username { get; set; }

        [JsonProperty("post")]
        public SimplePostDto ReportedPost { get; set; }
    }
}
