using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class User : BaseModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? BlockedUntil { get; set; }

        [Required]
        public bool Reported { get; set; }

        [Required]
        public bool Verified { get; set; }

        [Required]
        public bool WhiteList { get; set; }

        [Required]
        public bool ReportCandidate { get; set; }

        [Required]
        public float AdminReputation { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Token { get; internal set; }

        [Required(AllowEmptyStrings = true)]
        public string Password { get; internal set; }
    }

    public class UserLoginOrder
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
