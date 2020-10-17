using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CleanArchitecture.Application.Common.Extensions;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace CleanArchitecture.Application.UnitTests.Infrastructure
{
    public static class MoqUtils
    {
        /// <summary>
        /// Mocks a DbSet
        /// </summary>
        /// <typeparam name="TEnt">DbSet type</typeparam>
        /// <param name="dbMock">mock context that includes the DbSet to mock</param>
        /// <param name="expression">DbSet within the context</param>
        /// <param name="list">values to "bind" the DbSet to</param>
        /// <param name="clone">specifies if the input list should be deeply cloned </param>
        /// <returns>the mocked DbSet instance</returns>
        public static Mock<DbSet<TEnt>> SetDbSetData<TEnt>(this Mock<IApplicationDbContext> dbMock,
            Expression<Func<IApplicationDbContext, DbSet<TEnt>>> expression,
            IList<TEnt> list, 
            bool clone = true)
            where TEnt : class
        {
            var clonedList = clone ? list.DeepClone().ToList() : list.ToList();
            var mockDbSet = clonedList.AsQueryable().BuildMockDbSet();

            dbMock.Setup(expression).Returns(mockDbSet.Object);
            return mockDbSet;
        }
    }
}
