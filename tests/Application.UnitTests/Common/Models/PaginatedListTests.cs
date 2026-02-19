using CleanArchitecture.Application.Common.Models;
using NUnit.Framework;
using Shouldly;

namespace CleanArchitecture.Application.UnitTests.Common.Models;

public class PaginatedListTests
{
    [Test]
    public void HasPreviousPage_ShouldBeFalse_WhenOnFirstPage()
    {
        var list = new PaginatedList<int>([], 100, pageNumber: 1, pageSize: 10);

        list.HasPreviousPage.ShouldBeFalse();
    }

    [Test]
    public void HasPreviousPage_ShouldBeTrue_WhenOnSecondPage()
    {
        var list = new PaginatedList<int>([], 100, pageNumber: 2, pageSize: 10);

        list.HasPreviousPage.ShouldBeTrue();
    }

    [Test]
    public void HasPreviousPage_ShouldBeTrue_WhenOnePageBeyondLastPage()
    {
        var list = new PaginatedList<int>([], 100, pageNumber: 11, pageSize: 10);

        list.HasPreviousPage.ShouldBeTrue();
    }

    [Test]
    public void HasPreviousPage_ShouldBeFalse_WhenTwoPagesOrMoreBeyondLastPage()
    {
        var list = new PaginatedList<int>([], 100, pageNumber: 12, pageSize: 10);

        list.HasPreviousPage.ShouldBeFalse();
    }

    [Test]
    public void HasNextPage_ShouldBeFalse_WhenOnLastPage()
    {
        var list = new PaginatedList<int>([], 100, pageNumber: 10, pageSize: 10);

        list.HasNextPage.ShouldBeFalse();
    }

    [Test]
    public void HasNextPage_ShouldBeTrue_WhenNotOnLastPage()
    {
        var list = new PaginatedList<int>([], 100, pageNumber: 9, pageSize: 10);

        list.HasNextPage.ShouldBeTrue();
    }
}
