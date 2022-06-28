using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SocialNetwork.Models
{
    public class PostTag : BaseModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int PostID { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(PostID))]
        public Post Post { get; set; }

        [Required]
        public string TagID { get; set; }
    }
}
