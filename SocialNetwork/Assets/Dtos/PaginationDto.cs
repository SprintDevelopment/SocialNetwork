using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Assets.Dtos
{
    public class PaginationDto<T>
    {
        public string Next { get; set; }
        public string Previous { get; set; }
        public IEnumerable<T> Results{ get; set; }
        public int Count { get => Results.Count(); }
    }
}
