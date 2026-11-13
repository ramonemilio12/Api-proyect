# School API

API REST para administrar departamentos, cursos, estudiantes e instructores.
La solución está separada en tres proyectos y sigue la estructura de capas de
Dominio e Infraestructura.

## Estructura

```
School.Domain/
  Core/          BaseEntity.cs, Person.cs
  Entities/      Course.cs, Department.cs, Instructor.cs, Student.cs
  Repository/    IRepository.cs   (contrato genérico)

School.Infrastructure/
  Context/       SchoolContext.cs
  Core/          BaseRepository.cs, Repository.cs
  Exceptions/    CourseException.cs, DepartmentException.cs
  Interfaces/    ICourseRepository.cs, IDepartmentRepository.cs
  Models/        CursoModel.cs, DepartmentModel.cs   (DTOs)
  Repositories/  CourseRepository.cs, DepartmentRepository.cs

School.Api/      controladores HTTP y registro de dependencias
```

Los repositorios asignados (`Course` y `Department`) cuentan con interfaz,
modelo (DTO) y excepción propios. `Student` e `Instructor` se gestionan a
través del repositorio genérico `IRepository<T>`.

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
`409 Conflict`. También está disponible `GET /health`.

Ejemplo para crear un departamento:

```json
{
  "code": "SCI",
  "name": "Ciencias",
  "description": "Departamento de ciencias"
}
```
