﻿using AutoMapper;
using SlidFinance.App;
using SlidFinance.Domain;
using SlidFinance.Infrastucture;
using System.Linq;

namespace SlidFinance.WebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile(ApplicationDbContext context)
        {
            CreateMap<RegisterBindingModel, ApplicationUser>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.Email,
					opt => opt.MapFrom(src => src.Email))
				.ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Transaction, Dto.Transaction>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.Category == null ? null : (int?)src.Category.Id))
                .ForMember(dest => dest.AccountId,
                    opt => opt.MapFrom(src => src.Account.Id));

            CreateMap<Dto.Transaction, Transaction>()
                .ForMember(dest => dest.BankCategory,
                    opt => opt.MapFrom(src => src.BankCategory ?? ""))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.CategoryId == null ? null : context.Find<Category>(src.CategoryId)))
                .ForMember(dest => dest.Account,
                    opt => opt.MapFrom(src => context.Find<BankAccount>(src.AccountId)));

            CreateMap<BankAccount, Dto.BankAccount>()
                .ForMember(dest => dest.BankId,
                    opt => opt.MapFrom(src => src.Bank.Id));

            CreateMap<Dto.BankAccount, BankAccount>()
               .ForMember(dest => dest.Bank,
                   opt => opt.MapFrom(src => context.Find<Bank>(src.BankId)))
               .ForMember(dest => dest.Transactions, 
                   opt => opt.Ignore());

            CreateMap<Bank, Dto.Bank>()
                .ForMember(dest => dest.AccountIds,
                    opt => opt.MapFrom(src => src.Accounts.Select(x => x.Id)));

            CreateMap<ApplicationUser, Dto.User>()
                .ForMember(dest => dest.BankIds,
                    opt => opt.MapFrom(src => src.Banks.Select(x => x.Id)))
                .ForMember(dest => dest.CategoryIds,
                    opt => opt.MapFrom(src => src.Categories.Select(x => x.Id)));

            CreateMap<Rule, Dto.Rule>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.Category == null ? null : (int?)src.Category.Id))
                .ForMember(dest => dest.AccountId,
                    opt => opt.MapFrom((src => src.Account == null ? null : (int?)src.Account.Id)));

            CreateMap<Category, Dto.Category>();
        }
    }
}