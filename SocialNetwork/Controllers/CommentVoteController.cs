using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("[controller]")]
    public class CommentVoteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CommentVoteController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CommentVoteController(IMapper mapper, ILogger<CommentVoteController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<CommentVote> Create(CommentVoteCuOrder CommentVoteCuOrder)
        {
            if (ModelState.IsValid)
            {
                var commentVote = _mapper.Map<CommentVote>(CommentVoteCuOrder);
                var comment = await _unitOfWork.Comments.GetAsync(commentVote.CommentId);

                if (comment is not null)
                {
                    var preVote = _unitOfWork.CommentVotes.Find(pv => pv.CommentId == commentVote.CommentId && pv.UserId == commentVote.UserId).FirstOrDefault();
                    if (preVote is not null)
                    {
                        _ = commentVote.IsDown ? comment.Dislikes-- : comment.Likes--;
                        preVote.IsDown = commentVote.IsDown;
                    }
                    else
                        _unitOfWork.CommentVotes.Add(commentVote);

                    _ = commentVote.IsDown ? comment.Dislikes++ : comment.Likes++;

                    await _unitOfWork.CompleteAsync();

                    return commentVote;
                }
            }

            return null;
        }
    }
}
