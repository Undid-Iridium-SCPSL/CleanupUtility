using Exiled.API.Interfaces;
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

		public Dictionary<DebugFilter, bool> DebugFilters { get; set; } =
		   new Dictionary<DebugFilter, bool> {

			   {  DebugFilter.All , false },
			   {  DebugFilter.Fine , false },
			   {  DebugFilter.Finer , false },
			   {  DebugFilter.Finest , false }
		   };
	}
}
