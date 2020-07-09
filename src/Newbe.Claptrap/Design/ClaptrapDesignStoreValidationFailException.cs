using System;

namespace Newbe.Claptrap.Design
{
    public class ClaptrapDesignStoreValidationFailException : Exception
    {
        public ClaptrapDesignStoreValidationFailException(string details)
            : this(
                $"something error in ClaptrapDesignStore, you can check your code by this details : {details}",
                details)
        {
        }

        public ClaptrapDesignStoreValidationFailException(string message, string details) : base(message)
        {
            Details = details;
        }

        public ClaptrapDesignStoreValidationFailException(string message, Exception innerException, string details) :
            base(message, innerException)
        {
            Details = details;
        }

        public string Details { get; }
    }
}