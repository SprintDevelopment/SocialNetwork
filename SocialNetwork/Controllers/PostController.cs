using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using Serilog;
using SocialNetwork.Assets.Dtos;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Assets.Values.Constants;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Models;
using SocialNetwork.Services;
using System;
using System.Collections.Generic;
using System.Data;
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
        [HttpGet("test")]
        public async Task<IActionResult> Get(string username)
        {
            var cs = "Host=localhost;Username=bourse;Password=bourse_pass;Database=bourse_sn";

            using var con = new NpgsqlConnection(cs);
            con.Open();

            var sql = "SELECT * From social_network_user";

            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            int i = 0, rowCount = 0;
            var allRows = new List<User>();
            while (rdr.Read())
            {
                i++;
                rowCount++;
                allRows.Add(new User()
                {
                    Id = rdr.GetString(0),
                    Username = rdr.GetString(1),
                    Reported = rdr.GetBoolean(2),
                    CreateTime = rdr.GetDateTime(3),
                    BlockedUntil = rdr.IsDBNull(4) ? null : rdr.GetDateTime(4),
                    Verified = rdr.GetBoolean(5),
                    ReportCandidate = rdr.GetBoolean(6),
                    WhiteList = rdr.GetBoolean(7),
                    AdminReputation = 1,
                    Avatar = "",
                    Token = ""
                });

                if (i == 1000)
                {
                    _unitOfWork.Users.AddRange(allRows);
                    await _unitOfWork.CompleteAsync();
                    allRows.Clear();
                    i = 0;
                }
            }

            if (i > 0)
            {
                _unitOfWork.Users.AddRange(allRows);
                await _unitOfWork.CompleteAsync();

                i = 0;
            }

            return Ok($"{rowCount} rows inserted in Users table");
        }

        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public IActionResult Get(string user, string tags, int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                                    .Find(p => !p.Reported)
                                    .Include(p => p.Author)
                                    .Include(p => p.PostTags)
                                    .Include(p => p.Analysis);

            if (User.Identity.IsAuthenticated)
            {
                var requestUserId = User.FindFirst("userId").Value;
                //var blockerUserIds = _unitOfWork.Blocks.Find(b => b.BlockedId == requestUserId).Select(bu => bu.UserId);
                //query = query.Where(p => !blockerUserIds.Any(bui => bui == p.UserId));

                query = query.Include(p => p.PostVotes.Where(pv => pv.UserId == requestUserId));
            }

            query = user.IsNullOrWhitespace() ? query : query.Where(p => p.UserId == user); // user
            query = tags.IsNullOrWhitespace() ? query : query.Where(p => p.PostTags.Any(pt => pt.TagID == tags)); // tag

            return Ok(query.OrderByDescending(p => p.CreateTime)
                        .Select(p => _mapper.Map<SearchPostWithAnalysisDto>(p))
                        .Paginate(HttpContext.Request.GetDisplayUrl(), offset, limit));
        }

        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("latests")]
        public IActionResult Latests(int offset, int limit)
        {
            IQueryable<Post> query = _unitOfWork.Posts
                                    .Find(p => !p.Reported)
                                    .Include(p => p.Author)
                                    .Include(p => p.PostTags)
                                    .Include(p => p.Analysis);

            if (User.Identity.IsAuthenticated)
                query = query.Include(p => p.PostVotes.Where(pv => pv.UserId == User.FindFirst("userId").Value));

            return Ok(query.OrderByDescending(p => p.CreateTime)
                        .Select(p => _mapper.Map<SearchPostWithAnalysisDto>(p))
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
        public async Task<IActionResult> Create([FromForm] PostWithAnalysisCreateOrder postCreateOrder)
        {
            Log.Warning(JsonConvert.SerializeObject(postCreateOrder));
            if (ModelState.IsValid)
            {
                if (postCreateOrder.Tags.IsNullOrEmpty())
                {
                    Log.Warning("no tags found in create post");
                    return BadRequest(new ErrorResponse { Error = "no tags" });
                }

                if (postCreateOrder.Tags?.Count() > 2)
                    return BadRequest(new ErrorResponse { Error = "too many tags" });
                else if (postCreateOrder.Tags?.Count() == 2 && !postCreateOrder.Tags.HasAnyItemIn("تحلیل", "آموزش"))
                    return BadRequest(new ErrorResponse { Error = "invalid tags" });

                // UPLOAD IMAGE . . .

                var user = _unitOfWork.Users.Find(u => u.Id == User.FindFirst("userId").Value).FirstOrDefault();
                var post = _mapper.Map<Post>(postCreateOrder);

                if (!postCreateOrder.Time.IsNullOrEmpty())
                {
                    var analysis = new Analysis()
                    {
                        Drawing = postCreateOrder.Drawing,
                        Template = postCreateOrder.Template
                    };

                    if (long.TryParse(postCreateOrder.Time, out long longValue))
                        analysis.Time = longValue;

                    if (double.TryParse(postCreateOrder.EnterPrice, out double doubleValue))
                        analysis.EnterPrice = doubleValue;
                    if (double.TryParse(postCreateOrder.StopGain, out doubleValue))
                        analysis.StopGain = doubleValue;
                    if (double.TryParse(postCreateOrder.StopLoss, out doubleValue))
                        analysis.StopLoss = doubleValue;

                    if (bool.TryParse(postCreateOrder.IsShort, out bool boolValue))
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

                post.Symbol = postCreateOrder.Tags.FirstOrDefault(t => !t.IsIn("تحلیل", "آموزش")) ?? "TEMP";

                Log.Warning(JsonConvert.SerializeObject(post));
                if (!user.Verified && !user.WhiteList)
                {
                    var validateReulst = _unitOfWork.BlackListPatterns.ValidateMessage(postCreateOrder.Text);
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
        public async Task<IActionResult> Update(int id, [FromForm] PostUpdateOrder postUpdateOrder)
        {
            Log.Warning(JsonConvert.SerializeObject(postUpdateOrder));
            if (ModelState.IsValid)
            {
                var post = _unitOfWork.Posts.Find(p => p.Id == id).FirstOrDefault();

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                    post.Image = await _fileService.UploadAsync(files[0], "post-images", post.Image);

                if (post is not null)
                {
                    if (post.UserId != User.FindFirst("userId").Value)
                        return BadRequest(new ErrorResponse { Error = "ownership error" });

                    _mapper.Map(postUpdateOrder, post);

                    await _unitOfWork.CompleteAsync();

                    return Ok(_mapper.Map<SinglePostDto>(post));
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