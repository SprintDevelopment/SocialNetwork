using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Dtos;
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
    [Route("users")]
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
        [HttpPost("Login")]
        public IActionResult Login(UserLoginOrder userLoginOrder)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.Users.Find(u => u.Username == userLoginOrder.Username && u.Password == userLoginOrder.Password).FirstOrDefault();

                if (user != null)
                {
                    _userService.TokenizeUser(user);
                    return Ok(user);
                }

                return NotFound(new ResponseDto { Result = false, Error = "no such user found" });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data" });
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _unitOfWork.Users.Find().AsEnumerable();
        }

        [HttpPost]
        public IActionResult Create()
        {

        }
    }
}
