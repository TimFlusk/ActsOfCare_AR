using System;
using System.Collections;
using System.Collections.Generic;
namespace ActsOfCare.Data
{
	[Serializable]
	public class UserDetailsCollection : IList<UserDetails>
	{
		private List<UserDetails> userDetails;
		public IEnumerator<UserDetails> GetEnumerator()
		{
			return userDetails.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)userDetails).GetEnumerator();
		}
		public void Add(UserDetails item)
		{
			userDetails.Add(item);
		}
		public void Clear()
		{
			userDetails.Clear();
		}
		public bool Contains(UserDetails item)
		{
			return userDetails.Contains(item);
		}
		public void CopyTo(UserDetails[] array, int arrayIndex)
		{
			userDetails.CopyTo(array, arrayIndex);
		}
		public bool Remove(UserDetails item)
		{
			return userDetails.Remove(item);
		}
		public int Count => userDetails.Count;
		public bool IsReadOnly => ((ICollection<UserDetails>)userDetails).IsReadOnly;
		public int IndexOf(UserDetails item)
		{
			return userDetails.IndexOf(item);
		}
		public void Insert(int index, UserDetails item)
		{
			userDetails.Insert(index, item);
		}
		public void RemoveAt(int index)
		{
			userDetails.RemoveAt(index);
		}
		public UserDetails this[int index]
		{
			get => userDetails[index];
			set => userDetails[index] = value;
		}
	}
}