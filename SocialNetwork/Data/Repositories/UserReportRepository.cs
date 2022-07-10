using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Repositories
{
    public interface IUserReportRepository : IRepository<UserReport>
    {
    }

    public class UserReportRepository : Repository<UserReport>, IUserReportRepository
    {
        public UserReportRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
