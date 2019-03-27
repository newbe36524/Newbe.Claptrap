using System;
using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap.Autofac
{
    public class EventMethodRegistrationFinder : IEventMethodRegistrationFinder
    {
        public IEnumerable<EventMethodRegistration> FindAll(Type[] types)
        {
            var finders = new IEventMethodRegistrationFinder[]
            {
                new NamespaceFinder(),
            };
            var re = finders.SelectMany(x => x.FindAll(types));
            return re;
        }

        private class NamespaceFinder : IEventMethodRegistrationFinder
        {
            public IEnumerable<EventMethodRegistration> FindAll(Type[] types)
            {
                var re = types
                    .Where(x => x.Namespace.Contains(".N20EventMethods."))
                    .Select(x =>
                        new EventMethodRegistration
                        {
                            Type = x
                        });
                return re;
            }
        }
    }
}