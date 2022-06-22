using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class User : BaseModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime BlockedUntil { get; set; }
        public bool Reported { get; set; }
        public bool Verified { get; set; }
        public bool WhiteList { get; set; }
        public bool ReportCandidate { get; set; }
        public float AdminReputation { get; set; }
    }
}
