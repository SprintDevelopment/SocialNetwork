using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SocialNetwork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PostController(ILogger<PostController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Post> Get(string user, string tag, DateTime? date, int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts.Find().Include(p => p.PostTags);

            if (!user.IsNullOrWhitespace())
                query = query.Where(p => p.UserID == user);

            if (date != null)
                query = query.Where(p => p.CreatedAt == date);

            if (!tag.IsNullOrWhitespace())
                query = query.Where(p => p.PostTags.Any(pt => pt.TagID == tag));

            return query.AsEnumerable();
        }

        [HttpPost]
        public Post Create(Post post)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Posts.Add(post, true);

                _unitOfWork.PostTags.AddRange(post.PostTags.Select(pt => new PostTag { PostID = post.ID, TagID = pt.TagID }));
                _unitOfWork.CompleteAsync().Wait();

                return post;
            }
            return null;
        }

        // posts/ -> {user, tag, limit, offset, order, date} - query
        // posts/{post_id} -> { post_id }
    }
}