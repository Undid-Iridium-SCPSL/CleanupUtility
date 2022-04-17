using Exiled.API.Features;
using System;
using PlayerEvents = Exiled.Events.Handlers.Player;
using ServerEvents = Exiled.Events.Handlers.Server;


namespace CleanupUtility
{
	public class CleanupUtility : Plugin<Config>
	{

		private string harmonyId = "com.Undid-Iridium.CleanupUtility";
		public static Config earlyConfig;
		public override Version Version => new Version(1, 0, 0);
		public override Version RequiredExiledVersion => new Version(5, 1, 0, 0);

		public static CleanupUtility Instance;

		public override string Author => "Undid-Iridium";
		public override string Name => "Advanced Subclassing Redux";


		/// <summary>
		/// Entrance function called through Exile
		/// </summary>
		public override void OnEnabled()
		{
			RegisterEvents();

			base.OnEnabled();
		}


		/// <summary>
		/// Destruction function called through Exile
		/// </summary>
		public override void OnDisabled()
		{
			UnRegisterEvents();

			base.OnDisabled();
		}

		/// <summary>
		/// Registers events for EXILE to hook unto with cororotines (I think?)
		/// </summary>
		public void RegisterEvents()
		{
			// Register the event handler class. And add the event,
			// to the EXILED_Events event listener so we get the event.
			earlyConfig = Config;
			ServerEvents.RoundStarted += Events.EventHandler.RoundStart;
			ServerEvents.RoundEnded += Events.EventHandler.RoundEnd;
			PlayerEvents.DroppingItem += Events.EventHandler.DroppedEvent;


			Log.Info("SpawnControl has been loaded");

		}

		/// <summary>
		/// Unregisters the events defined in RegisterEvents, recommended that everything created be destroyed if not reused in some way.
		/// </summary>
		public void UnRegisterEvents()
		{

			ServerEvents.RoundStarted -= Events.EventHandler.RoundStart;
			ServerEvents.RoundEnded -= Events.EventHandler.RoundEnd;
			PlayerEvents.DroppingItem -= Events.EventHandler.DroppedEvent;

			Log.Info("SpawnControl has been unloaded");
		}
	}
}
