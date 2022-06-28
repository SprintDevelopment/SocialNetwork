using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class Post : BaseModel
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
        public string UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User Author { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Image { get; set; }

        public DateTime? EditedAt { get; set; }
        
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
        
        [Required]
        public bool NotificationSent { get; set; }
    }
}
