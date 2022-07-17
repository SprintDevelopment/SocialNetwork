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
using System.Net;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("relationship")]
    public class RelationshipController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RelationshipController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RelationshipController(IMapper mapper, ILogger<RelationshipController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<RelationshipDto> Get()
        {
            return _unitOfWork.Relationships
                        .Find(r => r.UserId == User.Identity.Name)
                        .Include(r => r.FollowingUser)
                        .Select(r => _mapper.Map<RelationshipDto>(r))
                        .AsEnumerable();
        }

        [HttpPost("follow/{id}")]
        public async Task<IActionResult> Follow(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                if (id == User.Identity.Name)
                    return StatusCode(406, new ResponseDto { Result = false, Error = "can not follow yourself." });

                var relationship = _mapper.Map<Relationship>(new RelationshipTemplate { FollowingId = id });
                var followingUser = await _unitOfWork.Users.GetAsync(relationship.FollowingId);

                if (followingUser is not null)
                {
                    var preRelationship = _unitOfWork.Relationships.Find(r => r.UserId == relationship.UserId && r.FollowingId == relationship.FollowingId).FirstOrDefault();
                    if (preRelationship is null)
                        _unitOfWork.Relationships.Add(relationship, true);

                    return Ok(new ResponseDto { Result = true, Message = "relation created" });
                }

                return BadRequest(new ResponseDto { Result = false, Error = $"user with id = {id} not found" });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data" });
        }

        [HttpPost("unfollow/{id}")]
        public async Task<IActionResult> Unfollow(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                var relationship = _mapper.Map<Relationship>(new RelationshipTemplate { FollowingId = id });
                var preRelationship = _unitOfWork.Relationships.Find(r => r.UserId == relationship.UserId && r.FollowingId == relationship.FollowingId).FirstOrDefault();
                
                if (preRelationship is not null)
                {
                    _unitOfWork.Relationships.Remove(relationship);
                    await _unitOfWork.CompleteAsync();

                    return Ok(new ResponseDto { Result = true, Message = "relation removed"});
                }

                return BadRequest(new ResponseDto { Result = false, Error = "no relation found" });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data" });
        }

        [HttpPost("status/{id}")]
        public async Task<IActionResult> Status(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                if (id == User.Identity.Name)
                    return StatusCode(406, new ResponseDto { Result = false, Error = "can not follow yourself." });

                var relationship = _mapper.Map<Relationship>(new RelationshipTemplate { FollowingId = id });
                var followingUser = await _unitOfWork.Users.GetAsync(relationship.FollowingId);

                if (followingUser is not null)
                {
                    var preRelationship = _unitOfWork.Relationships.Find(r => r.UserId == relationship.UserId && r.FollowingId == relationship.FollowingId).FirstOrDefault();
                    if (preRelationship is null)
                        _unitOfWork.Relationships.Add(relationship, true);

                    return Ok(new ResponseDto { Result = true, Message = "relation created" });
                }

                return BadRequest(new ResponseDto { Result = false, Error = $"user with id = {id} not found" });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data" });
        }
    }
}
