﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Assets.Dtos;
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
    public class BlackListPatternController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BlackListPatternController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IActionResult Create([FromForm]BlackListPattern blackListPattern)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.BlackListPatterns.Add(blackListPattern, true);

                return Ok(blackListPattern);
            }

            return BadRequest(new ResponseDto { Result = false, Error = "not enough input data" });
        }
    }
}
