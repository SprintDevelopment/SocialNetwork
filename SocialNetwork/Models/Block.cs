﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class Block : HasUserId
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("id")]
        public DateTime Time { get; set; }

        [Required]
        [Column("id")]
        public string BlockedId { get; set; }

        [ForeignKey(nameof(BlockedId))]
        public User BlockedUser { get; set; }
    }

    public class BlockDto
    {
        public long Id { get; set; }

        public DateTime Time { get; set; }

        public SimpleUserDto BlockedUser { get; set; }
    }

    public class BlockCuOrder : ShouldPassUserId
    {
        [Required]
        public string BlockedId { get; set; }
    }
}
