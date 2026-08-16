namespace Samad.Domain.Entity
{
    public interface IEntity<TKey>
    where TKey : notnull
    {
        TKey Id { get; set; }
    }
}
