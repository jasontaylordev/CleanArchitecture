using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Domain.UnitTests.ValueObjects
{
    public class AdAccountTests
    {
        [Test]
        public void ShouldHaveCorrectDomainAndName()
        {
            const string accountString = "SSW\\Jason";

            var account = AdAccount.For(accountString);

            account.Domain.Should().Be("SSW");
            account.Name.Should().Be("Jason");
        }

        [Test]
        public void ToStringReturnsCorrectFormat()
        {
            const string accountString = "SSW\\Jason";

            var account = AdAccount.For(accountString);

            var result = account.ToString();

            result.Should().Be(accountString);
        }

        [Test]
        public void ImplicitConversionToStringResultsInCorrectString()
        {
            const string accountString = "SSW\\Jason";

            var account = AdAccount.For(accountString);

            string result = account;

            result.Should().Be(accountString);
        }

        [Test]
        public void ExplicitConversionFromStringSetsDomainAndName()
        {
            const string accountString = "SSW\\Jason";

            var account = (AdAccount)accountString;

            account.Domain.Should().Be("SSW");
            account.Name.Should().Be("Jason");
        }

        [Test]
        public void ShouldThrowAdAccountInvalidExceptionForInvalidAdAccount()
        {
            FluentActions.Invoking(() => (AdAccount)"SSWJason")
                .Should().Throw<AdAccountInvalidException>();
        }
    }
}
