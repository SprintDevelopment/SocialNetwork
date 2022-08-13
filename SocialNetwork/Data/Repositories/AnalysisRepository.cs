using SocialNetwork.Models;

namespace SocialNetwork.Data.Repositories
{
    public interface IAnalysisRepository : IRepository<Analysis>
    {
    }

    public class AnalysisRepository : Repository<Analysis>, IAnalysisRepository
    {
        public AnalysisRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
