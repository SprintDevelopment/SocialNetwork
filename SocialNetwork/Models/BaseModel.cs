using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SocialNetwork.Models
{
    public class BaseModel
    {
        [JsonIgnore]
        [NotMapped]
        public int RowOrder { get; set; }

        [JsonIgnore]
        [NotMapped]
        public bool IsSelected { get; set; }

        public BaseModel()
        {
        }

        public BaseModel Clone()
        {
            return (BaseModel)MemberwiseClone();
        }
    }
}
