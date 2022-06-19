using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class Post
    {
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
        public bool Reported { get; set; }
        public string UserID { get; set; }
        public int MyProperty { get; set; }
    }
}
