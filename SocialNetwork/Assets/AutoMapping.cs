using AutoMapper;
using SocialNetwork.Models;
using SocialNetwork.Assets.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using SocialNetwork.Assets.Values.Constants;

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
            destination.UserId = _httpContextAccessor.HttpContext.User.FindFirst("userId").Value;
        }
    }

    public class SetImageUrl : IMappingAction<Post, SearchPostDto>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SetImageUrl(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Process(Post source, SearchPostDto destination, ResolutionContext context)
        {
            destination.Image = source.Image.IsNullOrWhitespace() ? "" :  $"{UrlConstants.SERVER_URL}/post-images/{source.Image}";
        }
    }

    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            // User
            CreateMap<User, SimpleUserDto>()
                .ForMember(dto => dto.CreateTime, opt => opt.MapFrom(model => model.CreateTime.ToPersianDateTime()))
                .ForMember(dto => dto.BlockedUntil, opt => opt.MapFrom(model => model.BlockedUntil.HasValue ? model.BlockedUntil.Value.ToPersianDateTime() : ""))
;

            CreateMap<UserCreateOrder, User>()
                .ForMember(model => model.CreateTime, opt => opt.MapFrom(order => DateTime.Now));


            // UserReport
            CreateMap<UserReportCuOrder, UserReport>()
                .ForMember(model => model.CreateTime, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>(); ;

            // Block
            CreateMap<Block, BlockedDto>()
                .ForMember(dto => dto.UserId, opt => opt.MapFrom(model => model.BlockedId))
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.BlockedUser is not null); opt.MapFrom(model => model.BlockedUser.Username); })
                .ForMember(dto => dto.Time, opt => opt.MapFrom(model => model.Time.ToPersianDateTime()))
                .ForMember(dto => dto.Verified, opt => { opt.PreCondition(model => model.BlockedUser is not null); opt.MapFrom(model => model.BlockedUser.Verified); });

            // Relationship
            CreateMap<Relationship, FollowingDto>()
                .ForMember(dto => dto.UserId, opt => opt.MapFrom(model => model.FollowingId))
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.FollowingUser is not null); opt.MapFrom(model => model.FollowingUser.Username); })
                .ForMember(dto => dto.Time, opt => opt.MapFrom(model => model.Time.ToPersianDateTime()))
                .ForMember(dto => dto.Verified, opt => { opt.PreCondition(model => model.FollowingUser is not null); opt.MapFrom(model => model.FollowingUser.Verified); });

            CreateMap<Relationship, FollowerDto>()
                .ForMember(dto => dto.UserId, opt => opt.MapFrom(model => model.UserId))
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.FollowerUser is not null); opt.MapFrom(model => model.FollowerUser.Username); })
                .ForMember(dto => dto.Time, opt => opt.MapFrom(model => model.Time.ToPersianDateTime()))
                .ForMember(dto => dto.Verified, opt => { opt.PreCondition(model => model.FollowerUser is not null); opt.MapFrom(model => model.FollowerUser.Verified); });

            CreateMap<RelationshipTemplate, Relationship>()
                .ForMember(model => model.Time, opt => opt.MapFrom(order => DateTime.Now))
                .AfterMap<SetUserId>();

            // Post
            CreateMap<Post, SearchPostDto>()
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Username); })
                .ForMember(dto => dto.UserVerified, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Verified); })
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(model => model.PostTags.Select(t => t.TagID)))
                .ForMember(dto => dto.CreateTime, opt => opt.MapFrom(model => model.CreateTime.ToPersianDateTime()))
                .ForMember(dto => dto.EditTime, opt => opt.MapFrom(model => model.EditTime.HasValue ? model.EditTime.Value.ToPersianDateTime() : ""))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.PostVotes.Any() ? (model.PostVotes.First().IsDown ? -1 : 1) : 0))
                .AfterMap<SetImageUrl>();

            CreateMap<Post, SinglePostDto>()
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Username); })
                .ForMember(dto => dto.UserVerified, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Verified); })
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(model => model.PostTags.Select(t => t.TagID)))
                .ForMember(dto => dto.CreateTime, opt => opt.MapFrom(model => model.CreateTime.ToPersianDateTime()))
                .ForMember(dto => dto.EditTime, opt => opt.MapFrom(model => model.EditTime.HasValue ? model.EditTime.Value.ToPersianDateTime() : ""))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.PostVotes.Any() ? (model.PostVotes.First().IsDown ? -1 : 1) : 0));

            CreateMap<PostCuOrder, Post>()
                .ForMember(model => model.CreateTime, opt => { opt.PreCondition(order => order.Id == 0); opt.MapFrom(order => DateTime.Now); })
                .ForMember(model => model.EditTime, opt => { opt.PreCondition(order => order.Id != 0); opt.MapFrom(order => DateTime.Now); })
                .ForMember(model => model.PostTags, opt => opt.MapFrom(order => order.Tags.Select(t => new PostTag { TagID = t })))
                .AfterMap<SetUserId>();
            
            // Analysis
            CreateMap<Post, SearchPostWithAnalysisDto>()
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Username); })
                .ForMember(dto => dto.UserVerified, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Verified); })
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(model => model.PostTags.Select(t => t.TagID)))
                .ForMember(dto => dto.CreateTime, opt => opt.MapFrom(model => model.CreateTime.ToPersianDateTime()))
                .ForMember(dto => dto.EditTime, opt => opt.MapFrom(model => model.EditTime.HasValue ? model.EditTime.Value.ToPersianDateTime() : ""))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.PostVotes.Any() ? (model.PostVotes.First().IsDown ? -1 : 1) : 0))
                .ForMember(dto => dto.Time, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.Time); })
                .ForMember(dto => dto.IsShort, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.IsShort); })
                .ForMember(dto => dto.EnterPrice, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.EnterPrice); })
                .ForMember(dto => dto.StopGain, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.StopGain); })
                .ForMember(dto => dto.StopLoss, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.StopLoss); })
                .ForMember(dto => dto.Drawing, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.Drawing); })
                .ForMember(dto => dto.Template, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.Template); })
                .ForMember(dto => dto.ReachedGain, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.ReachedGain); })
                .ForMember(dto => dto.ReachedLoss, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.ReachedLoss); })
                .ForMember(dto => dto.ReachedDate, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.ReachedDate); })
                .AfterMap<SetImageUrl>();

            CreateMap<Post, SinglePostWithAnalysisDto>()
                .ForMember(dto => dto.Username, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Username); })
                .ForMember(dto => dto.UserVerified, opt => { opt.PreCondition(model => model.Author is not null); opt.MapFrom(model => model.Author.Verified); })
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(model => model.PostTags.Select(t => t.TagID)))
                .ForMember(dto => dto.CreateTime, opt => opt.MapFrom(model => model.CreateTime.ToPersianDateTime()))
                .ForMember(dto => dto.EditTime, opt => opt.MapFrom(model => model.EditTime.HasValue ? model.EditTime.Value.ToPersianDateTime() : ""))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.PostVotes.Any() ? (model.PostVotes.First().IsDown ? -1 : 1) : 0))
                .ForMember(dto => dto.Time, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.Time); })
                .ForMember(dto => dto.IsShort, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.IsShort); })
                .ForMember(dto => dto.EnterPrice, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.EnterPrice); })
                .ForMember(dto => dto.StopGain, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.StopGain); })
                .ForMember(dto => dto.StopLoss, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.StopLoss); })
                .ForMember(dto => dto.Drawing, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.Drawing); })
                .ForMember(dto => dto.Template, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.Template); })
                .ForMember(dto => dto.ReachedGain, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.ReachedGain); })
                .ForMember(dto => dto.ReachedLoss, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.ReachedLoss); })
                .ForMember(dto => dto.ReachedDate, opt => { opt.PreCondition(model => model.Analysis is not null); opt.MapFrom(model => model.Analysis.ReachedDate); });

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
                .ForMember(dto => dto.CreateTime, opt => opt.MapFrom(model => model.CreateTime.ToPersianDateTime()))
                .ForMember(dto => dto.EditTime, opt => opt.MapFrom(model => model.EditTime.HasValue ? model.EditTime.Value.ToPersianDateTime() : ""))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.CommentVotes.Any() ? (model.CommentVotes.First().IsDown ? -1 : 1) : 0));

            CreateMap<Comment, SingleCommentDto>()
                .ForMember(dto => dto.CreateTime, opt => opt.MapFrom(model => model.CreateTime.ToPersianDateTime()))
                .ForMember(dto => dto.EditTime, opt => opt.MapFrom(model => model.EditTime.HasValue ? model.EditTime.Value.ToPersianDateTime() : ""))
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
