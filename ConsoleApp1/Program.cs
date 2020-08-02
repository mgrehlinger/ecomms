using ECOMMS_Manager;
using System;
using TestConsoleApp;

namespace ConsoleApp1
{
	class Program
	{

		static void Main(string[] args)
		{
			string nats_server = @"nats://192.168.1.16:4222";

			#region testing raise event with sim instrument participant
			var address = "51FCBDDF-f00d-48BD-BBCA-0B85EB1C7EA1";
			SimInstr instrument = new SimInstr(address);
			instrument.connect(nats_server);
			instrument.init();
			#endregion

			#region create a manager
			Manager enterpriseManager = new SimManager();
			Console.WriteLine("...connecting to " + nats_server);
			enterpriseManager.connect(nats_server);
			enterpriseManager.init();
			Console.WriteLine("...connected to " + nats_server);
			Console.WriteLine("...ecomms Manager running id = " + enterpriseManager.id);

			#endregion

		}
	}
}
