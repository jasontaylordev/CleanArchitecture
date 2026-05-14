using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;
using NUnit.Framework;
using Shouldly;

namespace CleanArchitecture.Domain.UnitTests.ValueObjects;

public class ColourTests
{
    [Test]
    public void ShouldReturnCorrectColourCode()
    {
        var code = "#E05C4D";

        var colour = Colour.From(code);

        colour.Code.ShouldBe(code);
    }

    [Test]
    public void ToStringReturnsCode()
    {
        var colour = Colour.Red;

        colour.ToString().ShouldBe(colour.Code);
    }

    [Test]
    public void ShouldPerformImplicitConversionToColourCodeString()
    {
        string code = Colour.Red;

        code.ShouldBe("#E05C4D");
    }

    [Test]
    public void ShouldPerformExplicitConversionGivenSupportedColourCode()
    {
        var colour = (Colour)"#E05C4D";

        colour.ShouldBe(Colour.Red);
    }

    [Test]
    public void ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode()
    {
        Should.Throw<UnsupportedColourException>(() => Colour.From("##FF33CC"));
    }

    [Test]
    public void ShouldBeComparableWithOperators()
    {
        var color1 = new Colour("#E05C4D");
        var color2 = new Colour("#E05C4D");
        var color3 = new Colour("#AAAAAA");
        (color1 == color2).ShouldBe(true);
        (color1 == color3).ShouldBe(false);
    }
}
