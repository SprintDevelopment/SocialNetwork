using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using SocialNetwork.Services;
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
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUserService userService, ILogger<UserController> logger, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLoginOrder userLoginOrder)
        {
            var user = _unitOfWork.Users.Find(u => u.Username == userLoginOrder.Username && u.Password == userLoginOrder.Password).FirstOrDefault();

            if (user != null)
            {
                _userService.TokenizeUser(user);
                return Ok(user);
            }

            return Unauthorized();
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            for (int i = 0; i < 5; i++)
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
            return _unitOfWork.Users.Find().AsEnumerable();
        }
    }
}
