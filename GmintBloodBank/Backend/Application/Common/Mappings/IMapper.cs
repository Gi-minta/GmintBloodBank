namespace Application.Common.Mappings;

/// <summary>
/// Contrato de mapeo manual entre una entidad de dominio y un DTO.
/// Reemplaza los perfiles de AutoMapper con código explícito y testeable.
/// </summary>
public interface IMapper<TEntity, TDto>
{
    TDto ToDto(TEntity entity);

    TEntity ToEntity(TDto dto);
}

/// <summary>
/// Contrato de mapeo de solo lectura (proyección hacia DTO).
/// Útil para Queries donde no se requiere reconstrucción de la entidad.
/// </summary>
public interface IReadOnlyMapper<TEntity, TDto>
{
    TDto ToDto(TEntity entity);
}
