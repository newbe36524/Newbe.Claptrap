using System;
using System.Threading.Tasks;
namespace Domain.Minion
{
    public partial class TestClaptrap
    {
        public Task NoneEventMethodWithoutReturnValue()
        {
            throw new NotImplementedException();
        }
        public Task<DateTime> NoneEventMethodWithReturnValue()
        {
            throw new NotImplementedException();
        }
        public Task AddBalance(decimal value)
        {
            throw new NotImplementedException();
        }
        public Task<(decimal value, int n)> SomeMethod(TestEventDataType testEventDataType)
        {
            throw new NotImplementedException();
        }
    }
}
