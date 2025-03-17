using AutoMapper;
using TaskListService.Application.Services.Commands;
using TaskListService.Application.Services.Vm;
using TaskService.Domain.Entities;

namespace TaskListService.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskList, TaskListItemVm>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
            .ForMember(dest => dest.SharedUsersIdList, opt => opt.MapFrom(src => src.SharedWith))
            .ReverseMap();
        
        CreateMap<TaskList,ShortTaskListItemVm>();
        CreateMap<CreateTaskListCommand, TaskList>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskListId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.SharedWith, opt => opt.MapFrom(_ => new List<string>()))
            .ReverseMap();
    }
}