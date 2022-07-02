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
            CreateMap<Post, SearchPostDto>();
        }
    }
}
