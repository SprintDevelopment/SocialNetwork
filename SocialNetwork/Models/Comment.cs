using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class Comment : BaseModel
    {
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
        public bool Reported { get; set; }
        public int PostID { get; set; }
        public int? ReplyTo { get; set; }
        public string UserID { get; set; }
        public IEnumerable<DateTime> EditedAt { get; set; }
        public bool AutoReport { get; set; }
        public string Description { get; set; }
        public bool AdminWhitelist { get; set; }
        public bool NotificationSent { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Replies { get; set; }
    }
}
