using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class PostTag : BaseModel
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Required]
        [Column("post_id")]
        public int PostID { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(PostID))]
        public Post Post { get; set; }

        [Required]
        [Column("post_id")]
        public string TagID { get; set; }
    }
}
