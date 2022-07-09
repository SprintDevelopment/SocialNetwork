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

    public class RelationshipCuOrder : ShouldPassUserId
    {
        [Required]
        public string FollowingId { get; set; }
    }
}
