using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SocialNetwork.Models
{
    public class Post : HasUserId
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public bool Reported { get; set; }

        [ForeignKey(nameof(UserId))]
        public User Author { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Image { get; set; }

        public DateTime? EditTime { get; set; }
        
        [Required]
        public bool AutoReport { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        [Required]
        public bool AdminWhitelist { get; set; }

        [Required]
        public float Score { get; set; }

        [Required]
        public float ScoreTime { get; set; }

        [Required]
        public string Symbol { get; set; }

        public DateTime? AutoReportTime { get; set; }
        
        [Required]
        public int Comments { get; set; }

        [Required]
        public int Likes { get; set; }

        [Required]
        public int Dislikes { get; set; }
        
        public virtual IEnumerable<PostTag> PostTags { get; set; }

        public virtual IEnumerable<PostVote> PostVotes { get; set; }
        
        [Required]
        public bool NotificationSent { get; set; }
    }

    public class PostCuOrder : ShouldPassUserId
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Image { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        [Required]
        public string Symbol { get; set; }

        public virtual IEnumerable<string> Tags { get; set; }
    }

    public class SearchPostDto
    {
        public int Id { get; set; }

        public string Text { get; set; }

        [JsonPropertyName("user")]
        public string UserId { get; set; }

        public string Image { get; set; }

        public bool Reported { get; set; }

        public int Comments { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        [JsonPropertyName("time")]
        public DateTime CreateTime { get; set; }

        [JsonPropertyName("edited_at")]
        public DateTime? EditTime { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Symbol { get; set; }

        public string Username { get; set; }

        [JsonPropertyName("user_verified")]
        public string UserVerified { get; set; }

        [JsonPropertyName("my_vote")]
        public int MyVote { get; set; }
    }

    public class SinglePostDto : SearchPostDto
    {
        [JsonPropertyName("post_author")]
        public SimpleUserDto Author { get; set; }
    }
}
