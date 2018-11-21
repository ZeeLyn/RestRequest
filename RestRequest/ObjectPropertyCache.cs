using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace RestRequest
{
	public class ObjectPropertyCache
	{
		private static readonly ConcurrentDictionary<Type, List<PropertyInfo>>
			Cache = new ConcurrentDictionary<Type, List<PropertyInfo>>();

		public static List<PropertyInfo> Set(Type type, List<PropertyInfo> property)
		{
			return Cache.AddOrUpdate(type, property, (key, oldValue) => property);
		}

		public static List<PropertyInfo> Get(Type type)
		{
			return Cache.TryGetValue(type, out var value) ? value : default;
		}
	}
}
