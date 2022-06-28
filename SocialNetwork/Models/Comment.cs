using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class Comment : BaseModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public bool Reported { get; set; }
        
        [Required]
        public int PostID { get; set; }

        [ForeignKey(nameof(PostID))]
        public Post Post { get; set; }
        
        public int? ReplyTo { get; set; }
        
        [Required]
        public string UserID { get; set; }
        
        public DateTime? EditedAt { get; set; }
        
        [Required]
        public bool AutoReport { get; set; }
        
        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }
        
        [Required]
        public bool AdminWhitelist { get; set; }
        
        [Required]
        public bool NotificationSent { get; set; }
        
        [Required]
        public int Likes { get; set; }
        
        [Required]
        public int Dislikes { get; set; }
        
        [Required]
        public int Replies { get; set; }
    }
}
