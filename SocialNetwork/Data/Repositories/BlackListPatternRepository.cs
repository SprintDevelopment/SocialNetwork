using SocialNetwork.Assets.Extensions;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Repositories
{
    public interface IBlackListPatternRepository : IRepository<BlackListPattern>
    {
        string ValidateMessage(string message);
    }

    public class BlackListPatternRepository : Repository<BlackListPattern>, IBlackListPatternRepository
    {
        public BlackListPatternRepository(ApplicationDbContext context) : base(context) { }

        public ApplicationDbContext DatabaseContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public string ValidateMessage(string message)
        {
            return DatabaseContext
                .BlackListPatterns
                .AsEnumerable()
                .Select(p => new Regex(p.Pattern).IsMatch(message) ? $"text should not contain '{p.Description}'" : "")
                .Where(cr => !cr.IsNullOrWhitespace())
                .Join(", ");
        }
    }
}
