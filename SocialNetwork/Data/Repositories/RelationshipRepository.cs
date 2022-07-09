using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Repositories
{
    public interface IRelationshipRepository : IRepository<Relationship>
    {
    }

    public class RelationshipRepository : Repository<Relationship>, IRelationshipRepository
    {
        public RelationshipRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }
    }
}
