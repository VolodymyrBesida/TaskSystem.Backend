using AutoMapper;
using TaskManagementSystem.Application.DTOs;

using EntityTask = TaskManagementSystem.Domain.Entities.Task;
using EntityTaskStatus = TaskManagementSystem.Domain.Enums.TaskStatus;

namespace TaskManagementSystem.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Domain -> DTO
        CreateMap<EntityTask, TaskDto>();

        // DTO -> Domain
        CreateMap<CreateTaskDto, EntityTask>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EntityTaskStatus.NotStarted));

        CreateMap<TaskStatusDto, EntityTask>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
    }
}