using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PostController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PostController(IMapper mapper, ILogger<PostController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<SearchPostDto> Get(string user, string tag, DateTime? date, int offset, int limit)
        {
            //_unitOfWork.Posts.Add(new Post
            //{
            //    CreateTime = DateTime.Now,
            //    Text = "New post",
            //    UserId = "d74a1dc7-3b5b-4c96-8945-e976c5364564",
            //    Image = "",
            //    EditTime = null,
            //    Description = "",
            //    Score = 1,
            //    ScoreTime = 2,
            //    Symbol = "ETHUSDT",
            //    AutoReportTime = null,
            //}, true);

            IQueryable<Post> query = _unitOfWork.Posts.Find().Include(p => p.PostTags).Include(p => p.PostVotes.Where(pp => pp.UserID == "645a1679-3a51-4ca8-8530-134f2b148ab3"));

            query = user.IsNullOrWhitespace() ? query : query.Where(p => p.UserId == user); // user
            query = date is null ? query : query.Where(p => p.CreateTime.Date == date.Value.Date); // date
            query = tag.IsNullOrWhitespace() ? query : query.Where(p => p.PostTags.Any(pt => pt.TagID == tag)); // tag

            return query.OrderBy(p => p.CreateTime)
                        .Paginate(offset, limit)
                        .Select(p => _mapper.Map<SearchPostDto>(p))
                        .AsEnumerable();
        }

        [HttpPost]
        public Post Create(Post post)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Posts.Add(post, true);

                _unitOfWork.PostTags.AddRange(post.PostTags.Select(pt => new PostTag { PostID = post.Id, TagID = pt.TagID }));
                _unitOfWork.CompleteAsync().Wait();

                return post;
            }
            return null;
        }

        // posts/ -> {user, tag, limit, offset, order, date} - query
        // posts/{post_id} -> { post_id }
    }
}