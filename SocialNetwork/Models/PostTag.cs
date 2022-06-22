using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class PostTag : BaseModel
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public string TagID { get; set; }
    }
}
