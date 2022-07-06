using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

    public class SimpleUserDto
    {
        public string Id { get; set; }

        public string Username { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("blocked_until")]
        public DateTime? BlockedUntil { get; set; }

        public bool Reported { get; set; }

        public bool Verified { get; set; }
    }
}
