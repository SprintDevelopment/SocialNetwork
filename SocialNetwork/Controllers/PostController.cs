using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PostController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get(string user, string tag, string date, int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                                    .Find()
                                    .Include(p => p.Author)
                                    .Include(p => p.PostTags)
                                    .Include(p => p.PostVotes.Where(pv => pv.UserId == User.Identity.Name));

            query = user.IsNullOrWhitespace() ? query : query.Where(p => p.UserId == user); // user
            query = date.IsNullOrWhitespace() || !DateTime.TryParseExact(date, "ddd MMM dd HH:mm:ss 'GMT'K yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var outDate) ? query : query.Where(p => p.CreateTime.Date == outDate.Date); // date
            query = tag.IsNullOrWhitespace() ? query : query.Where(p => p.PostTags.Any(pt => pt.TagID == tag)); // tag

            return Ok(query.OrderBy(p => p.CreateTime)
                        .Select(p => _mapper.Map<SearchPostDto>(p))
                        .AsEnumerable()
                        .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [AllowAnonymous]
        [HttpGet("latests")]
        public IActionResult Latests(int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                                    .Find()
                                    .Include(p => p.Author)
                                    .Include(p => p.PostTags)
                                    .Include(p => p.PostVotes.Where(pp => pp.UserId == User.Identity.Name));

            return Ok(query.OrderByDescending(p => p.CreateTime)
                        .Select(p => _mapper.Map<SearchPostDto>(p))
                        .AsEnumerable()
                        .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [HttpGet("mine")]
        public IActionResult MyPosts(int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                                    .Find(p => p.UserId == User.Identity.Name)
                                    .Include(p => p.PostTags)
                                    .Include(p => p.PostVotes.Where(pp => pp.UserId == User.Identity.Name));

            return Ok(query.OrderByDescending(p => p.CreateTime)
                        .Select(p => _mapper.Map<SearchPostDto>(p))
                        .AsEnumerable()
                        .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [AllowAnonymous]
        [HttpGet("{post_id}")]
        public IActionResult Get(int post_id)
        {
            var singlePost = _unitOfWork.Posts
                    .Find(p => p.Id == post_id)
                    .Include(p => p.Author)
                    .Include(p => p.PostTags)
                    .Include(p => p.PostVotes.Where(pp => pp.UserId == User.Identity.Name))
                    .Select(p => _mapper.Map<SinglePostDto>(p))
                    .FirstOrDefault();

            if (singlePost is null)
                return NotFound();

            return Ok(singlePost);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]PostCuOrder postCuOrder)
        {
            if (ModelState.IsValid)
            {
                if (postCuOrder.Tags?.Count() > 2)
                    return BadRequest(new ErrorResponse { Error = "too many tags" });

                // UPLOAD IMAGE . . .

                var user = _unitOfWork.Users.Find(u => u.Id == User.Identity.Name).FirstOrDefault();
                var post = _mapper.Map<Post>(postCuOrder);

                if (!user.Verified && !user.WhiteList)
                {
                    var validateReulst = _unitOfWork.BlackListPatterns.ValidateMessage(postCuOrder.Text);
                    if (!validateReulst.IsNullOrWhitespace())
                    {
                        post.AutoReport = true;
                        post.AutoReportTime = DateTime.Now;
                        post.Description = validateReulst;

                        // CHECK FOR REPORT USER . . .
                    }
                }

                _unitOfWork.Posts.Add(post, true);
                _unitOfWork.PostTags.AddRange(post.PostTags.Select(pt => new PostTag { PostID = post.Id, TagID = pt.TagID }));

                await _unitOfWork.CompleteAsync();

                return Ok(post);
            }

            return BadRequest();
        }

        // posts/ -> {user, tag, limit, offset, order, date} - query
        // posts/{post_id} -> { post_id }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]PostCuOrder postCuOrder)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.Users.Find(u => u.Id == User.Identity.Name).FirstOrDefault();
                var post = _unitOfWork.Posts.Find(p => p.Id == id).FirstOrDefault();

                if(post is not null)
                {
                    if (postCuOrder.Tags?.Count() > 2)
                        return BadRequest(new ErrorResponse { Error = "too many tags" });

                    // UPLOAD IMAGE . . .

                    _mapper.Map(postCuOrder, post);

                    if (!user.Verified && !user.WhiteList)
                    {
                        var validateReulst = _unitOfWork.BlackListPatterns.ValidateMessage(postCuOrder.Text);
                        if (!validateReulst.IsNullOrWhitespace())
                        {
                            post.AutoReport = true;
                            post.AutoReportTime = DateTime.Now;
                            post.Description = validateReulst;

                            // CHECK FOR REPORT USER . . .
                        }
                    }

                    //_unitOfWork.PostTags.AddRange(post.PostTags.Select(pt => new PostTag { PostID = post.Id, TagID = pt.TagID }));

                    await _unitOfWork.CompleteAsync();

                    return Ok(post);
                }

                return NotFound(new ResponseDto { Result = false, Error = "post not found." });
            }

            return BadRequest();
        }
    }
}