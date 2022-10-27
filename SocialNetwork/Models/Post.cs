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
        [Column("id")]
        public int Id { get; set; }
        
        [Required]
        [Column("time")]
        public DateTime CreateTime { get; set; }

        [Required]
        [Column("text")]
        public string Text { get; set; }

        [Required]
        [Column("reported")]
        public bool Reported { get; set; }

        [Required(AllowEmptyStrings = true)]
        [Column("image")]
        public string Image { get; set; } = "";

        [Column("edited_at")]
        public DateTime? EditTime { get; set; }
        
        [Required]
        [Column("auto_report")]
        public bool AutoReport { get; set; }

        [Required(AllowEmptyStrings = true)]
        [Column("description")]
        public string Description { get; set; } = "";

        [Required]
        [Column("admin_whitelist")]
        public bool AdminWhitelist { get; set; }

        [Required]
        [Column("score")]
        public float Score { get; set; }

        [Required]
        [Column("score_time")]
        public float ScoreTime { get; set; }

        [Required]
        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("auto_report_time")]
        public DateTime? AutoReportTime { get; set; }
        
        [Required]
        [Column("comments")]
        public int Comments { get; set; }

        [Required]
        [Column("likes")]
        public int Likes { get; set; }

        [Required]
        [Column("dislikes")]
        public int Dislikes { get; set; }
        
        public virtual IEnumerable<PostTag> PostTags { get; set; }

        public virtual IEnumerable<PostVote> PostVotes { get; set; }
        
        [Required]
        [Column("notification_sent")]
        public bool NotificationSent { get; set; }
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
    public class SimplePostDto : ShouldReadAuthorData 
    {
        public int Id { get; set; }

        public string Text { get; set; }

        [JsonProperty("user")]
        public string UserId { get; set; }

        new public string Username { get; set; }

        [JsonProperty("user_avatar")]
        new public string UserAvatar { get; set; }

        [JsonProperty("user_verified")]
        new public bool UserVerified { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }

    public class SearchPostDto : SimplePostDto
    {
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
