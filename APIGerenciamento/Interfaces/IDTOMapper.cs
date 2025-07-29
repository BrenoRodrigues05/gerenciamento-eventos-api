namespace APIGerenciamento.Interfaces
{
    public interface IDTOMapper <TDto, TEntity, TPatchDto>
    {
        TEntity ToEntity(TDto dto);
        TDto ToDto(TEntity entity);

        TPatchDto ToPatchDto(TEntity entity);
        void PatchToEntity(TPatchDto patchDto, TEntity entity);
    }
}
