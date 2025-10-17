namespace CleanArchitecture.Domain.Entities;
public class Product : BaseAuditableEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }


    // constructor
    public Product(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;

    }

    // (optional) methods, validations, etc.
}

