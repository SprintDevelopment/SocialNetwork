using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
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
            IQueryable<Post> query = _unitOfWork.Posts.Find().Include(p => p.PostTags).Include(p => p.PostVotes.Where(pv => pv.UserId == User.Identity.Name));

            query = user.IsNullOrWhitespace() ? query : query.Where(p => p.UserId == user); // user
            query = date is null ? query : query.Where(p => p.CreateTime.Date == date.Value.Date); // date
            query = tag.IsNullOrWhitespace() ? query : query.Where(p => p.PostTags.Any(pt => pt.TagID == tag)); // tag

            return query.OrderBy(p => p.CreateTime)
                        .Paginate(offset, limit)
                        .Select(p => _mapper.Map<SearchPostDto>(p))
                        .AsEnumerable();
        }

        [HttpGet("latests")]
        public IEnumerable<SearchPostDto> Latests(int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts.Find().Include(p => p.PostTags).Include(p => p.PostVotes.Where(pp => pp.UserId == User.Identity.Name));

            return query.OrderByDescending(p => p.CreateTime)
                        .Paginate(offset, limit)
                        .Select(p => _mapper.Map<SearchPostDto>(p))
                        .AsEnumerable();
        }

        [HttpGet("mine")]
        public IEnumerable<SearchPostDto> MyPosts(int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts.Find(p => p.UserId == User.Identity.Name).Include(p => p.PostTags).Include(p => p.PostVotes.Where(pp => pp.UserId == User.Identity.Name));

            return query.OrderByDescending(p => p.CreateTime)
                        .Paginate(offset, limit)
                        .Select(p => _mapper.Map<SearchPostDto>(p))
                        .AsEnumerable();
        }

        [HttpGet("{post_id}")]
        public SinglePostDto Get(int post_id)
        {
            return _unitOfWork.Posts
                    .Find(p => p.Id == post_id)
                    .Include(p => p.Author)
                    .Include(p => p.PostTags)
                    .Include(p => p.PostVotes.Where(pp => pp.UserId == User.Identity.Name))
                    .Select(p => _mapper.Map<SinglePostDto>(p))
                    .FirstOrDefault();
        }

        [HttpPost]
        public async Task<Post> Create(PostCuOrder postCuOrder)
        {
            if (ModelState.IsValid)
            {
                
                var post = _mapper.Map<Post>(postCuOrder);

                _unitOfWork.Posts.Add(post, true);
                _unitOfWork.PostTags.AddRange(post.PostTags.Select(pt => new PostTag { PostID = post.Id, TagID = pt.TagID }));
                
                await _unitOfWork.CompleteAsync();

                return post;
            }
            return null;
        }

        // posts/ -> {user, tag, limit, offset, order, date} - query
        // posts/{post_id} -> { post_id }
    }
}