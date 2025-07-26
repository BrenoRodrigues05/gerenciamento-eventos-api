namespace APIGerenciamento.Interfaces
{
    public interface IDTOMapper <TDto, TEntity>
    {
        TEntity ToEntity(TDto dto);
        TDto ToDto(TEntity entity);
    }
}
