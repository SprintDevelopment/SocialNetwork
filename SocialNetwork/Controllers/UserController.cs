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
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            for (int i = 0; i < 100; i++)
            {
                _unitOfWork.Users.Add(new User()
                {
                    Username = $"Username {i}",
                    Id = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.Now,
                    BlockedUntil = DateTime.Now,
                    AdminReputation = 0,
                    ReportCandidate = false,
                    WhiteList = true,
                    Reported = false,
                    Verified = true
                }, true);
            }
            return _unitOfWork.Users.GetAll().AsEnumerable();
        }
    }
}
