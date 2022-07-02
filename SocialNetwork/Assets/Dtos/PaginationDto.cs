using System.Collections.Generic;

namespace SocialNetwork.Assets.Dtos
{
    public class PaginationDto<T>
    {
        public string Next { get; set; }
        public string Previous { get; set; }
        public IEnumerable<T> Results{ get; set; }
    }
}
