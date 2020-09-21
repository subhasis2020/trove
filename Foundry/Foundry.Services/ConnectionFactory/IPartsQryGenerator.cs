
namespace Foundry.Services
{
    public interface IPartsQryGenerator<TEntity> where TEntity : class
    {
        string GenerateSelect();
        string GenerateSelect(object fieldsFilter);
        string GeneratePartInsert(string identityField = null);
        string GenerateDelete(object parameters);
        string GenerateUpdate(object pks);
        string GenerateDeleteEntity(object parameters);
        string GenerateMaxSelect(string identityField = null);
    }
}
