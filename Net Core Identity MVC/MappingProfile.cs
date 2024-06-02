using AutoMapper;
using Net_Core_Identity_MVC.Entitys;
using Net_Core_Identity_MVC.Models;

namespace Net_Core_Identity_MVC
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
           CreateMap<UserRegisterModel, User>().ForMember(user => user.UserName,
               opt => opt.MapFrom(model => model.Email));
        }
    }
}
