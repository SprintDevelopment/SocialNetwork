using AutoMapper;
using SocialNetwork.Models;
using mvc = Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace SocialNetwork.Assets
{
    public class SetUserId : IMappingAction<ShouldPassUserId, HasUserId>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SetUserId(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Process(ShouldPassUserId source, HasUserId destination, ResolutionContext context)
        {
            destination.UserId = _httpContextAccessor.HttpContext.User.Identity.Name;
        }
    }

    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<User, SimpleUserDto>();

            // Post
            CreateMap<Post, SearchPostDto>()
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(model => model.PostTags.Select(t => t.TagID)))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.PostVotes.Any() ? (model.PostVotes.First().IsDown ? -1 : 1) : 0));

            CreateMap<Post, SinglePostDto>()
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(model => model.PostTags.Select(t => t.TagID)))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.PostVotes.Any() ? (model.PostVotes.First().IsDown ? -1 : 1) : 0));

            CreateMap<PostCuOrder, Post>()
                .ForMember(model => model.CreateTime, opt => { opt.PreCondition(order => order.Id == 0); opt.MapFrom(order => DateTime.Now); })
                .ForMember(model => model.EditTime, opt => { opt.PreCondition(order => order.Id != 0); opt.MapFrom(order => DateTime.Now); })
                .ForMember(model => model.PostTags, opt => opt.MapFrom(order => order.Tags.Select(t => new PostTag { TagID = t })))
                .AfterMap<SetUserId>();

            // PostVote
            CreateMap<PostVoteCuOrder, PostVote>()
                .ForMember(model => model.CreateTime, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>(); ;

            // PostVote
            CreateMap<PostReportCuOrder, PostReport>()
                .ForMember(model => model.CreateTime, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>(); ;

            // Comment
            CreateMap<Comment, SearchCommentDto>()
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.CommentVotes.Any() ? (model.CommentVotes.First().IsDown ? -1 : 1) : 0));

            CreateMap<Comment, SingleCommentDto>()
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.CommentVotes.Any() ? (model.CommentVotes.First().IsDown ? -1 : 1) : 0));

            CreateMap<CommentCuOrder, Comment>()
                .ForMember(model => model.CreateTime, opt => { opt.PreCondition(order => order.Id == 0); opt.MapFrom(order => DateTime.Now); })
                .ForMember(model => model.EditTime, opt => { opt.PreCondition(order => order.Id != 0); opt.MapFrom(order => DateTime.Now); })
                .AfterMap<SetUserId>();

            // CommentVote
            CreateMap<CommentVoteCuOrder, CommentVote>()
                .ForMember(model => model.CreateTime, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>(); ;
        }
    }
}
