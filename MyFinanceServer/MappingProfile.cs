using AutoMapper;
using MyFinanceServer.Data;
using System.Linq;

namespace MyFinanceServer.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, Api.Dto.Transaction>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.Category == null ? null : src.Category.Id))
                .ForMember(dest => dest.AccountId, 
                    opt => opt.MapFrom((src => src.Account == null ? null :src.Account.Id)));

            CreateMap<BankAccount, Api.Dto.BankAccount>()
                .ForMember(dest => dest.BankId,
                    opt => opt.MapFrom(src => src.Bank == null ?  null : src.Bank.Id))
                .ForMember(dest => dest.TransactionIds,
                    opt => opt.MapFrom(src => src.Transactions == null ? new string[0] : src.Transactions.Select(x=>x.Id)));

            CreateMap<Bank, Api.Dto.Bank>()
                .ForMember(dest => dest.AccountIds,
                    opt => opt.MapFrom(src => src.Accounts == null ? new string[0] : src.Accounts.Select(x => x.Id)))
                .ForMember(dest => dest.Balance,
                    opt => opt.MapFrom(src => src.Accounts.Sum(x => x.Balance)));

            CreateMap<ApplicationUser, Api.Dto.User>()
                .ForMember(dest => dest.BankIds,
                    opt => opt.MapFrom(src => src.Banks == null ? new string[0] : src.Banks.Select(x => x.Id)))
                .ForMember(dest => dest.CategoryIds,
                    opt => opt.MapFrom(src => src.Categories == null ? new string[0] : src.Categories.Select(x => x.Id)));

            CreateMap<Rule, Api.Dto.Rule>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.Category == null ? null : src.Category.Id))
                .ForMember(dest => dest.AccountId,
                    opt => opt.MapFrom((src => src.Account == null ? null : src.Account.Id)));

            CreateMap<Category, Api.Dto.Category>();
        }
    }
}
