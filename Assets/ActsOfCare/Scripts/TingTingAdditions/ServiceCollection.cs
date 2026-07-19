using System;
using System.Collections;
using System.Collections.Generic;
using TNG_Framework.TingTing.Services;
namespace ActsOfCare.TingTingAdditions
{
	public class ServiceCollection : IList<IService>
	{
		private List<IService> services = new List<IService>();
		
		// Builder Pattern
		public ServiceCollection AddService<T>() where T: IService, new()
		{
			T service = new T();
			service.OnInit();
			services.Add(service);
			return this;
		}

		public ServiceCollection AddService(IService service)
		{
			service.OnInit();
			services.Add(service);
			return this;
		}
		
		public IEnumerator<IService> GetEnumerator()
		{
			return services.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)services).GetEnumerator();
		}
		public void Add(IService item)
		{
			services.Add(item);
		}
		public void Clear()
		{
			services.Clear();
		}
		public bool Contains(IService item)
		{
			return services.Contains(item);
		}
		public void CopyTo(IService[] array, int arrayIndex)
		{
			services.CopyTo(array, arrayIndex);
		}
		public bool Remove(IService item)
		{
			return services.Remove(item);
		}
		public int Count => services.Count;
		public bool IsReadOnly => false;
		public int IndexOf(IService item)
		{
			return services.IndexOf(item);
		}
		public void Insert(int index, IService item)
		{
			services.Insert(index, item);
		}
		public void RemoveAt(int index)
		{
			services.RemoveAt(index);
		}
		public IService this[int index]
		{
			get => services[index];
			set => services[index] = value;
		}
	}
}