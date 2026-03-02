using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedEngine.Core
{
    public class ServiceLocator
    {
        private static ServiceLocator _instance;

        public static ServiceLocator Instance => _instance ??= new ServiceLocator();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _instance = null;
        }

        private readonly Dictionary<Type, IGameService> _registeredServices = new();

        public T Get<T>() where T : IGameService
        {
            Type type = typeof(T);

            if (_registeredServices.ContainsKey(type))
            {
                return (T)_registeredServices[type];
            }

            Debug.LogError($"{type} not registered with {GetType().Name}");
			
            throw new InvalidOperationException();
        }

        public bool TryGet<T>(out T gameService) where T : IGameService
        {
            if (_registeredServices.TryGetValue(typeof(T), out IGameService gameServiceInterface))
            {
                gameService = (T)gameServiceInterface;
                return true;
            }

            gameService = default;
            return false;
        }

        public void Register<T>(T service) where T : IGameService
        {
            Type type = typeof(T);

            if (!_registeredServices.ContainsKey(type))
            {
                _registeredServices.Add(type, service);
                return;
            }

            Debug.LogError($"Attempted to register service of type {type} which is already registered with the {GetType().Name}.");
        }

        public void Unregister<T>() where T : IGameService
        {
            Type type = typeof(T);

            if (_registeredServices.ContainsKey(type))
            {
                _registeredServices.Remove(type);
                return;
            }

            Debug.LogError($"Attempted to unregister service of type {type} which is not registered with the {GetType().Name}.");
        }

        public void WrapUp(bool appExit)
        {
            foreach (var service in _registeredServices.Values)
            {
                service.WrapUp(appExit);
            }
        }
    }
}