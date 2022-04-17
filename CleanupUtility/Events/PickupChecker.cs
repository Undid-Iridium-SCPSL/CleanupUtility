using CleanupUtility.Utility;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using System;
using System.Collections.Generic;
using System.Threading;
using static SharedLogicOrchestrator.DebugFilters;

namespace CleanupUtility.Events
{
	public class PickupChecker
	{
		LinkedList<Pickup> toRemove = new LinkedList<Pickup>();
		Thread backgroundCleanerThread;
		private bool releaseChecker;
		private Queue<bool> releaseQueue;

		public Dictionary<Pickup, DateTime> pickups = new Dictionary<Pickup, DateTime>();

		private object externalThreadLock = new object();

		public ItemCappedQueue<Pickup> itemTrackingQueue;

		public PickupChecker()
		{
			itemTrackingQueue = new ItemCappedQueue<Pickup>();
			releaseChecker = false;
			releaseQueue = new Queue<bool>();
			backgroundCleanerThread = new Thread(() => ThreadedCheck(releaseQueue, itemTrackingQueue, externalThreadLock));
			backgroundCleanerThread.Start();
		}

		public void ThreadedCheck(Queue<bool> releaseQueue, ItemCappedQueue<Pickup> itemTrackingQueue, object externalThreadLock)
		{



			Log.Debug($"ThreadedCheck started and will wait", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);
			lock (externalThreadLock)
			{
				Monitor.Wait(externalThreadLock);
			}
			Log.Debug($"ThreadedCheck about to start loop", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);

			while (true)
			{
				Log.Debug($"CheckItems looping", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);


				if (releaseQueue.Count > 0)
				{
					break;
				}

				bool stillChecking = true;
				while (stillChecking && itemTrackingQueue.Count != 0)
				{
					Tuple<Pickup, DateTime> item = (Tuple<Pickup, DateTime>)itemTrackingQueue.Peek();
					Log.Debug($"Still itemTrackingQueue looping, what was item {item.Item1.Type}", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Finest]);
					if ((DateTime.UtcNow - item.Item2).TotalSeconds > 30)
					{
						itemTrackingQueue.Deque();
						toRemove.AddLast(item.Item1);
					}
					else
					{
						stillChecking = false;
					}
				}

				Log.Debug($"itemTrackingQueue sleeping", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);
				lock (externalThreadLock)
				{
					Monitor.Wait(externalThreadLock);
				}


			}
		}



		public IEnumerator<float> CheckItems()
		{
			Log.Debug($"CheckItems Called", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);
			yield return Timing.WaitForSeconds(10);
			lock (externalThreadLock)
			{
				Monitor.Pulse(externalThreadLock);
			}
			yield return Timing.WaitForSeconds(10);
			Log.Debug($"CheckItems about to start loop", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);

			while (true && !releaseChecker)
			{
				try
				{
					lock (externalThreadLock)
					{
						Monitor.Pulse(externalThreadLock);
					}
					lock (toRemove)
					{
						while (!toRemove.IsEmpty())
						{
							try
							{
								toRemove.First.Value.Destroy();
								toRemove.RemoveFirst();
							}
							catch (Exception ex)
							{
								Log.Debug($" Unable to delete our object for some reason {ex} ", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Finer]);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Log.Debug($" Unable to lock our threads for some reason {ex} ", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Finer]);
				}
				yield return Timing.WaitForSeconds(10);
				Log.Debug($"Time to loop our CheckItems again", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);
			}
		}

		/// <summary>
		/// Direct way of making the IEnumerable loop to stop if Timing.KillCourutines fails
		/// </summary>
		public void releaseCoroutine()
		{
			Log.Debug($"releaseCoroutine running", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);
			releaseChecker = true;

			this.releaseQueue.Enqueue(true);

			itemTrackingQueue.Clear();
		}
	}
}
