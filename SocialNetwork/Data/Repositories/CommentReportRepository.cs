using SocialNetwork.Models;

namespace SocialNetwork.Data.Repositories
{
    public interface ICommentReportRepository : IRepository<CommentReport>
    {
    }

    public class CommentReportRepository : Repository<CommentReport>, ICommentReportRepository
    {
        public CommentReportRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
