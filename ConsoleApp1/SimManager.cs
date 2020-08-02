using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestConsoleApp
{
	internal class SimManager : Manager
	{
		internal SimManager() : base(new SimFactory())
		{
			this.id = "00B52941-0000-0000-0000-C2A2FB6FE0A2";
		}

		public override void init()
		{
			base.init();
			addObserver(new ObserverAdapter(observeClients));
			addObserver(new ObserverAdapterEx(clientUpdate));
		}

		void observeClients(IObservable o, object hint)
		{
			Thread.Sleep(1000);
			switch (hint)
			{
				case "CLIENTS_CHANGED":
					int sensorCount = clients.Where((i) => i.role == Role.Sensor).Count();
					int clientCount = clients.Count();
					Console.WriteLine("there are " + sensorCount + " sensors online " + clientCount + " clients");
					foreach ( var client in clients)
						Console.WriteLine(" Client: " + client.id + " role: " + client.role);
					break;
			}
		}

		public void clientUpdate(IObservableEx observable, object hint, object clientObj)
		{
			//need to wait to notify until after base class has gotton response
			//to role request
			//or have library query first before creating client
			//WIP...
			var client = clientObj as IClient;
			Thread.Sleep(1000);
			Console.WriteLine("...SimManager.clientUpdate: " + hint);

			switch (hint)
			{
				case "CONNECTED":
					//was it an instrument?
					if (client.role == Role.Instrument)
					{
						Console.WriteLine(client.name + " INSTRUMENT CONNECTED");

						//listen for run state changes
						client.addObserver(new ObserverAdapterEx((anobject, ahint, data) =>
						{
							var bytes = (byte[])data;
							var anInstrumentClient = (InstrumentClient)anobject;

							if ((ahint as string) == "RUNSTATE_CHANGED")
							{
								var say = string.Format("{0} notified {1} with {2}",
									client.name,
									ahint,
									Encoding.UTF8.GetString(bytes, 0, bytes.Length)
									);

								Console.WriteLine(say);
							}
						}));

						//add a status listener
						client.addStatusListener((name, data) =>
						{
							Console.WriteLine("status listener: {0}", name);
						});
					}

					if (client.role == Role.Sensor)
					{

						Console.WriteLine(client.name + " SENSOR CONNECTED");

						//listen for run state changes
						client.addObserver(new ObserverAdapterEx((anobject, ahint, data) =>
						{
							Console.WriteLine((ahint as string));
						}));

						//add a status listener
						client.addStatusListener((name, bytes) =>
						{
							Console.WriteLine("status listener: {0}:{1}", name, Encoding.UTF8.GetString(bytes, 0, bytes.Length));
						});
					}
					break;
			}

		}
	}
}
