using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class BlackListPattern : BaseModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Pattern { get; set; }
        
        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }
    }
}
