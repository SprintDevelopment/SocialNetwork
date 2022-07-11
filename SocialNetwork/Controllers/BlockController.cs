using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public IEnumerable<BlockDto> Get()
        {
            return _unitOfWork.Blocks
                        .Find(r => r.UserId == User.Identity.Name)
                        .Include(r => r.BlockedUser)
                        .Select(r => _mapper.Map<BlockDto>(r))
                        .AsEnumerable();
        }

        [HttpPost("block")]
        public async Task<Block> Block(BlockCuOrder blockCuOrder)
        {
            if (ModelState.IsValid)
            {
                var block = _mapper.Map<Block>(blockCuOrder);
                var blockedUser = await _unitOfWork.Users.GetAsync(blockCuOrder.BlockedId);

                if (blockedUser is not null)
                {
                    var preBlock = _unitOfWork.Blocks.Find(r => r.UserId == block.UserId && r.BlockedId == blockCuOrder.BlockedId).FirstOrDefault();
                    if (preBlock is null)
                        _unitOfWork.Blocks.Add(block, true);

                    return block;
                }
            }

            return null;
        }

        [HttpPost("unblock")]
        public async Task<Block> Unblock(BlockCuOrder blockCuOrder)
        {
            if (ModelState.IsValid)
            {
                var block = _mapper.Map<Block>(blockCuOrder);
                var blockedUser = await _unitOfWork.Users.GetAsync(blockCuOrder.BlockedId);

                if (blockedUser is not null)
                {
                    var preBlock = _unitOfWork.Blocks.Find(r => r.UserId == block.UserId && r.BlockedId == blockCuOrder.BlockedId).FirstOrDefault();
                    if (preBlock is null)
                    {
                        _unitOfWork.Blocks.Remove(block);
                        await _unitOfWork.CompleteAsync();
                        return block;
                    }
                }
            }

            return null;
        }
    }
}
