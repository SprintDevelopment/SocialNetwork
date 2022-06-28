using SocialNetwork.Models;

namespace SocialNetwork.Data.Repositories
{
    public interface IPostVoteRepository : IRepository<PostVote>
    {
    }

    public class PostVoteRepository : Repository<PostVote>, IPostVoteRepository
    {
        public PostVoteRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
