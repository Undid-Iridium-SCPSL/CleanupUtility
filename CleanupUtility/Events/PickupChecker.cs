using CleanupUtility.Utility;
using Exiled.API.Features.Items;
using MEC;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CleanupUtility.Events
{
	public class PickupChecker
	{
		List<Pickup> toRemove = new List<Pickup>();
		Thread backgroundCleanerThread;
		private bool releaseChecker;
		private Queue<bool> releaseQueue;

		public Dictionary<Pickup, DateTime> pickups = new Dictionary<Pickup, DateTime>();

		public ItemCappedQueue<Pickup> itemTrackingQueue;

		public PickupChecker()
		{
			itemTrackingQueue = new ItemCappedQueue<Pickup>();
			releaseChecker = false;
			releaseQueue = new Queue<bool>();
			backgroundCleanerThread = new Thread(() => ThreadedCheck(releaseQueue, itemTrackingQueue));
			backgroundCleanerThread.Start();
		}

		public void ThreadedCheck(Queue<bool> releaseQueue, ItemCappedQueue<Pickup> itemTrackingQueue)
		{
			Monitor.Wait(this);
			while (true)
			{



				if (releaseQueue.Count > 0)
				{
					break;
				}

				bool stillChecking = true;
				while (stillChecking)
				{
					Tuple<Pickup, DateTime> item = (Tuple<Pickup, DateTime>)itemTrackingQueue.Peek();
					if ((DateTime.UtcNow - item.Item2).TotalSeconds > 30)
					{
						itemTrackingQueue.Deque();
						toRemove.Add(item.Item1);
					}
					else
					{
						stillChecking = false;
					}
				}

				Monitor.Wait(this);


			}
		}



		public IEnumerator<float> CheckItems()
		{
			yield return Timing.WaitForSeconds(10);
			Monitor.Pulse(backgroundCleanerThread);
			yield return Timing.WaitForSeconds(10);

			while (true && !releaseChecker)
			{
				Monitor.Pulse(backgroundCleanerThread);
				lock (toRemove)
				{
					foreach (Pickup pickup in toRemove)
						pickup.Destroy();
				}
				yield return Timing.WaitForSeconds(10);
			}
		}

		/// <summary>
		/// Direct way of making the IEnumerable loop to stop if Timing.KillCourutines fails
		/// </summary>
		public void releaseCoroutine()
		{
			releaseChecker = true;

			this.releaseQueue.Enqueue(true);

			itemTrackingQueue.Clear();
		}
	}
}
