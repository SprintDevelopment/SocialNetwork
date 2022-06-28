using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class PostVote : BaseModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsDown { get; set; }
        
        [Required]
        public int PostID { get; set; }
        
        [Required]
        public string UserID { get; set; }
    }
}
