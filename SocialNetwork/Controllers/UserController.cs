using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Assets.Values.Constants;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using SocialNetwork.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IFileService _fileService;

        public UserController(IMapper mapper, IUserService userService, IUnitOfWork unitOfWork, IFileService fileService)
        {
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
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

                    if (user.Avatar.IsNullOrWhitespace())
                        user.Avatar = $"{UrlConstants.SERVER_URL}/user-avatars/{user.Avatar}";

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
            {
                user.Avatar = user.Avatar.IsNullOrWhitespace() ? "" : $"{UrlConstants.SERVER_URL}/user-avatars/{user.Avatar}";

                return Ok(user);
            }

            return NotFound();
        }

        [AllowAnonymous]
        [HttpGet("/search/users")]
        public IActionResult Search(string username, int offset, int limit)
        {
            if (username.IsNullOrWhitespace() || username.Length < 3)
                return BadRequest(new ResponseDto { Result = false, Error = "not enough input data" });

            return Ok(_unitOfWork.Users.Find(u => u.Username.ToLower().Contains(username.ToLower()))
                .Select(u => _mapper.Map<SimpleUserDto>(u))
                .AsEnumerable()
                .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [HttpGet("Avatar/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var image = _fileService.DownloadAsync(Path.Combine("post-images", fileName));
            return File(image, "image/jpeg");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserCreateOrder userCuOrder)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(userCuOrder);

                if (_unitOfWork.Users.Find(u => u.Id == user.Id).Any())
                    return BadRequest(new UserError { ID = "user with this id already exists." });

                if (_unitOfWork.Users.Find(u => u.Username == user.Username).Any())
                    return BadRequest(new UserError { Username = new string[] { "user with this username already exists." } });

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                    user.Avatar = await _fileService.UploadAsync(files[0], "user-avatars", "");

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

                if (user is null)
                    return NotFound(new UserError { Username = new string[] { "user with this user id is not found!." } });

                if (userUpdateUsernameOrder.Username == user.Username)
                    return BadRequest(new UserError { Username = new string[] { "username is the same as previous." } });

                if (_unitOfWork.Users.Find(u => u.Username == userUpdateUsernameOrder.Username).Any())
                    return BadRequest(new UserError { Username = new string[] { "user with this username already exists." } });

                user.Username = userUpdateUsernameOrder.Username;
                await _unitOfWork.CompleteAsync();

                if (user.Avatar.IsNullOrWhitespace())
                    user.Avatar = $"{UrlConstants.SERVER_URL}/user-avatars/{user.Avatar}";

                return Ok(user);
            }

            return BadRequest(new DetailedResponse { Detail = "Unknown" });
        }

        [HttpPatch("Avatar/{id}")]
        public async Task<IActionResult> UpdateAvatar(string id, [FromForm] FakeFormForUploadAvatar fakeFormForUploadAvatar)
        {
            var files = HttpContext.Request.Form.Files;
            if (!id.IsNullOrEmpty() && files.Count > 0)
            {

                var user = _unitOfWork.Users.Find(u => u.Id == id).FirstOrDefault();

                if (user is null)
                    return NotFound(new UserError { Username = new string[] { "user with this user id is not found!." } });

                user.Avatar = await _fileService.UploadAsync(files[0], "user-avatars", user.Avatar);

                await _unitOfWork.CompleteAsync();

                if (user.Avatar.IsNullOrWhitespace())
                    user.Avatar = $"{UrlConstants.SERVER_URL}/user-avatars/{user.Avatar}";

                return Ok(user);
            }

            return BadRequest(new DetailedResponse { Detail = "Unknown" });
        }
    }
}
