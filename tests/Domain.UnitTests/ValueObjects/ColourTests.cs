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
        var code = "#FFFFFF";

        var colour = Colour.From(code);

        colour.Code.ShouldBe(code);
    }

    [Test]
    public void ToStringReturnsCode()
    {
        var colour = Colour.White;

        colour.ToString().ShouldBe(colour.Code);
    }

    [Test]
    public void ShouldPerformImplicitConversionToColourCodeString()
    {
        string code = Colour.White;

        code.ShouldBe("#FFFFFF");
    }

    [Test]
    public void ShouldPerformExplicitConversionGivenSupportedColourCode()
    {
        var colour = (Colour)"#FFFFFF";

        colour.ShouldBe(Colour.White);
    }

    [Test]
    public void ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode()
    {
        Should.Throw<UnsupportedColourException>(() => Colour.From("##FF33CC"));
    }

    [Test]
    public void ShouldBeComparableWithOperators()
    {
        var color1 = new Colour("#FFFFFF");
        var color2 = new Colour("#FFFFFF");
        var color3 = new Colour("#AAAAAA");
        (color1 == color2).ShouldBe(true);
        (color1 == color3).ShouldBe(false);
    }
}
