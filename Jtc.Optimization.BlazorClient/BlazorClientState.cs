using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient
{
    public class BlazorClientState
    {

        Dictionary<Type, Action> _registry = new Dictionary<Type, Action>();

        public void SubscribeStateHasChanged(Type componentType, Action action)
        {
            if (_registry.ContainsKey(componentType))
            {
                _registry.Remove(componentType);
            }
            _registry.Add(componentType, action);

        }

        public void NotifyStateHasChanged(Type componentType)
        {
            _registry[componentType].Invoke();
        }

    }
}
