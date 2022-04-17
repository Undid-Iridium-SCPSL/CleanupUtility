using System;
using System.Collections.Generic;

namespace CleanupUtility.Utility
{
	public class ItemCappedQueue<T>
	{
		private readonly int m_Capacity;
		private Queue<Tuple<T, DateTime>> internalQueue;
		public ItemCappedQueue(int capacity)
		{
			m_Capacity = capacity;
			internalQueue = new Queue<Tuple<T, DateTime>>(capacity);
		}

		public ItemCappedQueue()
		{
			internalQueue = new Queue<Tuple<T, DateTime>>();
		}

		/// <summary>
		/// Wrapper to add ItemBaseType and DateTime, or any object + datetime. Enque calls underlying Enque with Tuple added
		/// </summary>
		/// <returns> Nothing </returns>
		public void Enqueue(T item)
		{
			Tuple<T, DateTime> itemToAdd = Tuple.Create(item, DateTime.UtcNow);
			internalQueue.Enqueue(itemToAdd);
		}


		/// <summary>
		/// Wrapper to get ItemBaseType and DateTime, or any object + datetime. Deque calls underlying Deque
		/// </summary>
		/// <returns> Tuple of Object, DateTime </returns>
		public Tuple<T, DateTime> Deque()
		{
			return internalQueue.Dequeue();
		}

		///// <summary>
		///// Wrapper to peek ItemBaseType and DateTime, or any object + datetime. Peek calls underlying peek
		///// </summary>
		///// <returns> Tuple of Object, DateTime </returns>
		//public Tuple<T, DateTime> Peek()
		//{
		//	return internalQueue.Peek();
		//}


		/// <summary>
		/// Wrapper to peek ItemBaseType and DateTime, or any object + datetime. Peek calls underlying peek
		/// </summary>
		/// <returns> Tuple of Object, DateTime </returns>
		public Object Peek()
		{
			return internalQueue.Peek();
		}

		public IEnumerator<Tuple<T, DateTime>> GetEnumerator()
		{
			return internalQueue.GetEnumerator();
		}

		/// <summary>
		/// Clears internal queue
		/// </summary>
		internal void Clear()
		{
			this.internalQueue.Clear();
		}
	}
}
