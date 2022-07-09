﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    [Route("[controller]")]
    public class CommentReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CommentReportController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CommentReportController(IMapper mapper, ILogger<CommentReportController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<CommentReport> Create(CommentReportCuOrder CommentReportCuOrder)
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

                    return CommentReport;
                }
            }

            return null;
        }
    }
}