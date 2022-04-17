using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using MEC;

namespace CleanupUtility.Events
{
	internal class EventHandler
	{
		public static CoroutineHandle CleanupCoroutine;

		private static PickupChecker pickupChecker;

		internal static void RoundStart()
		{
			if (CleanupCoroutine != null)
			{
				if (CleanupCoroutine.IsRunning)
				{
					Timing.KillCoroutines(CleanupCoroutine);
				}
			}

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

			pickupChecker.releaseCoroutine();
			pickupChecker = null;
		}

		internal static void DroppedEvent(DroppingItemEventArgs obj)
		{

			pickupChecker.itemTrackingQueue.Enqueue(Pickup.Get(obj.Item.Base.PickupDropModel));
		}

	}
}
