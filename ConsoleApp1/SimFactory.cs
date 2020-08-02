using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestConsoleApp
{

	class SimFactory : IClientFactory
	{
		/// <summary>
		/// return a participant instance to the manager
		/// </summary>
		/// <param name="address"></param>
		/// <param name="role"></param>
		/// <param name="type"></param>
		/// <param name="subType"></param>
		/// <returns></returns>
		public IClient getClientFor(string address, Role role, ECOMMS_Entity.Type type, SubType subType)
		{
			IClient client = null;
			Console.WriteLine("...SimFactory.getClientFor() address: " + address);
			Console.WriteLine("  role: " + role + " type: " + type + " subType: " + subType);
			switch (role)
			{
				case Role.Instrument:
					client = new SimClient(address);
					break;
				case Role.Sensor:
					client = new SimSensorClient(address);
					break;
				default:
					Console.WriteLine("...create generic client");
					client = new Client(address, role, type);
					break;
			}
			return client;
		}
	}
}
