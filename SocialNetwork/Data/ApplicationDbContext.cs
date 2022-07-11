using SocialNetwork.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Block> Blocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReport> CommentReports { get; set; }
        public DbSet<CommentVote> CommentVotes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReport> PostReports { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<PostVote> PostVotes { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
    }
}
