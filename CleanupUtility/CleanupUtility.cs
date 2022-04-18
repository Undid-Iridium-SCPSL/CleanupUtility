using Exiled.API.Features;
using HarmonyLib;
using System;
using PlayerEvents = Exiled.Events.Handlers.Player;
using ServerEvents = Exiled.Events.Handlers.Server;


namespace CleanupUtility
{
	public class CleanupUtility : Plugin<Config>
	{

		public override string Name => "CleanupUtility";

		private Harmony harmony;
		private string harmonyId = "com.Undid-Iridium.CleanupUtility";

		public static Config earlyConfig;
		public override Version Version => new Version(1, 0, 0);
		public override Version RequiredExiledVersion => new Version(5, 1, 0, 0);

		public static CleanupUtility Instance;

		public override string Author => "Undid-Iridium";



		/// <summary>
		/// Entrance function called through Exile
		/// </summary>
		public override void OnEnabled()
		{
			RegisterEvents();
			RegisterHarmony();
			base.OnEnabled();
		}

		private void RegisterHarmony()
		{
			harmony = new Harmony(harmonyId + DateTime.Now.Ticks.ToString());
			harmony.PatchAll();
		}


		/// <summary>
		/// Destruction function called through Exile
		/// </summary>
		public override void OnDisabled()
		{
			UnRegisterEvents();
			UnRegisterHarmony();
			base.OnDisabled();
		}

		private void UnRegisterHarmony()
		{
			harmony.UnpatchAll(harmony.Id);
			harmony = null;
		}

		/// <summary>
		/// Registers events for EXILE to hook unto with cororotines (I think?)
		/// </summary>
		public void RegisterEvents()
		{
			// Register the event handler class. And add the event,
			// to the EXILED_Events event listener so we get the event.
			Instance = this;
			earlyConfig = Config;
			ServerEvents.RoundStarted += Events.EventHandler.RoundStart;
			ServerEvents.RestartingRound += Events.EventHandler.RestartingRound;

			Log.Info("SpawnControl has been loaded");

		}

		/// <summary>
		/// Unregisters the events defined in RegisterEvents, recommended that everything created be destroyed if not reused in some way.
		/// </summary>
		public void UnRegisterEvents()
		{

			ServerEvents.RoundStarted -= Events.EventHandler.RoundStart;
			ServerEvents.RestartingRound -= Events.EventHandler.RestartingRound;

			Instance = null;
			Log.Info("SpawnControl has been unloaded");
		}
	}
}
