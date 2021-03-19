using CleanArchitecture.Application.Common.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Exceptions
{
    public class ForbiddenAccessExceptionTests
    {
        [Test]
        public void ShouldBeSerializable()
        {
            new ForbiddenAccessException().Should().BeBinarySerializable();
        }
    }
}
