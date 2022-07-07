using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("[controller]")]
    public class PostVoteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PostVoteController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PostVoteController(IMapper mapper, ILogger<PostVoteController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<PostVote> Create(PostVoteCuOrder postVoteCuOrder)
        {
            if (ModelState.IsValid)
            {
                var postVote = _mapper.Map<PostVote>(postVoteCuOrder);
                var post = await _unitOfWork.Posts.GetAsync(postVote.PostId);

                if (post is not null)
                {
                    var preVote = _unitOfWork.PostVotes.Find(pv => pv.PostId == postVote.PostId && pv.UserId == postVote.UserId).FirstOrDefault();
                    if (preVote is not null)
                    {
                        _ = postVote.IsDown ? post.Dislikes-- : post.Likes--;
                        preVote.IsDown = postVote.IsDown;
                    }
                    else
                        _unitOfWork.PostVotes.Add(postVote);
                        
                    _ = postVote.IsDown ? post.Dislikes++ : post.Likes++;

                    await _unitOfWork.CompleteAsync();

                    return postVote;
                }
            }

            return null;
        }
    }
}
