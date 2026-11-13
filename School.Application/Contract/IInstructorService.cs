using School.Application.Core;
using School.Application.Dtos.Instructor;

namespace School.Application.Contract;

public interface IInstructorService
    : IBaseService<InstructorDto, InstructorCreateDto, InstructorUpdateDto>
{
}
