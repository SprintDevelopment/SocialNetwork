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
    [Route("postvotes")]
    public class PostVoteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PostVoteController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]PostVoteCuOrder postVoteCuOrder)
        {
            if (ModelState.IsValid)
            {
                var postVote = _mapper.Map<PostVote>(postVoteCuOrder);
                var post = await _unitOfWork.Posts.GetAsync(postVote.PostId);

                if (post is not null)
                {
                    _unitOfWork.PostVotes.Add(postVote);

                    _ = postVote.IsDown ? post.Dislikes++ : post.Likes++;

                    await _unitOfWork.CompleteAsync();

                    return Ok(postVote);
                }

                return NotFound(new ResponseDto { Result = false, Error = "post not found." });
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm]PostVoteCuOrder postVoteCuOrder)
        {
            if (ModelState.IsValid)
            {
                var post = await _unitOfWork.Posts.GetAsync(postVoteCuOrder.PostId);

                if (post is not null)
                {
                    var preVote = _unitOfWork.PostVotes.Find(pv => pv.PostId == postVoteCuOrder.PostId && pv.UserId == User.Identity.Name).FirstOrDefault();
                    if (preVote is not null)
                    {
                        _ = preVote.IsDown ? post.Dislikes-- : post.Likes--;
                        preVote.IsDown = postVoteCuOrder.IsDown;
                        _ = preVote.IsDown ? post.Dislikes++ : post.Likes++;

                        await _unitOfWork.CompleteAsync();

                        return Ok(preVote);
                    }

                    return NotFound(new ResponseDto { Result = false, Error = "post vote not found." });
                }

                return NotFound(new ResponseDto { Result = false, Error = "post not found." });
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(PostVoteCuOrder postVoteCuOrder)
        {
            if (ModelState.IsValid)
            {
                var post = await _unitOfWork.Posts.GetAsync(postVoteCuOrder.PostId);

                var preVote = _unitOfWork.PostVotes.Find(pv => pv.PostId == postVoteCuOrder.PostId && pv.UserId == User.Identity.Name).FirstOrDefault();
                if (preVote is not null)
                {
                    _ = preVote.IsDown ? post.Dislikes-- : post.Likes--;

                    _unitOfWork.PostVotes.Remove(preVote);
                    await _unitOfWork.CompleteAsync();

                    return Ok(preVote);
                }

                return NotFound(new ResponseDto { Result = false, Error = "post vote not found." });
            }

            return BadRequest();
        }
    }
}
