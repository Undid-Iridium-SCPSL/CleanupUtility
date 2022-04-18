using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using System;
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



		internal static void RestartingRound()
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
	}
}
