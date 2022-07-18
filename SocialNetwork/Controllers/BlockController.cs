using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Route("relationship")]
    public class BlockController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<BlockController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public BlockController(IMapper mapper, ILogger<BlockController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("block/{id}")]
        public async Task<IActionResult> Block(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                if (id == User.Identity.Name)
                    return StatusCode(406, new ResponseDto { Result = false, Error = "can not block yourself." });

                var block = _mapper.Map<Block>(id);
                var blockedUser = await _unitOfWork.Users.GetAsync(id);

                if (blockedUser is not null)
                {
                    var preBlock = _unitOfWork.Blocks.Find(r => r.UserId == block.UserId && r.BlockedId == id).FirstOrDefault();
                    if (preBlock is null)
                        _unitOfWork.Blocks.Add(block, true);

                    return Ok(new ResponseDto { Result = true, Message = "user blocked." });
                }

                return BadRequest(new ResponseDto { Result = false, Error = $"user with id = {id} not found." });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data." });
        }

        [HttpPost("unblock/{id}")]
        public async Task<IActionResult> Unblock(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                var preBlock = _unitOfWork.Blocks.Find(r => r.UserId == User.Identity.Name && r.BlockedId == id).FirstOrDefault();

                if (preBlock is not null)
                {
                    _unitOfWork.Blocks.Remove(preBlock);
                    await _unitOfWork.CompleteAsync();

                    return Ok(new ResponseDto { Result = true, Message = "user unblocked." });
                }

                return BadRequest(new ResponseDto { Result = false, Error = "no block found." });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data." });
        }

        [HttpGet("blocklist")]
        public IActionResult GetUserBlockList()
        {
            return Ok(_unitOfWork.Blocks.Find(f => f.UserId == User.Identity.Name).Include(r => r.BlockedUser).Select(r => _mapper.Map<FollowingDto>(r)).AsEnumerable());
        }
    }
}
