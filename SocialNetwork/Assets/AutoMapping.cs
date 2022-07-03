using AutoMapper;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Assets
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Post, SearchPostDto>()
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(model => model.PostTags.Select(t => t.TagID)))
                .ForMember(dto => dto.MyVote, opt => opt.MapFrom(model => model.PostVotes.Any() ? (model.PostVotes.First().IsDown ? -1 : 1) : 0));
        }
    }
}
