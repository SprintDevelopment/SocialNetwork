using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Dtos;
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
    [Route("userreports")]
    public class UserReportController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserReportController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]UserReportCuOrder userReportCuOrder)
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

                    return Ok(userReport);
                }

                return NotFound(new ResponseDto { Result = false, Error = "user not found." });
            }

            return BadRequest();
        }
    }
}
