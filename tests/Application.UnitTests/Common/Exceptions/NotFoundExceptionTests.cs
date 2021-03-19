using CleanArchitecture.Application.Common.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Exceptions
{
    public class NotFoundExceptionTests
    {
        [Test]
        public void ShouldBeBinarySerializable()
        {
            new NotFoundException("EntityName", 1).Should().BeBinarySerializable();
        }
    }
}
