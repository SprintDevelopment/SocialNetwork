using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SocialNetwork.Models
{
    public class UserReport : HasUserId
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("time")]
        public DateTime CreateTime { get; set; }

        [Required]
        [Column("text")]
        public string Text { get; set; }

        [Required]
        [Column("reported_user_id")]
        public string ReportedUserId { get; set; }

        [Required]
        [Column("checked")]
        public bool Checked { get; set; }
    }

    public class UserReportCuOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }

        [BindProperty(Name = "reported_user")]
        [Required]
        public string ReportedUserId { get; set; }
    }
}
