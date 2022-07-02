using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SocialNetwork.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
        }

        public BaseModel Clone()
        {
            return (BaseModel)MemberwiseClone();
        }
    }
}
