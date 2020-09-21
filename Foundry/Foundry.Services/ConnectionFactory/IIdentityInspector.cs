
namespace Foundry.Services
{
    public interface IIdentityInspector<TEntity> where TEntity : class
    {
        string GetColumnsIdentityForType();
    }
}
