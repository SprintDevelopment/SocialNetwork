using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialNetwork.Models
{
    public class Comment : HasAuthor
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

        [Required]
        [Column("post_id")]
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
        
        [Column("reply_to_id")]
        public int? ReplyTo { get; set; }

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
        [Column("likes")]
        public int Likes { get; set; }
        
        [Required]
        [Column("dislikes")]
        public int Dislikes { get; set; }
        
        [Required]
        public int Replies { get; set; }
        
        [Required]
        [Column("notification_sent")]
        public bool NotificationSent { get; set; }

        public virtual IEnumerable<CommentVote> CommentVotes { get; set; }
    }

    public class CommentCreateOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }
        
        [BindProperty(Name = "post")]
        [Required]
        public int PostId { get; set; }
        
        [BindProperty(Name = "reply_to")]
        public int? ReplyTo { get; set; }
    }

    public class CommentUpdateOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }
    }

    public class SimpleCommentDto : ShouldReadAuthorData
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool Reported { get; set; }

        public string Description { get; set; }

        [JsonProperty("user")]
        public string UserId { get; set; }

        new public string Username { get; set; }

        [JsonProperty("user_avatar")]
        new public string UserAvatar { get; set; }

        [JsonProperty("user_verified")]
        new public bool UserVerified { get; set; }
    }

    public class SearchCommentDto : SimpleCommentDto
    {
        [JsonProperty("time")]
        public string CreateTime { get; set; }

        [JsonProperty("post")]
        public int PostId { get; set; }

        [JsonProperty("reply_to")]
        public int? ReplyTo { get; set; }

        [JsonProperty("edited_at")]
        public string EditTime { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public int Replies { get; set; }

        [JsonProperty("my_vote")]
        public int MyVote { get; set; }
    }

    public class CommentNotificationOrder
    {
        [JsonProperty("secret")]
        public string Secret { get; set; } = "9TgVm748%_+f=-?>1g";

        [JsonProperty("post_id")]
        public int PostId { get; set; }

        [JsonProperty("comment_id")]
        public int CommentId { get; set; }

        [JsonProperty("comment_text")]
        public string CommentText { get; set; }

        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; }
    }

    public class ReplyToPostNotificationOrder : CommentNotificationOrder
    {
        [JsonProperty("comment_username")]
        public string CommentUsername { get; set; }

        [JsonProperty("post_userid")]
        public string PostUserId { get; set; }

        [JsonProperty("post_text")]
        public string PostText { get; set; }
    }

    public class ReplyToCommentNotificationOrder : CommentNotificationOrder
    {
        [JsonProperty("reply_username")]
        public string ReplyUsername { get; set; }

        [JsonProperty("comment_userid")]
        public string CommentUserId { get; set; }

        [JsonProperty("reply_text")]
        public string ReplyText { get; set; }
    }

    public class SingleCommentDto : SearchCommentDto
    {
        public SimpleUserDto Author { get; set; }
    }
}
