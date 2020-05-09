using System;

namespace Newbe.Claptrap
{
    public class LKTypeDefinitionErrorException : Exception
    {
        public LKTypeDefinitionErrorException()
        {
        }

        public LKTypeDefinitionErrorException(string message) : base(message)
        {
        }
    }
}