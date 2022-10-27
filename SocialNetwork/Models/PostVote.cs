using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class PostVote : HasUserId
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
        [Column("post_id")]
        public int PostId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
    }

    public class PostVoteCuOrder : ShouldPassUserId
    {
        [BindProperty(Name = "is_down")]
        [Required]
        public bool IsDown { get; set; }

        [BindProperty(Name = "post")]
        [Required]
        public int PostId { get; set; }
    }
}
