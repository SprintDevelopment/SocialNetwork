﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialNetwork.Models
{
    public class Comment : HasUserId
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

        [Required]
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
        
        public int? ReplyTo { get; set; }
        
        public DateTime? EditTime { get; set; }
        
        [Required]
        public bool AutoReport { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; } = "";
        
        [Required]
        public bool AdminWhitelist { get; set; }
        
        [Required]
        public int Likes { get; set; }
        
        [Required]
        public int Dislikes { get; set; }
        
        [Required]
        public int Replies { get; set; }
        
        [Required]
        public bool NotificationSent { get; set; }

        public virtual IEnumerable<CommentVote> CommentVotes { get; set; }
    }

    public class CommentCuOrder : ShouldPassUserId
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
        
        [BindProperty(Name = "post")]
        [Required]
        public int PostId { get; set; }
        
        [BindProperty(Name = "reply_to")]
        public int? ReplyTo { get; set; }
    }

    public class SearchCommentDto
    {
        public int Id { get; set; }

        [JsonProperty("time")]
        public string CreateTime { get; set; }

        public string Text { get; set; }

        public bool Reported { get; set; }

        [JsonProperty("post")]
        public int PostId { get; set; }

        [JsonProperty("reply_to")]
        public int? ReplyTo { get; set; }

        [JsonProperty("user")]
        public string UserId { get; set; }

        [JsonProperty("edited_time")]
        public string EditTime { get; set; }

        public string Description { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public int Replies { get; set; }

        public string Username { get; set; }

        [JsonProperty("user_verified")]
        public string UserVerified { get; set; }

        [JsonProperty("my_vote")]
        public int MyVote { get; set; }
    }

    public class SingleCommentDto : SearchCommentDto
    {
        public SimpleUserDto Author { get; set; }
    }
}
