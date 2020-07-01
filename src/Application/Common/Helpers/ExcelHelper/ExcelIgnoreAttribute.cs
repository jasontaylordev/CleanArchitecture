using System;

namespace CleanArchitecture.Application.Common.Helpers.ExcelHelper
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ExcelIgnore : Attribute
    {
        public bool Ignored { get; } = true;
    }
}
