using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class BaseModel
    {
        [NotMapped]
        public int RowOrder { get; set; }

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
