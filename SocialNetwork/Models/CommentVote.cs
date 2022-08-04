using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class CommentVote : HasUserId
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        
        [Required]
        public bool IsDown { get; set; }
        
        [Required]
        public int CommentId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(CommentId))]
        public Comment Comment { get; set; }
    }

    public class CommentVoteCuOrder : ShouldPassUserId
    {
        [BindProperty(Name = "is_down")]
        [Required]
        public bool IsDown { get; set; }

        [BindProperty(Name = "comment")]
        [Required]
        public int CommentId { get; set; }
    }
}
