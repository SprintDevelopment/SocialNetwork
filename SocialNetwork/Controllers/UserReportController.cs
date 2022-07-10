using AutoMapper;
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
    public class UserReportController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserReportController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UserReportController(IMapper mapper, ILogger<UserReportController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<UserReport> Create(UserReportCuOrder userReportCuOrder)
        {
            if (ModelState.IsValid)
            {
                var userReport = _mapper.Map<UserReport>(userReportCuOrder);
                var user = await _unitOfWork.Users.GetAsync(userReport.ReportedUserId);

                if (user is not null)
                {
                    var preReport = _unitOfWork.UserReports.Find(pv => pv.ReportedUserId == userReport.ReportedUserId && pv.UserId == userReport.UserId).FirstOrDefault();
                    if (preReport is not null)
                        _mapper.Map(userReportCuOrder, preReport);
                    else
                        _unitOfWork.UserReports.Add(userReport);

                    await _unitOfWork.CompleteAsync();

                    return userReport;
                }
            }

            return null;
        }
    }
}
