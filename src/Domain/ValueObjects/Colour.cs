namespace CleanArchitecture.Domain.ValueObjects;

public class Colour(string code) : ValueObject
{
    public static Colour From(string code)
    {
        var colour = new Colour(code);

        if (!SupportedColours.Contains(colour))
        {
            throw new UnsupportedColourException(code);
        }

        return colour;
    }

    public static Colour Red => new("#E05C4D");

    public static Colour Orange => new("#D98B2B");

    public static Colour Green => new("#4CAF50");

    public static Colour Teal => new("#26A69A");

    public static Colour Blue => new("#5C6BC0");

    public static Colour Purple => new("#AB47BC");

    public static Colour Grey => new("#78909C");

    public string Code { get; private set; } = string.IsNullOrWhiteSpace(code)?"#000000":code;

    public static implicit operator string(Colour colour)
    {
        return colour.ToString();
    }

    public static explicit operator Colour(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    public static IEnumerable<Colour> SupportedColours
    {
        get
        {
            yield return Red;
            yield return Orange;
            yield return Green;
            yield return Teal;
            yield return Blue;
            yield return Purple;
            yield return Grey;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
