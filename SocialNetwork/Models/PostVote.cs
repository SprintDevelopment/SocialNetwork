using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SocialNetwork.Models
{
    public class PostVote : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public bool IsDown { get; set; }
        
        [Required]
        public int PostId { get; set; }
        
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
