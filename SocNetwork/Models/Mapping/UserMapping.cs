using AutoMapper;
using SocNetwork.Models.Db;
using SocNetwork.Models.ViewModel;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<RegisterViewModel, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src =>
                new DateTimeOffset(new DateTime(int.Parse(src.Year), src.Month, src.Date, 0, 0, 0), TimeSpan.Zero))) // UTC
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailReg))
            .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => string.Empty));

        // User → UserViewModel
        CreateMap<User, UserViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src =>
                src.BirthDate.ToLocalTime())) // Для отображения в локальном времени
            .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src =>
                src.BirthDate.ToLocalTime())); // Для отображения в локальном времени

        // UserEditViewModel → User
        CreateMap<UserEditViewModel, User>()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src =>
                src.BirthDate.ToUniversalTime())); // Конвертируем в UTC

        // User → UserEditViewModel
        CreateMap<User, UserEditViewModel>()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src =>
                src.BirthDate.ToLocalTime())) // Для формы в локальном времени
            .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src =>
                src.BirthDate.ToLocalTime())); // Для формы в локальном времени
    }
}