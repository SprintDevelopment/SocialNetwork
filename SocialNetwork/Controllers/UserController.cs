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
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IMapper mapper, IUserService userService, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromForm] UserLoginOrder userLoginOrder)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.Users.Find(u => u.Username == userLoginOrder.Username).FirstOrDefault();

                if (user != null)
                {
                    _userService.TokenizeUser(user);
                    return Ok(user);
                }

                return NotFound(new ResponseDto { Result = false, Error = "no such user found" });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data" });
        }

        [AllowAnonymous]
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
        public IActionResult Create([FromForm] UserCreateOrder userCuOrder)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(userCuOrder);

                if (_unitOfWork.Users.Find(u => u.Id == user.Id).Any())
                    return BadRequest(new UserError { ID = "user with this id already exists." });

                if (_unitOfWork.Users.Find(u => u.Username == user.Username).Any())
                    return BadRequest(new UserError { Username = new string[] { "user with this username already exists." } });

                _userService.TokenizeUser(user);
                _unitOfWork.Users.Add(user, true);

                return Ok(user);
            }

            return BadRequest(new DetailedResponse { Detail = "Unknown" });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUsername(string id, [FromForm] UserUpdateUsernameOrder userUpdateUsernameOrder)
        {
            if (!id.IsNullOrEmpty() && ModelState.IsValid)
            {
                var user = _unitOfWork.Users.Find(u => u.Id == id).FirstOrDefault();

                if (userUpdateUsernameOrder.Username == user.Username)
                    return BadRequest(new UserError { Username = new string[] { "username is the same as previous." } });

                if (_unitOfWork.Users.Find(u => u.Username == userUpdateUsernameOrder.Username).Any())
                    return BadRequest(new UserError { Username = new string[] { "user with this username already exists." } });

                user.Username = userUpdateUsernameOrder.Username;
                await _unitOfWork.CompleteAsync();

                return Ok(user);
            }

            return BadRequest(new DetailedResponse { Detail = "Unknown" });
        }
    }
}
