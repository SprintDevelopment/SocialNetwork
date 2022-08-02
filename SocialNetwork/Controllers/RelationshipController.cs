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
        private readonly IUnitOfWork _unitOfWork;

        public RelationshipController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("follow/{id}")]
        public async Task<IActionResult> Follow(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                if (id == User.FindFirst("userId").Value)
                    return StatusCode(406, new ResponseDto { Result = false, Error = "can not follow yourself." });

                var relationship = _mapper.Map<Relationship>(new RelationshipTemplate { FollowingId = id });
                var followingUser = await _unitOfWork.Users.GetAsync(relationship.FollowingId);

                if (followingUser is not null)
                {
                    var preRelationship = _unitOfWork.Relationships.Find(r => r.UserId == relationship.UserId && r.FollowingId == relationship.FollowingId).FirstOrDefault();
                    if (preRelationship is null)
                        _unitOfWork.Relationships.Add(relationship, true);

                    return Ok(new ResponseDto { Result = true, Message = "relation created." });
                }

                return BadRequest(new ResponseDto { Result = false, Error = $"user with id = {id} not found." });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data." });
        }

        [HttpPost("unfollow/{id}")]
        public async Task<IActionResult> Unfollow(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                var preRelationship = _unitOfWork.Relationships.Find(r => r.UserId == User.FindFirst("userId").Value && r.FollowingId == id).FirstOrDefault();

                if (preRelationship is not null)
                {
                    _unitOfWork.Relationships.Remove(preRelationship);
                    await _unitOfWork.CompleteAsync();

                    return Ok(new ResponseDto { Result = true, Message = "relation removed." });
                }

                return BadRequest(new ResponseDto { Result = false, Error = "no relation found." });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data." });
        }

        // relationship status between current user and user specified by id
        [HttpGet("status/{id}")]
        public IActionResult GetRelationshipStatus(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                return Ok(new RelationshipStatusDto
                {
                    IsFollowing = _unitOfWork.Relationships.Find(r => r.UserId == User.FindFirst("userId").Value && r.FollowingId == id).Any(),
                    IsFollower = _unitOfWork.Relationships.Find(r => r.UserId == id && r.FollowingId == User.FindFirst("userId").Value).Any()
                });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "user id is not valid." });
        }

        [HttpGet("info/{id}")]
        public async Task<IActionResult> GetUserInfo(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                var user = await _unitOfWork.Users.GetAsync(id);

                if (user is not null)
                {
                    return Ok(new RelationshipInfoDto
                    {
                        Following = _unitOfWork.Relationships.Find(r => r.UserId == id).Count(),
                        Follower = _unitOfWork.Relationships.Find(r => r.FollowingId == id).Count(),
                        Verified = user.Verified
                    });
                }

                return BadRequest(new ResponseDto { Result = false, Error = $"user with id = {id} not found." });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "user id is not valid." });
        }

        [HttpGet("follower/{id}")]
        public async Task<IActionResult> GetUserFollowers(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                var user = await _unitOfWork.Users.GetAsync(id);

                if (user is not null)
                {
                    return Ok(_unitOfWork.Relationships.Find(f => f.FollowingId == id).Include(r => r.FollowerUser).Select(r => _mapper.Map<FollowerDto>(r)).AsEnumerable());
                }

                return BadRequest(new ResponseDto { Result = false, Error = $"user with id = {id} not found." });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "user id is not valid." });
        }

        [HttpGet("following/{id}")]
        public async Task<IActionResult> GetUserFollowings(string id)
        {
            if (!id.IsNullOrWhitespace())
            {
                var user = await _unitOfWork.Users.GetAsync(id);

                if (user is not null)
                {
                    return Ok(_unitOfWork.Relationships.Find(f => f.UserId == id).Include(r => r.FollowingUser).Select(r => _mapper.Map<FollowingDto>(r)).AsEnumerable());
                }

                return BadRequest(new ResponseDto { Result = false, Error = $"user with id = {id} not found." });
            }

            return BadRequest(new ResponseDto { Result = false, Error = "user id is not valid." });
        }
    }
}
