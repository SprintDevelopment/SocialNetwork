using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class Relationship : HasUserId
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [ForeignKey(nameof(UserId))]
        public User FollowerUser { get; set; }

        [Required]
        public string FollowingId { get; set; }

        [ForeignKey(nameof(FollowingId))]
        public User FollowingUser { get; set; }
    }

    public class RelationshipDto
    {
        public long Id { get; set; }

        public DateTime Time { get; set; }

        public SimpleUserDto FollowingUser { get; set; }
    }

    public class RelationshipStatusDto
    {
        [JsonProperty("is_follower")]
        public bool IsFollower { get; set; }

        [JsonProperty("is_following")]
        public bool IsFollowing { get; set; }
    }

    public class RelationshipInfoDto
    {
        [JsonProperty("follower")]
        public int Follower { get; set; }

        [JsonProperty("following")]
        public int Following { get; set; }

        [JsonProperty("user_verified")]
        public bool Verified { get; set; }
    }

    public class FollowerFollowingBlockedDto
    {
        public DateTime Time { get; set; }

        [JsonProperty("id")]
        public string UserId { get; set; }

        public string Username { get; set; }

        public bool Verified { get; set; }
    }

    public class FollowerDto : FollowerFollowingBlockedDto { }

    public class FollowingDto : FollowerFollowingBlockedDto { }

    public class BlockedDto : FollowerFollowingBlockedDto { }

    // Note that this is a model only for mapping. FollowingId will be passed via query string.
    public class RelationshipTemplate : ShouldPassUserId
    {
        [Required]
        public string FollowingId { get; set; }
    }
}
