using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static SharedLogicOrchestrator.DebugFilters;

namespace CleanupUtility
{
	public sealed class Config : IConfig
	{
		[Description("Whether to enable or disable plugin")]
		public bool IsEnabled { get; set; } = true;

		[Description("The message to show most debug messages.")]
		public bool debugEnabled { get; set; } = false;

		[Description("Debug filter for logging levels.")]
		public Dictionary<DebugFilter, bool> DebugFilters { get; set; } =
		   new Dictionary<DebugFilter, bool> {

			   {  DebugFilter.All , false },
			   {  DebugFilter.Fine , false },
			   {  DebugFilter.Finer , false },
			   {  DebugFilter.Finest , false }
		   };

		[Description("Amount of time a thread will try to remove items before breaking until the next set of data is available. This is to prevent thread for constantly spinning with items it can't remove yet")]
		public TimeSpan spinoutTime { get; set; } = TimeSpan.FromSeconds(30);

		[Description("Item filter. If you want an item to be removed, add it here with a time associated")]
		public Dictionary<ItemType, TimeSpan> ItemFilter { get; set; } =
		   new Dictionary<ItemType, TimeSpan>
		   {
			   { ItemType.GrenadeHE, TimeSpan.FromSeconds(15) },
		   };
	}

}
