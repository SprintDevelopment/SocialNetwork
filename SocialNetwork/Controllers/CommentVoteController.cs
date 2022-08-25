using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("commentvotes")]
    public class CommentVoteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CommentVoteController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CommentVoteCuOrder CommentVoteCuOrder)
        {
            if (ModelState.IsValid)
            {
                var commentVote = _mapper.Map<CommentVote>(CommentVoteCuOrder);
                var comment = await _unitOfWork.Comments.GetAsync(commentVote.CommentId);

                if (comment is not null)
                {
                    _unitOfWork.CommentVotes.Add(commentVote);

                    _ = commentVote.IsDown ? comment.Dislikes++ : comment.Likes++;

                    await _unitOfWork.CompleteAsync();

                    return Ok(commentVote);
                }

                return NotFound(new ResponseDto { Result = false, Error = "comment not found." });
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] CommentVoteCuOrder CommentVoteCuOrder)
        {
            if (ModelState.IsValid)
            {
                var comment = await _unitOfWork.Comments.GetAsync(CommentVoteCuOrder.CommentId);

                if (comment is not null)
                {
                    var preVote = _unitOfWork.CommentVotes.Find(pv => pv.CommentId == CommentVoteCuOrder.CommentId && pv.UserId == User.FindFirst("userId").Value).FirstOrDefault();
                    if (preVote is not null)
                    {
                        _ = preVote.IsDown ? comment.Dislikes-- : comment.Likes--;
                        preVote.IsDown = CommentVoteCuOrder.IsDown;

                        _ = preVote.IsDown ? comment.Dislikes++ : comment.Likes++;

                        await _unitOfWork.CompleteAsync();

                        return Ok(preVote);
                    }

                    return NotFound(new ResponseDto { Result = false, Error = "comment vote not found." });
                }

                return NotFound(new ResponseDto { Result = false, Error = "comment not found." });
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] CommentVoteCuOrder commentVoteCuOrder)
        {
            if (ModelState.IsValid)
            {
                var comment = await _unitOfWork.Posts.GetAsync(commentVoteCuOrder.CommentId);

                var preVote = _unitOfWork.CommentVotes.Find(pv => pv.CommentId == commentVoteCuOrder.CommentId && pv.UserId == User.FindFirst("userId").Value).FirstOrDefault();
                if (preVote is not null)
                {
                    _ = preVote.IsDown ? comment.Dislikes-- : comment.Likes--;

                    _unitOfWork.CommentVotes.Remove(preVote);
                    await _unitOfWork.CompleteAsync();

                    return Ok(preVote);
                }

                return NotFound(new ResponseDto { Result = false, Error = "comment vote not found." });
            }

            return BadRequest();
        }
    }
}
