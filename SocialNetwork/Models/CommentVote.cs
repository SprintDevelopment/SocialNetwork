using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class CommentVote : BaseModel
    {
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDown { get; set; }
        public int CommentID { get; set; }
        public string UserID { get; set; }
    }
}
