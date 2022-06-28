using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(ILogger<CommentController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Comment> Get()
        {
            //_unitOfWork.Posts.Add(new Post
            //{
            //    CreatedAt = DateTime.Now,
            //    Text = "New post",
            //    UserID = "4f4abcde-757f-433c-92a2-03a6da043a0c",
            //    Image = "",
            //    EditedAt = null,
            //    Description = "",
            //    Score = 1,
            //    ScoreTime = 2,
            //    Symbol = "ETHUSDT",
            //    AutoReportTime = null,
            //}, true);

            return _unitOfWork.Comments.GetAll().AsEnumerable();
        }

        [HttpPost]
        public async Task<Comment> Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Comments.Add(comment);
                await _unitOfWork.Posts.Find(p => p.ID == comment.PostID).ForEachAsync(p => p.Comments++);
                
                await _unitOfWork.CompleteAsync();

                return comment;
            }
            return null;
        }
    }
}