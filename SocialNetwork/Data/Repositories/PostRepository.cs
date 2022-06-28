using SocialNetwork.Assets.Extensions;
using SocialNetwork.Assets.Values.Constants;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Data.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        IEnumerable<Post> GetLatest(int pageNumber);
    }

    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public IEnumerable<Post> GetLatest(int pageNumber = 0)
        {
            return Find(x => true).OrderBy(x => x.CreatedAt).Paginate(pageNumber);
        }
    }
}
