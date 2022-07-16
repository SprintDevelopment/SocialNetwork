using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class PostVote : HasUserId
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        public bool IsDown { get; set; }
        
        [Required]
        public int PostId { get; set; }

        [JsonIgnore]        
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
    }

    public class PostVoteCuOrder : ShouldPassUserId
    {
        [Required]
        public bool IsDown { get; set; }

        [Required]
        public int PostId { get; set; }
    }
}
