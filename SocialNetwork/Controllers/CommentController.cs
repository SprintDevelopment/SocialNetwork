using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
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
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public IActionResult Get(int post, int offset, int limit)
        {
            IQueryable<Comment> query = _unitOfWork.Comments
                    .Find()
                    .Include(c => c.Author);
            
            if (User.Identity.IsAuthenticated)
                query = query.Include(p => p.CommentVotes.Where(pv => pv.UserId == User.FindFirst("userId").Value));

            query = post == 0 ? query : query.Where(c => c.PostId == post); // post
            
            return Ok(query.OrderBy(c => c.CreateTime)
                        .Select(c => _mapper.Map<SearchCommentDto>(c))
                        .AsEnumerable()
                        .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CommentCuOrder commentCuOrder)
        {
            Log.Warning(JsonConvert.SerializeObject(commentCuOrder));
            if (ModelState.IsValid)
            {
                var post = _unitOfWork.Posts.Find(p => p.Id == commentCuOrder.PostId).FirstOrDefault();

                if (post is not null)
                {
                    var user = _unitOfWork.Users.Find(u => u.Id == User.FindFirst("userId").Value).FirstOrDefault();
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

                    return Ok(_mapper.Map<SingleCommentDto>(comment));
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
                    var user = _unitOfWork.Users.Find(u => u.Id == User.FindFirst("userId").Value).FirstOrDefault();
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