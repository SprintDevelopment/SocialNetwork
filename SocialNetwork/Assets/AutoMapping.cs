using AutoMapper;
using SocialNetwork.Models;
using SocialNetwork.Assets.Extensions;
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
            // User
            CreateMap<User, SimpleUserDto>();

            CreateMap<UserCreateOrder, User>()
                .ForMember(model => model.CreateTime, opt => { opt.PreCondition(order => order.Id.IsNullOrWhitespace()); opt.MapFrom(order => DateTime.Now); });


            // UserReport
            CreateMap<UserReportCuOrder, UserReport>()
                .ForMember(model => model.CreateTime, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>(); ;

            // Block
            CreateMap<BlockCuOrder, Block>()
                .ForMember(model => model.Time, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>(); ;

            // Relationship
            CreateMap<Relationship, RelationshipDto>();

            CreateMap<RelationshipTemplate, Relationship>()
                .ForMember(model => model.Time, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>();

            // Post
            CreateMap<Post, SearchPostDto>()
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Username); })
                .ForMember(dto => dto.UserVerified, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Verified); })
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(model => model.PostTags.Select(t => t.TagID)))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.PostVotes.Any() ? (model.PostVotes.First().IsDown ? -1 : 1) : 0));

            CreateMap<Post, SinglePostDto>()
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Username); })
                .ForMember(dto => dto.UserVerified, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Verified); })
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
                .AfterMap<SetUserId>();

            // PostReport
            CreateMap<PostReportCuOrder, PostReport>()
                .ForMember(model => model.CreateTime, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>(); ;

            // Comment
            CreateMap<Comment, SearchCommentDto>()
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Username); })
                .ForMember(dto => dto.UserVerified, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Verified); })
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
                .AfterMap<SetUserId>();

            // CommentReport
            CreateMap<CommentReportCuOrder, CommentReport>()
                .ForMember(model => model.CreateTime, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>(); ;
        }
    }
}
