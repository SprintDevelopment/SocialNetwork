using System;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationDbContext GetContext();
        ICommentRepository Comments { get; }
        ICommentVoteRepository CommentVotes { get; }
        IPostRepository Posts { get; }
        IPostTagRepository PostTags { get; }
        IPostVoteRepository PostVotes { get; }
        IUserRepository Users { get; }

        Task<int> CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        public ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Comments = new CommentRepository(_context);
            CommentVotes = new CommentVoteRepository(_context);
            Posts = new PostRepository(_context);
            PostTags = new PostTagRepository(_context);
            PostVotes = new PostVoteRepository(_context);
            Users = new UserRepository(_context);
        }

        public ICommentRepository Comments { get; private set; }
        public ICommentVoteRepository CommentVotes { get; private set; }
        public IPostRepository Posts { get; private set; }
        public IPostTagRepository PostTags { get; private set; }
        public IPostVoteRepository PostVotes { get; private set; }
        public IUserRepository Users { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public ApplicationDbContext GetContext()
        {
            return _context;
        }
    }
}
