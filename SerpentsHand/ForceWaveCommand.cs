using CommandSystem;
using System;

namespace SerpentsHand
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	internal class ForceWaveCommand : ICommand
	{
		public string Command { get; set; } = "forceshwave";
		public string Description { get; set; } = "Forces SH on next spawn wave!";
		public string[] Aliases { get; set; } = new string[]
		{
			"forcesh",
			"shwave"
		};

		public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents))
			{
				response = "You do not have permission to use this command!";
				return false;
			}

			Plugin.Instance.IsForced = true;
			Plugin.Instance.IsSpawnable = true;

			response = "Next Chaos wave will be SH!";
			return true;
		}
	}
}