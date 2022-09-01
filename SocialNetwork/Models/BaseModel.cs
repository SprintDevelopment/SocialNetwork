using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


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

    public class HasUserId : BaseModel
    {
        [Required]
        public string UserId { get; set; }
    }

    public class HasAuthor : HasUserId
    {
        [ForeignKey(nameof(UserId))]
        public User Author { get; set; }
    }

    public class ShouldPassUserId { }

    public class ShouldReadAuthorData 
    {
        [JsonIgnore]
        public string Username { get; set; }

        [JsonIgnore]
        public bool UserVerified { get; set; }
    }
}
