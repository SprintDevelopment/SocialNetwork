using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class User : BaseModel
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [Column("time")]
        public DateTime CreateTime { get; set; }

        [Column("blocked_until")]
        public DateTime? BlockedUntil { get; set; }

        [Required]
        [Column("reported")]
        public bool Reported { get; set; }

        [Required]
        [Column("verified")]
        public bool Verified { get; set; }

        [Required]
        [Column("white_list")]
        public bool WhiteList { get; set; }

        [Required]
        [Column("report_candidate")]
        public bool ReportCandidate { get; set; }

        [Required]
        [Column("admin_reputation")]
        public float AdminReputation { get; set; }
    }

    public class UserCreateOrder
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Username { get; set; }
    }

    public class UserUpdateUsernameOrder
    {
        [Required]
        public string Username { get; set; }
    }

    public class FakeFormForUploadAvatar
    {
        [Required]
        public string fakeField { get; set; }
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

        [JsonProperty("user_avatar")]
        public string Avatar { get; set; }

        [JsonProperty("created_at")]
        public string CreateTime { get; set; }

        [JsonProperty("blocked_until")]
        public string BlockedUntil { get; set; }

        public bool Reported { get; set; }

        public bool Verified { get; set; }
    }

    public class UserError
    {
        public string ID { get; set; }
        public string[] Username { get; set; }
    }
}
