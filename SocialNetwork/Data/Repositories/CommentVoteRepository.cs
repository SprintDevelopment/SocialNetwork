using SocialNetwork.Models;

namespace SocialNetwork.Data.Repositories
{
    public interface ICommentVoteRepository : IRepository<CommentVote>
    {
    }

    public class CommentVoteRepository : Repository<CommentVote>, ICommentVoteRepository
    {
        public CommentVoteRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
