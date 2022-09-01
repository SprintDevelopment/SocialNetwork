using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("postreports")]
    public class PostReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PostReportController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IQueryable<PostReport> query = _unitOfWork.PostReports
                                    .Find()
                                    .Include(pr => pr.Author)
                                    .Include(pr => pr.ReportedPost).ThenInclude(p => p.PostTags);

            return Ok(query.OrderByDescending(pr => pr.CreateTime)
                        .Select(pr => _mapper.Map<SearchPostReportDto>(pr))
                        .AsEnumerable()
                        .Paginate(HttpContext.Request.GetDisplayUrl()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]PostReportCuOrder postReportCuOrder)
        {
            if (ModelState.IsValid)
            {
                var postReport = _mapper.Map<PostReport>(postReportCuOrder);
                var post = await _unitOfWork.Posts.GetAsync(postReport.ReportedPostId);

                if (post is not null)
                {
                    var preReport = _unitOfWork.PostReports.Find(pv => pv.ReportedPostId == postReport.ReportedPostId && pv.UserId == postReport.UserId).FirstOrDefault();
                    if (preReport is not null)
                        _mapper.Map(postReportCuOrder, preReport);
                    else
                        _unitOfWork.PostReports.Add(postReport);

                    await _unitOfWork.CompleteAsync();

                    return Ok(postReport);
                }

                return NotFound(new ResponseDto { Result = false, Error = "post not found." });
            }

            return BadRequest();
        }
    }
}
