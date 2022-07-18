using Newtonsoft.Json;
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
        public DateTime CreateTime { get; set; }

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

    public class UserCreateOrder
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UserUpdateUsernameOrder
    {
        [Required]
        public string Username { get; set; }
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

        [JsonProperty("created_at")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("blocked_until")]
        public DateTime? BlockedUntil { get; set; }

        public bool Reported { get; set; }

        public bool Verified { get; set; }
    }

    public class UserError
    {
        public string ID { get; set; }
        public string Username { get; set; }
    }
}
