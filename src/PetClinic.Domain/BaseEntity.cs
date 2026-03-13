namespace PetClinic.Domain;

public abstract class BaseEntity : BaseEntity<Guid>
{
    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}

public abstract class BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; set; } = default!;
}