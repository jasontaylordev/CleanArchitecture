using System;
namespace CleanArchitecture.Application.Common.Exceptions
{
    public class GenericException : Exception
    {
        public string ErrorCode { get; set; }

        public GenericException(string message) : base(message)
        {

        }

        public GenericException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
