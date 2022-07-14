using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public IEnumerable<SearchCommentDto> Get(int post, int offset, int limit)
        {
            IQueryable<Comment> query = _unitOfWork.Comments.Find().Include(c => c.CommentVotes.Where(cv => cv.UserId == User.Identity.Name));

            query = post != 0 ? query : query.Where(c => c.PostId == post); // user

            return query.OrderBy(c => c.CreateTime)
                        .Paginate(offset, limit)
                        .Select(c => _mapper.Map<SearchCommentDto>(c))
                        .AsEnumerable();
        }

        [HttpPost]
        public async Task<Comment> Create(CommentCuOrder commentCuOrder)
        {
            if (ModelState.IsValid)
            {
                var comment = _mapper.Map<Comment>(commentCuOrder);

                _unitOfWork.Comments.Add(comment, true);
                // codes for add comment count 
                await _unitOfWork.CompleteAsync();

                return comment;
            }
            return null;
        }
    }
}