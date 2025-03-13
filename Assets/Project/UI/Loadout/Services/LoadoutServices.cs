using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.UI.Loadout.Services
{
	/// <summary>
	/// Service locator for the Loadout UI system. Provides access to shared services.
	/// </summary>
	public static class LoadoutServices
	{
		private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

		/// <summary>
		/// Registers a service with the service locator.
		/// </summary>
		/// <typeparam name="T">The type of service to register</typeparam>
		/// <param name="service">The service instance</param>
		public static void Register<T>(T service) where T : class
		{
			Type type = typeof(T);

			if (Services.ContainsKey(type))
			{
				Debug.LogWarning($"Service of type {type.Name} is already registered. Replacing...");
				Services[type] = service;
			}
			else
			{
				Services.Add(type, service);
			}
		}

		/// <summary>
		/// Gets a service from the service locator.
		/// </summary>
		/// <typeparam name="T">The type of service to get</typeparam>
		/// <returns>The service instance, or null if not found</returns>
		public static T Get<T>() where T : class
		{
			Type type = typeof(T);

			if (Services.TryGetValue(type, out object service))
			{
				return (T)service;
			}

			Debug.LogError($"Service of type {type.Name} is not registered.");
			return null;
		}

		/// <summary>
		/// Checks if a service is registered.
		/// </summary>
		/// <typeparam name="T">The type of service to check</typeparam>
		/// <returns>True if the service is registered, false otherwise</returns>
		public static bool IsRegistered<T>() where T : class
		{
			return Services.ContainsKey(typeof(T));
		}

		/// <summary>
		/// Unregisters a service from the service locator.
		/// </summary>
		/// <typeparam name="T">The type of service to unregister</typeparam>
		public static void Unregister<T>() where T : class
		{
			Type type = typeof(T);

			if (Services.ContainsKey(type))
			{
				Services.Remove(type);
			}
			else
			{
				Debug.LogWarning($"Attempted to unregister service of type {type.Name}, but it was not registered.");
			}
		}

		/// <summary>
		/// Clears all registered services.
		/// </summary>
		public static void Clear()
		{
			Services.Clear();
		}
	}
}