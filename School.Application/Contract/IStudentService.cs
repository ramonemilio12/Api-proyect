using School.Application.Core;
using School.Application.Dtos.Student;

namespace School.Application.Contract;

public interface IStudentService
    : IBaseService<StudentDto, StudentCreateDto, StudentUpdateDto>
{
}
