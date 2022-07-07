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
    public class PostReportController : Controller
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
        public async Task<PostReport> Create(PostReportCuOrder postReportCuOrder)
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

                    return postReport;
                }
            }

            return null;
        }
    }
}
