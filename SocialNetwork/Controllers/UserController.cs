using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using SocialNetwork.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IMapper mapper, IUserService userService, ILogger<UserController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
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

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if (id.IsNullOrWhitespace())
                return BadRequest(new ResponseDto { Result = false, Error = "not enough input data" });

            var user = _unitOfWork.Users.Find(u => u.Id == id).FirstOrDefault();

            if (user is not null)
                return Ok(user);

            return NotFound();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create(UserCreateOrder userCuOrder)
        {
            if (ModelState.IsValid)
            {
                if (_unitOfWork.Users.Find(u => u.Id == userCuOrder.Id).Any())
                    return BadRequest(new UserError { Username = "user with this id already exists." });

                if (_unitOfWork.Users.Find(u => u.Username == userCuOrder.Username).Any())
                    return BadRequest(new UserError { Username = "user with this username already exists." });

                var user = _mapper.Map<User>(userCuOrder);
                _userService.TokenizeUser(user, false);
                _unitOfWork.Users.Add(user, true);

                return Ok(user);
            }

            return BadRequest(new DetailedResponse { Detail = "Unknown" });
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateUsername(UserUpdateUsernameOrder userUpdateUsernameOrder)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.Users.Find(u => u.Id == User.Identity.Name).FirstOrDefault();

                if (userUpdateUsernameOrder.Username == user.Username)
                    return BadRequest(new UserError { Username = "username is the same as previous." });

                if (_unitOfWork.Users.Find(u => u.Username == userUpdateUsernameOrder.Username).Any())
                    return BadRequest(new UserError { Username = "user with this username already exists." });

                user.Username = userUpdateUsernameOrder.Username;
                await _unitOfWork.CompleteAsync();

                return Ok(user);
            }

            return BadRequest(new DetailedResponse { Detail = "Unknown" });
        }
    }
}
