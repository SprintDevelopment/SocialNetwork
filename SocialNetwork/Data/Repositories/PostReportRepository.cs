using SocialNetwork.Models;

namespace SocialNetwork.Data.Repositories
{
    public interface IPostReportRepository : IRepository<PostReport>
    {
    }

    public class PostReportRepository : Repository<PostReport>, IPostReportRepository
    {
        public PostReportRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
