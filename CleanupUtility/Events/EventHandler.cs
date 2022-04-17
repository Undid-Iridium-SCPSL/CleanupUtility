using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using static SharedLogicOrchestrator.DebugFilters;

namespace CleanupUtility.Events
{
	public class EventHandler
	{
		public static CoroutineHandle CleanupCoroutine;

		public static PickupChecker pickupChecker;

		internal static void RoundStart()
		{
			if (CleanupCoroutine != null)
			{
				if (CleanupCoroutine.IsRunning)
				{
					Timing.KillCoroutines(CleanupCoroutine);
				}
			}

			Log.Debug($"Round started, starting coroutine", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);
			pickupChecker = new PickupChecker();

			CleanupCoroutine = Timing.RunCoroutine(pickupChecker.CheckItems());

		}

		internal static void RoundEnd(RoundEndedEventArgs ev)
		{
			if (CleanupCoroutine != null)
			{
				if (CleanupCoroutine.IsRunning)
				{
					Timing.KillCoroutines(CleanupCoroutine);
				}
			}
			Log.Debug($"Round Ending, End coroutine", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);
			pickupChecker.releaseCoroutine();
			pickupChecker = null;
		}

		internal static void DroppedEvent(DroppingItemEventArgs droppedItem)
		{
			//if (droppedItem.Item != null)
			//{
			//	Timing.CallDelayed(5, () =>
			//	{
			//		Pickup currItem = Pickup.Get(droppedItem.Item.Base.PickupDropModel);
			//		Log.Debug($"Dropping Item even called, What was item {droppedItem.Item.Type} and what was pickup {currItem.Type}", CleanupUtility.Instance.Config.DebugFilters[DebugFilter.Fine]);
			//		pickupChecker.itemTrackingQueue.Enqueue(currItem);
			//	});
			//}
		}

	}
}
