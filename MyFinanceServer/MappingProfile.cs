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

            CreateMap<Models.Account, Api.Dto.Account>()
                .ForMember(dest => dest.BankId,
                    opt => opt.MapFrom(src => src.Bank == null ? (int?) null : src.Bank.Id))
                .ForMember(dest => dest.TransactionIds,
                    opt => opt.MapFrom(src => src.Transactions == null ? new int[0] : src.Transactions.Select(x=>x.Id)));

            CreateMap<Models.Bank, Api.Dto.Bank>()
                .ForMember(dest => dest.AccountIds,
                    opt => opt.MapFrom(src => src.Accounts == null ? new int[0] : src.Accounts.Select(x => x.Id)));

            CreateMap<Models.User, Api.Dto.User>()
                .ForMember(dest => dest.BankIds,
                    opt => opt.MapFrom(src => src.Banks == null ? new int[0] : src.Banks.Select(x => x.Id)))
                .ForMember(dest => dest.CategoryIds,
                    opt => opt.MapFrom(src => src.Categories == null ? new int[0] : src.Categories.Select(x => x.Id)));

            CreateMap<Models.Category, Api.Dto.Category>();
        }
    }
}
