using System;
using System.Collections.Generic;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;
using Xunit;

namespace CleanArchitecture.UnitTests.Domain.Common
{
    public class ValueObjectCollectionCustomSize : ValueObject
    {
        public int Size { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            for (int i = 0; i < Size; i++)
            {
                yield return $"Value#{i}";
            }
        }
    }

    public class ValueObjectTests : ValueObject
    {
        private string domain;
        private string name;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return domain;
            yield return name;
        }

        [Theory]
        [Trait("MethodSection", "EqualOperatorTest")]
        [InlineData(null, null)]
        public void EqualOperator_Test_AllNull(ValueObject left, ValueObject right)
        {
            Assert.True(EqualOperator(left, right));
        }

        [Theory]
        [Trait("MethodSection", "EqualOperatorTest")]
        [InlineData("Domain1\\User1", "Domain1\\User1")]
        public void EqualOperator_Test_TrueCase(string left, string right)
        {
            Assert.True(EqualOperator((AdAccount)left, (AdAccount)right));
        }

        [Theory]
        [Trait("MethodSection", "EqualOperatorTest")]
        [InlineData("Domain1\\User1", "Domain1\\User1")]
        public void EqualOperator_Test_FalseCase(string left, string right)
        {
            Assert.False(EqualOperator((AdAccount)left, null));
            Assert.False(EqualOperator(null, (AdAccount)right));
        }

        [Theory]
        [Trait("MethodSection", "NotEqualOperatorTest")]
        [InlineData(null, null)]
        public void NotEqualOperator_Test_AllNull(ValueObject left, ValueObject right)
        {
            Assert.False(NotEqualOperator(left, right));
        }

        [Theory]
        [Trait("MethodSection", "EqualsTest")]
        [InlineData(null, "Domain1\\User1")]
        public void Equals_Test_Exceptions(string left, string right)
        {
            var exception = Assert.Throws<AdAccountInvalidException>(() => ((AdAccount)left).Equals(right));
            Assert.Equal("AD Account \"\" is invalid.", exception.Message);

            Assert.IsType<NullReferenceException>(exception.InnerException);
        }

        [Fact]
        [Trait("MethodSection", "EqualsTest")]
        public void Equals_Test_Enumerables()
        {
            var account1 = new ValueObjectCollectionCustomSize { Size = 2 };
            var account11 = new ValueObjectCollectionCustomSize { Size = 2 };
            var account2 = new ValueObjectCollectionCustomSize { Size = 4 };

            Assert.True(EqualOperator(account1, account11));
            Assert.False(EqualOperator(account1, account2));
        }

        [Theory]
        [Trait("MethodSection", "EqualsTest")]
        [InlineData("Domain1\\User1", "Domain1\\User1")]
        public void Equals_Test_TrueCase(string left, string right)
        {
            Assert.True(((AdAccount)left).Equals((AdAccount)right));
        }

        [Theory]
        [Trait("MethodSection", "EqualsTest")]
        [InlineData("Domain1\\User1", null)]
        [InlineData("Domain1\\User1", "string")]
        [InlineData("Domain1\\User1", "Domain1\\User2")]
        [InlineData("Domain1\\User1", "Domain1\\")]
        public void Equals_Test_False(string left, string right)
        {
            Assert.False(((AdAccount)left).Equals(right));
        }

        [Theory]
        [Trait("MethodSection", "EqualsTest")]
        [InlineData(null, null, "Domain\\User")]
        [InlineData("Domain", null, "Domain\\User")]
        [InlineData(null, "User", "Domain\\User")]
        [InlineData(null, "Domain", "Domain\\User")]
        [InlineData("User", null, "Domain\\User")]
        public void Equals_Test_AtomicValuesEnumerator_ReturnFalse(string domain, string name, string adAccountString)
        {
            var corruptedAccount = (AdAccount)"a\\a";
            typeof(AdAccount).GetProperty(nameof(corruptedAccount.Domain)).SetValue(corruptedAccount, domain);
            typeof(AdAccount).GetProperty(nameof(corruptedAccount.Name)).SetValue(corruptedAccount, name);

            var account = (AdAccount)adAccountString;

            Assert.False(account.Equals(corruptedAccount));
            Assert.False(corruptedAccount.Equals(account));
        }

        [Theory]
        [Trait("MethodSection", "EqualsTest")]
        [InlineData("Domain", "User", "Domain\\User")]
        public void Equals_Test_AtomicValuesEnumerator_ReturnTrue(string domain, string name, string adAccountString)
        {
            var corruptedAccount = (AdAccount)"a\\a";
            typeof(AdAccount).GetProperty(nameof(corruptedAccount.Domain)).SetValue(corruptedAccount, domain);
            typeof(AdAccount).GetProperty(nameof(corruptedAccount.Name)).SetValue(corruptedAccount, name);

            var account = (AdAccount)adAccountString;

            Assert.True(account.Equals(corruptedAccount));
            Assert.True(corruptedAccount.Equals(account));
        }

        [Theory]
        [Trait("MethodSection", "GetHashCodeTest")]
        [InlineData(null, null)]
        public void GetHashCode_Test_AllNull(string domain, string name)
        {
            this.domain = domain;
            this.name = name;
            Assert.Equal(0, GetHashCode());
        }

        [Theory]
        [Trait("MethodSection", "GetHashCodeTest")]
        [InlineData("Domain1", "User1")]
        [InlineData(null, "User1")]
        [InlineData("Domain1", null)]
        public void GetHashCode_Test_AtLeastOneArgNotNull(string domain, string name)
        {
            this.domain = domain;
            this.name = name;

            int domainHashCode = domain != null ? domain.GetHashCode() : 0;
            int nameHashCode = name != null ? name.GetHashCode() : 0;

            Assert.NotEqual(0, GetHashCode());
            Assert.Equal(domainHashCode ^ nameHashCode, GetHashCode());
        }
    }
}
