using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper; 
        private readonly ILogger<PostController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PostController(IMapper mapper, ILogger<PostController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper; 
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<SearchPostDto> Get()
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

            return _unitOfWork.Posts.GetAll().Include(p => p.PostTags).Select(p => _mapper.Map<SearchPostDto>(p)).AsEnumerable();
        }

        [HttpPost]
        public Post Create(Post post)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Posts.Add(post, true);

                _unitOfWork.PostTags.AddRange(post.PostTags.Select(pt => new PostTag { PostID = post.Id, TagID = pt.TagID }));
                _unitOfWork.CompleteAsync().Wait();

                return post;
            }
            return null;
        }
    }
}