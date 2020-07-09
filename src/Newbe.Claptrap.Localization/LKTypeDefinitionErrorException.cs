using System;

namespace Newbe.Claptrap.Localization
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