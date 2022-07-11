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
using System.Net;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("[controller]")]
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

        [HttpPost("follow")]
        public async Task<Relationship> Follow(RelationshipCuOrder relationshipCuOrder)
        {
            if (ModelState.IsValid)
            {
                var relationship = _mapper.Map<Relationship>(relationshipCuOrder);
                var followingUser = await _unitOfWork.Users.GetAsync(relationshipCuOrder.FollowingId);

                if (followingUser is not null)
                {
                    var preRelationship = _unitOfWork.Relationships.Find(r => r.UserId == relationship.UserId && r.FollowingId == relationship.FollowingId).FirstOrDefault();
                    if (preRelationship is null)
                        _unitOfWork.Relationships.Add(relationship, true);

                    return relationship;
                }
            }

            return null;
        }

        [HttpPost("unfollow")]
        public async Task<Relationship> Unfollow(RelationshipCuOrder relationshipCuOrder)
        {
            if (ModelState.IsValid)
            {
                var relationship = _mapper.Map<Relationship>(relationshipCuOrder);
                var preRelationship = _unitOfWork.Relationships.Find(r => r.UserId == relationship.UserId && r.FollowingId == relationship.FollowingId).FirstOrDefault();
                
                if (preRelationship is not null)
                {
                    _unitOfWork.Relationships.Remove(relationship);
                    await _unitOfWork.CompleteAsync();
                    return relationship;
                }
            }

            return null;
        }
    }
}
