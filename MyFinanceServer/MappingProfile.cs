using AutoMapper;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using System.Linq;

namespace MyFinanceServer.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile(ApplicationDbContext context)
        {
            CreateMap<RegisterBindingModel, ApplicationUser>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.Email))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Transaction, Api.Dto.Transaction>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.Category == null ? null : (int?)src.Category.Id))
                .ForMember(dest => dest.AccountId,
                    opt => opt.MapFrom(src => src.Account.Id));

            CreateMap<Api.Dto.Transaction, Transaction>()
                .ForMember(dest => dest.BankCategory,
                    opt => opt.MapFrom(src => src.BankCategory ?? ""))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.CategoryId == null ? null : context.Find<Category>(src.CategoryId)))
                .ForMember(dest => dest.Account,
                    opt => opt.MapFrom(src => context.Find<BankAccount>(src.AccountId)));

            CreateMap<BankAccount, Api.Dto.BankAccount>()
                .ForMember(dest => dest.BankId,
                    opt => opt.MapFrom(src => src.Bank.Id));

            CreateMap<Bank, Api.Dto.Bank>()
                .ForMember(dest => dest.AccountIds,
                    opt => opt.MapFrom(src => src.Accounts.Select(x => x.Id)));

            CreateMap<ApplicationUser, Api.Dto.User>()
                .ForMember(dest => dest.BankIds,
                    opt => opt.MapFrom(src => src.Banks.Select(x => x.Id)))
                .ForMember(dest => dest.CategoryIds,
                    opt => opt.MapFrom(src => src.Categories.Select(x => x.Id)));

            CreateMap<Rule, Api.Dto.Rule>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.Category == null ? null : (int?)src.Category.Id))
                .ForMember(dest => dest.AccountId,
                    opt => opt.MapFrom((src => src.Account == null ? null : (int?)src.Account.Id)));

            CreateMap<Category, Api.Dto.Category>();
        }
    }
}
