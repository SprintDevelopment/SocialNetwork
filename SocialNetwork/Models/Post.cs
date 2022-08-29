using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class Post : HasAuthor
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public bool Reported { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Image { get; set; } = "";

        public DateTime? EditTime { get; set; }
        
        [Required]
        public bool AutoReport { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; } = "";

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

        public int? AnalysisId { get; set; }

        [ForeignKey(nameof(AnalysisId))]
        public Analysis Analysis { get; set; }
    }

    public class PostCreateOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }

        public virtual IEnumerable<string> Tags { get; set; }
    }

    public class PostUpdateOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }
    }

    public class SearchPostDto : ShouldReadAuthorData
    {
        public int Id { get; set; }

        public string Text { get; set; }

        [JsonProperty("user")]
        public string UserId { get; set; }

        public string Image { get; set; }

        public bool Reported { get; set; }

        public int Comments { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        [JsonProperty("time")]
        public string CreateTime { get; set; }

        [JsonProperty("edited_at")]
        public string EditTime { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Symbol { get; set; }

        [JsonProperty("my_vote")]
        public int MyVote { get; set; }
    }

    public class PersonalPostNotificationOrder
    {
        [JsonProperty("secret")]
        public string Secret { get; set; } = "9TgVm748%_+f=-?>1g";

        [JsonProperty("post_id")]
        public int Id { get; set; }

        [JsonProperty("post_text")]
        public string Text { get; set; }

        [JsonProperty("post_userid")]
        public string UserId { get; set; }

        [JsonProperty("post_username")]
        public string Username { get; set; }
    }

    public class SinglePostDto : SearchPostDto
    {
        [JsonProperty("post_author")]
        public SimpleUserDto Author { get; set; }
    }
}
