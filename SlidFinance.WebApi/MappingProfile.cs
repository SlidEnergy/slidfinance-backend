﻿using AutoMapper;
using SaltEdgeNetCore.Models.Transaction;
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
					opt => opt.MapFrom(src => src.Account.Id))
				.ForMember(dest => dest.Mcc,
					opt => opt.MapFrom(src => src.Mcc == null ? null : (int?)Convert.ToInt32(src.Mcc.Code)));

			CreateMap<Dto.Transaction, Transaction>()
				.ForMember(dest => dest.BankCategory,
					opt => opt.MapFrom(src => src.BankCategory ?? ""))
				.ForMember(dest => dest.Description,
					opt => opt.MapFrom(src => src.Description ?? ""))
				.ForMember(dest => dest.UserDescription,
					opt => opt.MapFrom(src => src.UserDescription ?? ""))
				.ForMember(dest => dest.Category,
					opt => opt.MapFrom(src => src.CategoryId == null ? null : context.Find<UserCategory>(src.CategoryId)))
				.ForMember(dest => dest.Mcc,
					opt => opt.MapFrom(src => src.Mcc == null ? null : context.Mcc.FirstOrDefault(m => m.Code == src.Mcc.Value.ToString("D4"))))
				.ForMember(dest => dest.MccId,
					opt => opt.MapFrom(src => src.Mcc == null ? null : (int?)context.Mcc.First(m => m.Code == src.Mcc.Value.ToString("D4")).Id))
				.ForMember(dest => dest.Account,
					opt => opt.MapFrom(src => context.Find<BankAccount>(src.AccountId)));

			CreateMap<Dto.ImportTransaction, Transaction>()
				.ForMember(dest => dest.Id,
					opt => opt.Ignore())
				.ForMember(dest => dest.Approved,
					opt => opt.Ignore())
				.ForMember(dest => dest.BankCategory,
					opt => opt.MapFrom(src => src.Category ?? ""))
				.ForMember(dest => dest.UserDescription,
					opt => opt.Ignore())
				.ForMember(dest => dest.Category,
					opt => opt.Ignore())
				.ForMember(dest => dest.CategoryId,
					opt => opt.Ignore())
				.ForMember(dest => dest.Mcc,
					opt => opt.MapFrom(src => src.Mcc == null ? null : context.Mcc.FirstOrDefault(m => m.Code == src.Mcc.Value.ToString("D4"))))
				.ForMember(dest => dest.MccId,
					opt => opt.MapFrom(src => src.Mcc == null ? null : (int?)context.Mcc.First(m => m.Code == src.Mcc.Value.ToString("D4")).Id))
				.ForMember(dest => dest.Account,
					opt => opt.Ignore())
				.ForMember(dest => dest.AccountId,
					opt => opt.Ignore());

			CreateMap<BankAccount, Dto.BankAccount>();

			CreateMap<Dto.BankAccount, BankAccount>()
			   .ForMember(dest => dest.Bank,
				   opt => opt.MapFrom(src => context.Find<Bank>(src.BankId)))
			   .ForMember(dest => dest.Product,
					opt => opt.Ignore())
			   .ForMember(dest => dest.SelectedTariff,
					opt => opt.Ignore());

			CreateMap<ApplicationUser, Dto.User>()
				.ForMember(dest => dest.IsAdmin,
					opt => opt.Ignore());

			CreateMap<Rule, Dto.Rule>()
				.ForMember(dest => dest.CategoryId,
					opt => opt.MapFrom(src => src.Category == null ? null : (int?)src.Category.Id))
				.ForMember(dest => dest.AccountId,
					opt => opt.MapFrom((src => src.Account == null ? null : (int?)src.Account.Id)));

			CreateMap<UserCategory, Dto.Category>();

			CreateMap<Mcc, Dto.Mcc>()
				.ForMember(dest => dest.Category,
					opt => opt.MapFrom(src => src.Category == MccCategory.None ? null :
						new Dto.MccCategory() { Id = (int)src.Category, Title = EnumUtils.GetDescription(src.Category) }));
		}
	}
}
