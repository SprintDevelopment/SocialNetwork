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
    [Route("postreports")]
    public class PostReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PostReportController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PostReportController(IMapper mapper, ILogger<PostReportController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
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
