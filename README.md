# BiciTaller API

API REST para administrar categorías, servicios, clientes y técnicos de un
taller de bicicletas. La solución está separada en tres proyectos y sigue la
estructura de capas de Dominio e Infraestructura.

## Estructura

```
BiciTaller.Domain/
  Core/          BaseEntity.cs, Persona.cs
  Entities/      Categoria.cs, Servicio.cs, Cliente.cs, Tecnico.cs
  Repository/    IRepository.cs   (contrato genérico)

BiciTaller.Infrastructure/
  Context/       BiciTallerContext.cs
  Core/          BaseRepository.cs, Repository.cs
  Exceptions/    CategoriaException.cs, ServicioException.cs
  Interfaces/    ICategoriaRepository.cs, IServicioRepository.cs
  Models/        CategoriaModel.cs, ServicioModel.cs   (DTOs)
  Repositories/  CategoriaRepository.cs, ServicioRepository.cs

BiciTaller.Api/  controladores HTTP y registro de dependencias
```

Los repositorios asignados (`Categoria` y `Servicio`) cuentan con interfaz,
modelo (DTO) y excepción propios. `Cliente` y `Tecnico` se gestionan a
través del repositorio genérico `IRepository<T>`.

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
`409 Conflict`. También está disponible `GET /health`.

Ejemplo para crear una categoría:

```json
{
  "codigo": "REP",
  "nombre": "Reparacion",
  "descripcion": "Servicios de reparacion general"
}
```
