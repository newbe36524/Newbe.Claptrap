using System.Collections.Generic;
using Newbe.Claptrap;

namespace HelloClaptrap.Models.Cart
{
    public class CartState : IStateData
    {
        public Dictionary<string, int> Items { get; set; }
    }
}