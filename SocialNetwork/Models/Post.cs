﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class Post : BaseModel
    {
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
        public bool Reported { get; set; }
        public string UserID { get; set; }
        public IEnumerable<string> Image { get; set; }
        public IEnumerable<DateTime> EditedAt { get; set; }
        public bool AutoReport { get; set; }
        public string Description { get; set; }
        public bool AdminWhitelist { get; set; }
        public float Score { get; set; }
        public float ScoreTime { get; set; }
        public string Symbol { get; set; }
        public IEnumerable<DateTime> AutoReportTime { get; set; }
        public int Comments { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public IEnumerable<PostTag> PostTags { get; set; }
        public bool NotificationSent { get; set; }
    }
}
