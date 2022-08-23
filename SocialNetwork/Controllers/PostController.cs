using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Assets.Values.Constants;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using SocialNetwork.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IHttpClientFactory _httpClientFactory;

        public PostController(IMapper mapper, IUnitOfWork unitOfWork, IFileService fileService, IHttpClientFactory httpClientFactory)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public IActionResult Get(string user, string tags, int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                                    .Find()
                                    .Include(p => p.Author)
                                    .Include(p => p.PostTags)
                                    .Include(p => p.Analysis);

            if (User.Identity.IsAuthenticated)
                query = query.Include(p => p.PostVotes.Where(pv => pv.UserId == User.FindFirst("userId").Value));

            query = user.IsNullOrWhitespace() ? query : query.Where(p => p.UserId == user); // user
            query = tags.IsNullOrWhitespace() ? query : query.Where(p => p.PostTags.Any(pt => pt.TagID == tags)); // tag

            return Ok(query.OrderByDescending(p => p.CreateTime)
                        .Select(p => _mapper.Map<SearchPostWithAnalysisDto>(p))
                        .AsEnumerable()
                        .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("latests")]
        public IActionResult Latests(int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                                    .Find()
                                    .Include(p => p.Author)
                                    .Include(p => p.PostTags)
                                    .Include(p => p.Analysis);

            if (User.Identity.IsAuthenticated)
                query = query.Include(p => p.PostVotes.Where(pv => pv.UserId == User.FindFirst("userId").Value));

            return Ok(query.OrderByDescending(p => p.CreateTime)
                        .Select(p => _mapper.Map<SearchPostWithAnalysisDto>(p))
                        .AsEnumerable()
                        .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [HttpGet("mine")]
        public IActionResult MyPosts(int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                                    .Find(p => p.UserId == User.FindFirst("userId").Value)
                                    .Include(p => p.PostTags)
                                    .Include(p => p.PostVotes.Where(pp => pp.UserId == User.FindFirst("userId").Value))
                                    .Include(p => p.Analysis);


            return Ok(query.OrderByDescending(p => p.CreateTime)
                        .Select(p => _mapper.Map<SearchPostWithAnalysisDto>(p))
                        .AsEnumerable()
                        .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{post_id}")]
        public IActionResult Get(int post_id)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                    .Find(p => p.Id == post_id)
                    .Include(p => p.Author)
                    .Include(p => p.PostTags)
                    .Include(p => p.Analysis);


            if (User.Identity.IsAuthenticated)
                query = query.Include(p => p.PostVotes.Where(pv => pv.UserId == User.FindFirst("userId").Value));

            var singlePost = query
                    .Select(p => _mapper.Map<SinglePostWithAnalysisDto>(p))
                    .FirstOrDefault();

            if (singlePost is null)
                return NotFound();

            return Ok(singlePost);
        }

        [HttpGet("Image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var image = _fileService.DownloadAsync(Path.Combine("post-images", fileName));
            return File(image, "image/jpeg");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PostWithAnalysisCuOrder postCuOrder)
        {
            Log.Warning(JsonConvert.SerializeObject(postCuOrder));
            if (ModelState.IsValid)
            {
                if (postCuOrder.Tags.IsNullOrEmpty())
                {
                    Log.Warning("no tags found in create post");
                    return BadRequest(new ErrorResponse { Error = "no tags" });
                }

                if (postCuOrder.Tags?.Count() > 2)
                    return BadRequest(new ErrorResponse { Error = "too many tags" });
                else if (postCuOrder.Tags?.Count() == 2 && !postCuOrder.Tags.HasAnyItemIn("تحلیل", "آموزش"))
                    return BadRequest(new ErrorResponse { Error = "invalid tags" });

                // UPLOAD IMAGE . . .

                var user = _unitOfWork.Users.Find(u => u.Id == User.FindFirst("userId").Value).FirstOrDefault();
                var post = _mapper.Map<Post>(postCuOrder);

                if (!postCuOrder.Time.IsNullOrEmpty())
                {
                    var analysis = new Analysis()
                    {
                        Drawing = postCuOrder.Drawing,
                        Template = postCuOrder.Template
                    };

                    if (long.TryParse(postCuOrder.Time, out long longValue))
                        analysis.Time = longValue;

                    if (double.TryParse(postCuOrder.EnterPrice, out double doubleValue))
                        analysis.EnterPrice = doubleValue;
                    if (double.TryParse(postCuOrder.StopGain, out doubleValue))
                        analysis.StopGain = doubleValue;
                    if (double.TryParse(postCuOrder.StopLoss, out doubleValue))
                        analysis.StopLoss = doubleValue;

                    if (bool.TryParse(postCuOrder.IsShort, out bool boolValue))
                        analysis.IsShort = boolValue;

                    if (0d.IsIn(analysis.EnterPrice, analysis.StopGain, analysis.StopLoss) || analysis.Time <= 0)
                        Log.Warning("No enough data for signal");
                    else
                    {
                        _unitOfWork.Analyses.Add(analysis, true);
                        post.AnalysisId = analysis.Id;
                    }

                }

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                    post.Image = await _fileService.UploadAsync(files[0], "post-images", "");

                post.Symbol = postCuOrder.Tags.FirstOrDefault(t => !t.IsIn("تحلیل", "آموزش")) ?? "TEMP";

                Log.Warning(JsonConvert.SerializeObject(post));
                if (!user.Verified && !user.WhiteList)
                {
                    var validateReulst = _unitOfWork.BlackListPatterns.ValidateMessage(postCuOrder.Text);
                    if (!validateReulst.IsNullOrWhitespace())
                    {
                        post.AutoReport = true;
                        post.AutoReportTime = DateTime.Now;
                        post.Description = validateReulst;

                        // CHECK FOR REPORT USER . . .
                    }
                }

                _unitOfWork.Posts.Add(post, true);
                _unitOfWork.PostTags.AddRange(post.PostTags.Select(pt => new PostTag { PostID = post.Id, TagID = pt.TagID }));

                await _unitOfWork.CompleteAsync();

                {
                    StringContent notification = new StringContent(JsonConvert.SerializeObject(
                            new PersonalPostNotificationOrder
                            {
                                Id = post.Id,
                                Text = post.Text,
                                UserId = user.Id,
                                Username = user.Username,
                            }),
                            Encoding.UTF8,
                            Application.Json);

                    var httpClient = _httpClientFactory.CreateClient(NamedClientsConstants.NOTIFICATION_CLIENT);

                    using var httpResponseMessage =
                        await httpClient.PostAsync("personal", notification);

                    httpResponseMessage.EnsureSuccessStatusCode();
                }

                return Ok(_mapper.Map<SinglePostDto>(post));
            }

            return BadRequest();
        }

        // posts/ -> {user, tag, limit, offset, order, date} - query
        // posts/{post_id} -> { post_id }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PostCuOrder postCuOrder)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.Users.Find(u => u.Id == User.FindFirst("userId").Value).FirstOrDefault();
                var post = _unitOfWork.Posts.Find(p => p.Id == id).FirstOrDefault();

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                    post.Image = await _fileService.UploadAsync(files[0], "post-images", post.Image);


                if (post is not null)
                {
                    if (postCuOrder.Tags?.Count() > 2)
                        return BadRequest(new ErrorResponse { Error = "too many tags" });

                    // UPLOAD IMAGE . . .

                    _mapper.Map(postCuOrder, post);

                    if (!user.Verified && !user.WhiteList)
                    {
                        var validateReulst = _unitOfWork.BlackListPatterns.ValidateMessage(postCuOrder.Text);
                        if (!validateReulst.IsNullOrWhitespace())
                        {
                            post.AutoReport = true;
                            post.AutoReportTime = DateTime.Now;
                            post.Description = validateReulst;

                            // CHECK FOR REPORT USER . . .
                        }
                    }

                    //_unitOfWork.PostTags.AddRange(post.PostTags.Select(pt => new PostTag { PostID = post.Id, TagID = pt.TagID }));

                    await _unitOfWork.CompleteAsync();

                    return Ok(post);
                }

                return NotFound(new ResponseDto { Result = false, Error = "post not found." });
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPatch("signal/{id}")]
        public async Task<IActionResult> UpdateAnalysis(int id, [FromForm] UpdateAnalysisOrder updateAnalysisOrder)
        {
            Log.Error(JsonConvert.SerializeObject(updateAnalysisOrder));
            if (ModelState.IsValid)
            {
                var post = _unitOfWork.Posts.Find(p => p.Id == id).Include(p => p.Analysis).FirstOrDefault();

                if (post is not null && post.Analysis is not null)
                {
                    post.Analysis.ReachedGain = updateAnalysisOrder.ReachedGain;
                    post.Analysis.ReachedLoss = updateAnalysisOrder.ReachedLoss;
                    post.Analysis.ReachedDate = updateAnalysisOrder.ReachedDate;

                    await _unitOfWork.CompleteAsync();

                    return Ok(post.Analysis);
                }

                return NotFound(new ResponseDto { Result = false, Error = post is null ? "post not found." : "post has no signal." });
            }

            return BadRequest();
        }
    }
}