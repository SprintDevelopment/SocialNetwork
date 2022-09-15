using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Assets.Values.Constants;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("comments")]
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;

        public CommentController(IMapper mapper, IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public IActionResult Get(int post, int offset, int limit)
        {
            IQueryable<Comment> query = _unitOfWork.Comments
                    .Find(c => !c.Reported)
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
        public async Task<IActionResult> Create([FromForm] CommentCreateOrder commentCreateOrder)
        {
            Log.Warning(JsonConvert.SerializeObject(commentCreateOrder));
            if (ModelState.IsValid)
            {
                var post = _unitOfWork.Posts.Find(p => p.Id == commentCreateOrder.PostId).Include(p => p.PostTags).FirstOrDefault();

                if (post is not null)
                {
                    var user = _unitOfWork.Users.Find(u => u.Id == User.FindFirst("userId").Value).FirstOrDefault();
                    var comment = _mapper.Map<Comment>(commentCreateOrder);

                    #region BLOCKED USERS
                    var commentId = commentCreateOrder.ReplyTo;
                    var commentContributerIds = new List<string> { post.UserId };

                    while (commentId is not null)
                    {
                        var previousCommentInfo = _unitOfWork.Comments.Find(c => c.Id == commentId).Select(c => new { c.UserId, c.ReplyTo }).FirstOrDefault();
                        commentContributerIds.Add(previousCommentInfo.UserId);

                        commentId = previousCommentInfo.ReplyTo;
                    }

                    if (_unitOfWork.Blocks.Find(b => b.BlockedId == comment.UserId && commentContributerIds.Any(c => c == b.UserId)).Any())
                        return Ok(_mapper.Map<SingleCommentDto>(comment));
                    #endregion

                    if (!user.Verified && !user.WhiteList)
                    {
                        var validateReulst = _unitOfWork.BlackListPatterns.ValidateMessage(commentCreateOrder.Text);
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

                    {
                        StringContent notification = null;

                        if (comment.ReplyTo is null && post.UserId != user.Id)
                            notification = new StringContent(JsonConvert.SerializeObject(
                                new ReplyToPostNotificationOrder
                                {
                                    CommentId = comment.Id,
                                    CommentText = comment.Text,
                                    CommentUsername = user.Username,
                                    PostId = post.Id,
                                    PostText = post.Text,
                                    PostUserId = post.UserId,
                                    Tags = post.PostTags.Select(t => t.TagID)
                                }),
                                Encoding.UTF8,
                                Application.Json);
                        else if (comment.ReplyTo is not null)
                        {
                            var previousComment = _unitOfWork.Comments.Find(c => c.Id == comment.ReplyTo.Value).FirstOrDefault();

                            if (previousComment is not null && previousComment.UserId != user.Id) // Don't send notifcation for reply to yourself
                                notification = new StringContent(JsonConvert.SerializeObject(
                                    new ReplyToCommentNotificationOrder
                                    {
                                        CommentId = comment.Id,
                                        CommentText = previousComment.Text,
                                        ReplyUsername = user.Username,
                                        PostId = post.Id,
                                        ReplyText = comment.Text,
                                        CommentUserId = previousComment.UserId,
                                        Tags = post.PostTags.Select(t => t.TagID)
                                    }),
                                    Encoding.UTF8,
                                    Application.Json);
                        }

                        if (notification != null)
                        {
                            var httpClient = _httpClientFactory.CreateClient(NamedClientsConstants.NOTIFICATION_CLIENT);

                            using var httpResponseMessage =
                                await httpClient.PostAsync(comment.ReplyTo is null ? "comment" : "reply", notification);

                            httpResponseMessage.EnsureSuccessStatusCode();
                        }
                    }

                    return Ok(_mapper.Map<SingleCommentDto>(comment));
                }

                return BadRequest(new ResponseDto { Result = false, Error = "no post found with this postID." });
            }

            return BadRequest();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CommentUpdateOrder commentUpdateOrder)
        {
            Log.Warning(JsonConvert.SerializeObject(commentUpdateOrder));
            if (ModelState.IsValid)
            {
                var comment = _unitOfWork.Comments.Find(c => c.Id == id).FirstOrDefault();
                Log.Warning(JsonConvert.SerializeObject(comment));

                if (comment is not null)
                {
                    if (comment.UserId != User.FindFirst("userId").Value)
                        return BadRequest(new ErrorResponse { Error = "ownership error" });

                    _mapper.Map(commentUpdateOrder, comment);
                    Log.Warning(JsonConvert.SerializeObject(comment));

                    await _unitOfWork.CompleteAsync();

                    return Ok(_mapper.Map<SingleCommentDto>(comment));
                }

                return NotFound(new ResponseDto { Result = false, Error = "comment not found." });
            }

            return BadRequest();
        }
    }
}