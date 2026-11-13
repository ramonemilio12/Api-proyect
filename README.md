# BiciTaller API

API REST para administrar categorías, servicios, clientes y técnicos de un
taller de bicicletas. La solución está separada en cuatro proyectos y sigue una
arquitectura por capas (Dominio, Infraestructura, Aplicación y API).

## Estructura

```
BiciTaller.Domain/
  Core/          BaseEntity.cs, Persona.cs
  Entities/      Categoria.cs, Servicio.cs, Cliente.cs, Tecnico.cs
  Repository/    IRepository.cs          (contrato genérico)

BiciTaller.Infrastructure/
  Context/       BiciTallerContext.cs
  Core/          BaseRepository.cs, Repository.cs
  Exceptions/    CategoriaException.cs, ServicioException.cs
  Interfaces/    ICategoriaRepository.cs, IServicioRepository.cs
  Repositories/  CategoriaRepository.cs, ServicioRepository.cs

BiciTaller.Application/
  Contract/      ICategoriaService.cs, IServicioService.cs,
                 IClienteService.cs, ITecnicoService.cs
  Core/          IBaseService.cs, BaseService.cs, ServiceResult.cs
  Dtos/          DtoBase.cs
    Categoria/   CategoriaDto.cs, CategoriaCreateDto.cs, CategoriaUpdateDto.cs
    Servicio/    ServicioDto.cs, ServicioCreateDto.cs, ServicioUpdateDto.cs
    Cliente/     ClienteDto.cs, ClienteCreateDto.cs, ClienteUpdateDto.cs
    Tecnico/     TecnicoDto.cs, TecnicoCreateDto.cs, TecnicoUpdateDto.cs
  Service/       CategoriaService.cs, ServicioService.cs,
                 ClienteService.cs, TecnicoService.cs

BiciTaller.Api/      controladores HTTP y registro de dependencias
```

La capa de **Aplicación** contiene los servicios de negocio. Cada servicio:
- Implementa las validaciones de los campos de su entidad (longitudes, rangos,
  formatos de correo y teléfono).
- Aplica reglas de negocio (códigos únicos, correos únicos, existencia de
  referencias como categorías y técnicos).
- Devuelve `ServiceResult<T>` con los datos o la lista de errores.

Los controladores consumen los servicios y traducen `ServiceResult` a respuestas
HTTP (200/201/204/400/404/409).

## Ejecutar

Requiere .NET 8 SDK. Desde la raíz del repositorio:

```powershell
dotnet restore BiciTaller.sln
dotnet run --project BiciTaller.Api
```

En modo Development, Swagger está disponible en `/swagger`. La base de datos
SQLite (`bicitaller.db`) y sus tablas se crean automáticamente al iniciar el API.
La cadena de conexión se configura en `BiciTaller.Api/appsettings.json`.

## Endpoints

Los cuatro recursos ofrecen `GET /api/{recurso}`, `GET /api/{recurso}/{id}`,
`POST /api/{recurso}`, `PUT /api/{recurso}/{id}` y
`DELETE /api/{recurso}/{id}`:

- `/api/categorias`
- `/api/servicios`
- `/api/clientes`
- `/api/tecnicos`

Las operaciones requieren GUID válidos para las categorías relacionadas.
Los códigos de categorías/servicios, números de clientes y técnicos y
correos son únicos. Los conflictos de integridad o duplicados se devuelven como
`409 Conflict` con una lista de errores. Las validaciones de campos fallan con
`400 Bad Request`. También está disponible `GET /health`.

Ejemplo para crear una categoría:

```json
{
  "codigo": "REP",
  "nombre": "Reparación",
  "descripcion": "Servicios de reparación general"
}
```
