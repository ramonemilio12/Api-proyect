# School API

API REST para administrar departamentos, cursos, estudiantes e instructores.
La solución está separada en cuatro proyectos y sigue una arquitectura por capas
(Dominio, Infraestructura, Aplicación y API).

## Estructura

```
School.Domain/
  Core/          BaseEntity.cs, Person.cs
  Entities/      Course.cs, Department.cs, Instructor.cs, Student.cs
  Repository/    IRepository.cs          (contrato genérico)

School.Infrastructure/
  Context/       SchoolContext.cs
  Core/          BaseRepository.cs, Repository.cs
  Exceptions/    CourseException.cs, DepartmentException.cs
  Interfaces/    ICourseRepository.cs, IDepartmentRepository.cs
  Repositories/  CourseRepository.cs, DepartmentRepository.cs

School.Application/
  Contract/      IDepartamentService.cs, ICourseService.cs,
                 IStudentService.cs, IInstructorService.cs
  Core/          IBaseService.cs, BaseService.cs, ServiceResult.cs
  Dtos/          DtoBase.cs
    Course/      CourseDto.cs, CourseCreateDto.cs, CourseUpdateDto.cs
    Department/  DepartmentDto.cs, DepartmentCreateDto.cs, DepartmentUpdateDto.cs
    Student/     StudentDto.cs, StudentCreateDto.cs, StudentUpdateDto.cs
    Instructor/  InstructorDto.cs, InstructorCreateDto.cs, InstructorUpdateDto.cs
  Service/       DepartamentService.cs, CourseService.cs,
                 StudentService.cs, InstructorService.cs

School.Api/      controladores HTTP y registro de dependencias
```

La capa de **Aplicación** contiene los servicios de negocio. Cada servicio:
- Implementa las validaciones de los campos de su entidad (longitudes, rangos,
  formatos de correo y teléfono).
- Aplica reglas de negocio (códigos únicos, correos únicos, existencia de
  referencias como departamentos e instructores).
- Devuelve `ServiceResult<T>` con los datos o la lista de errores.

Los controladores consumen los servicios y traducen `ServiceResult` a respuestas
HTTP (200/201/204/400/404/409).

## Ejecutar

Requiere .NET 8 SDK. Desde la raíz del repositorio:

```powershell
dotnet restore School.sln
dotnet run --project School.Api
```

En modo Development, Swagger está disponible en `/swagger`. La base de datos
SQLite (`school.db`) y sus tablas se crean automáticamente al iniciar el API.
La cadena de conexión se configura en `School.Api/appsettings.json`.

## Endpoints

Los cuatro recursos ofrecen `GET /api/{recurso}`, `GET /api/{recurso}/{id}`,
`POST /api/{recurso}`, `PUT /api/{recurso}/{id}` y
`DELETE /api/{recurso}/{id}`:

- `/api/departments`
- `/api/courses`
- `/api/students`
- `/api/instructors`

Las operaciones requieren GUID válidos para los departamentos relacionados.
Los códigos de departamentos/cursos, números de estudiantes e instructores y
correos son únicos. Los conflictos de integridad o duplicados se devuelven como
`409 Conflict` con una lista de errores. Las validaciones de campos fallan con
`400 Bad Request`. También está disponible `GET /health`.

Ejemplo para crear un departamento:

```json
{
  "code": "SCI",
  "name": "Ciencias",
  "description": "Departamento de ciencias"
}
```
