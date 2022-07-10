using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class UserReport : HasUserId
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int ReportedUserId { get; set; }

        [Required]
        public bool Checked { get; set; }
    }

    public class UserReportCuOrder : ShouldPassUserId
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string ReportedUserId { get; set; }
    }
}
