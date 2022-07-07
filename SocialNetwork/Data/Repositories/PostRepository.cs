using SocialNetwork.Assets.Extensions;
using SocialNetwork.Assets.Values.Constants;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Data.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
    }

    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
