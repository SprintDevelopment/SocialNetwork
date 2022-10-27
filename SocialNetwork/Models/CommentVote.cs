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
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        
        [Required]
        [Column("is_down")]
        public bool IsDown { get; set; }
        
        [Required]
        [Column("comment_id")]
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
