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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("commentreports")]
    public class CommentReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CommentReportController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IQueryable<CommentReport> query = _unitOfWork.CommentReports
                                    .Find()
                                    .Include(p => p.Author)
                                    .Include(p => p.ReportedComment);

            return Ok(query.OrderByDescending(cr => cr.CreateTime)
                        .Select(cr => _mapper.Map<SearchCommentReportDto>(cr))
                        .Paginate(HttpContext.Request.GetDisplayUrl()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CommentReportCuOrder CommentReportCuOrder)
        {
            if (ModelState.IsValid)
            {
                var CommentReport = _mapper.Map<CommentReport>(CommentReportCuOrder);
                var Comment = await _unitOfWork.Comments.GetAsync(CommentReport.ReportedCommentId);

                if (Comment is not null)
                {
                    var preReport = _unitOfWork.CommentReports.Find(pv => pv.ReportedCommentId == CommentReport.ReportedCommentId && pv.UserId == CommentReport.UserId).FirstOrDefault();
                    if (preReport is not null)
                        _mapper.Map(CommentReportCuOrder, preReport);
                    else
                        _unitOfWork.CommentReports.Add(CommentReport);

                    await _unitOfWork.CompleteAsync();

                    return Ok(CommentReport);
                }

                return NotFound(new ResponseDto { Result = false, Error = "comment not found." });
            }

            return BadRequest();
        }
    }
}
