using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public interface ITestClaptrap : IClaptrapGrain
    {
        Task NoneEventMethodWithoutReturnValue();
        Task<DateTime> NoneEventMethodWithReturnValue();
        Task AddBalance(decimal value);
        Task<(decimal value, int n)> SomeMethod(TestEventDataType testEventDataType);
    }
}