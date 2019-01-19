using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyFinanceServer.Models;

namespace MyFinanceServer
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Transaction, Api.Dto.Transaction>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.Category == null ? (int?)null : src.Category.Id))
                .ForMember(dest => dest.AccountId, 
                    opt => opt.MapFrom((src => src.Account == null ? (int?)null :src.Account.Id)));
        }
    }
}
