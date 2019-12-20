﻿using AutoMapper;
using SlidFinance.App;
using SlidFinance.App.Utils;
using SlidFinance.Domain;
using SlidFinance.Infrastructure;
using System;
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
				.ForMember(dest => dest.UserDescription,
					opt => opt.MapFrom(src => src.UserDescription ?? ""))
				.ForMember(dest => dest.Category,
					opt => opt.MapFrom(src => src.CategoryId == null ? null : context.Find<Category>(src.CategoryId)))
				.ForMember(dest => dest.Mcc,
					opt => opt.MapFrom(src => src.Mcc == null ? null : context.Mcc.FirstOrDefault(m => m.Code == src.Mcc.Value.ToString("D4"))))
				.ForMember(dest => dest.MccId,
					opt => opt.MapFrom(src => src.Mcc == null ? null : (int?)context.Mcc.First(m => m.Code == src.Mcc.Value.ToString("D4")).Id))
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

			CreateMap<Bank, Dto.Bank>();

			CreateMap<ApplicationUser, Dto.User>();

			CreateMap<Rule, Dto.Rule>()
				.ForMember(dest => dest.CategoryId,
					opt => opt.MapFrom(src => src.Category == null ? null : (int?)src.Category.Id))
				.ForMember(dest => dest.AccountId,
					opt => opt.MapFrom((src => src.Account == null ? null : (int?)src.Account.Id)));

			CreateMap<Category, Dto.Category>();

			CreateMap<Mcc, Dto.Mcc>()
				.ForMember(dest => dest.Category,
				opt => opt.MapFrom(src => src.Category == MccCategory.None ? null :
					new Dto.MccCategory() { Id = (int)src.Category, Title = EnumUtils.GetDescription(src.Category) }));
		}
    }
}
