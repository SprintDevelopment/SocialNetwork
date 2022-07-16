using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("comments")]
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CommentController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IMapper mapper, ILogger<CommentController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(int post, int offset, int limit)
        {
            IQueryable<Comment> query = _unitOfWork.Comments.Find().Include(c => c.Author).Include(c => c.CommentVotes.Where(cv => cv.UserId == User.Identity.Name));

            query = post != 0 ? query : query.Where(c => c.PostId == post); // post

            return Ok(query.OrderBy(c => c.CreateTime)
                        .Paginate(offset, limit)
                        .Select(c => _mapper.Map<SearchCommentDto>(c))
                        .AsEnumerable());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentCuOrder commentCuOrder)
        {
            if (ModelState.IsValid)
            {
                var post = _unitOfWork.Posts.Find(p => p.Id == commentCuOrder.PostId).FirstOrDefault();

                if (post is not null)
                {
                    var user = _unitOfWork.Users.Find(u => u.Id == User.Identity.Name).FirstOrDefault();
                    var comment = _mapper.Map<Comment>(commentCuOrder);

                    if (!user.Verified && !user.WhiteList)
                    {
                        var validateReulst = _unitOfWork.BlackListPatterns.ValidateMessage(commentCuOrder.Text);
                        if (!validateReulst.IsNullOrWhitespace())
                        {
                            comment.Reported = true;
                            comment.AutoReport = true;
                            comment.Description = validateReulst;

                            // CHECK FOR REPORT USER . . .
                        }
                    }

                    _unitOfWork.Comments.Add(comment, true);
                    post.Comments++;

                    await _unitOfWork.CompleteAsync();

                    return Ok(comment);
                }

                return BadRequest(new ResponseDto { Result = false, Error = "no post found with this postID." });
            }

            return BadRequest();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]CommentCuOrder commentCuOrder)
        {
            if (ModelState.IsValid)
            {
                var comment = _unitOfWork.Comments.Find(c => c.Id == id).FirstOrDefault();

                if (comment is not null)
                {
                    var user = _unitOfWork.Users.Find(u => u.Id == User.Identity.Name).FirstOrDefault();
                    _mapper.Map(commentCuOrder, comment);

                    if (!user.Verified && !user.WhiteList)
                    {
                        var validateReulst = _unitOfWork.BlackListPatterns.ValidateMessage(commentCuOrder.Text);
                        if (!validateReulst.IsNullOrWhitespace())
                        {
                            comment.Reported = true;
                            comment.AutoReport = true;
                            comment.Description = validateReulst;

                            // CHECK FOR REPORT USER . . .
                        }
                    }

                    await _unitOfWork.CompleteAsync();

                    return Ok(comment);
                }

                return NotFound(new ResponseDto { Result = false, Error = "comment not found." });
            }

            return BadRequest();
        }
    }
}