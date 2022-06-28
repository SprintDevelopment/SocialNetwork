using SocialNetwork.Models;

namespace SocialNetwork.Data.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
    }

    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
