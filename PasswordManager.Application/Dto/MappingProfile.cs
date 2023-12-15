using AutoMapper;
using PasswordManager.Application.Dto;
using PasswordmanagerApp.Application.Model;

namespace PasswordmanagerApp.Application.Dto

{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PasswordForPostDTO, Password>();
            CreateMap<Password, PasswordForPostDTO>();

            CreateMap<PasswordForPutDTO, Password>();
            CreateMap<Password, PasswordForPutDTO>();
            CreateMap<BankcardDto, Bankcard>();
            CreateMap<Bankcard, BankcardDto>();
            CreateMap<IdcardForPostDTO, Idcard>();
              //  .ForMember(dest => dest.GroupId, opt => opt.Ignore());  
            CreateMap<Idcard, IdcardForPostDTO>();
            CreateMap<IdcardForPutDTO, Idcard>();
            //  .ForMember(dest => dest.GroupId, opt => opt.Ignore());  
            CreateMap<Idcard, IdcardForPutDTO>();
        }
    }
}
