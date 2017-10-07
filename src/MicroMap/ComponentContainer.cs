using System;
using System.Collections;
using System.Collections.Generic;

namespace MicroMap
{
    public class ComponentContainer : IEnumerable<IQueryComponent>
    {
        private readonly List<IQueryComponent> _components;

        public ComponentContainer()
        {
            _components = new List<IQueryComponent>();
        }

        public ComponentContainer Add(IQueryComponent component)
        {
            _components.Add(component);

            return this;
        }

        public IEnumerator<IQueryComponent> GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
