using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace CleanArchitecture.Domain.UnitTests.ValueObjects
{
    public class AdAccountTests
    {
        [Fact]
        public void ShouldHaveCorrectDomainAndName()
        {
            const string accountString = "SSW\\Jason";

            var account = AdAccount.For(accountString);

            account.Domain.ShouldBe("SSW");
            account.Name.ShouldBe("Jason");
        }

        [Fact]
        public void ToStringReturnsCorrectFormat()
        {
            const string accountString = "SSW\\Jason";

            var account = AdAccount.For(accountString);

            var result = account.ToString();

            result.ShouldBe(accountString);
        }

        [Fact]
        public void ImplicitConversionToStringResultsInCorrectString()
        {
            const string accountString = "SSW\\Jason";

            var account = AdAccount.For(accountString);

            string result = account;

            result.ShouldBe(accountString);
        }

        [Fact]
        public void ExplicitConversionFromStringSetsDomainAndName()
        {
            const string accountString = "SSW\\Jason";

            var account = (AdAccount)accountString;

            account.Domain.ShouldBe("SSW");
            account.Name.ShouldBe("Jason");
        }

        [Fact]
        public void ShouldThrowAdAccountInvalidExceptionForInvalidAdAccount()
        {
            Assert.Throws<AdAccountInvalidException>(() => (AdAccount)"SSWJason");
        }
    }
}
