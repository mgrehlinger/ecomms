using ECOMMS_Participant;
using NATS.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	internal class SimInstr : InstrumentParticipant
	{

		public enum InstrRunState
			{
			Unknown,
			Online,
			Idle,
			Start,
			Run,
			Stop,
			};


		public SimInstr(string address) : base(address, ECOMMS_Entity.Type.Rheology)   //our sim is a rheology instrument
		{
			RunState = InstrRunState.Unknown;
			location = "nowhere";
			name = "sne";
		}

		public override void init()
		{
			base.init();
			Task.Factory.StartNew(new Action(doWork));
/*
			Task.Factory.StartNew(() =>
			{
				//simulate a status
				while (true)
				{
					Thread.Sleep(1000);
					sendStatus("instrument.runstate", "Idle");

					//try raising an event
					raise("INSTRUMENT_RUNSTATE_CHANGED");
				}

			});
*/
			//register action facility
			//you are expected to fulfill ack/nak/response protocol
			registerActionFacility("control.autosampler", (what, args) =>
			{
				switch (what)
				{
					case "openlid":
						Console.WriteLine("*** asked to openlid!!!");
						ack(args.Message);
						Console.WriteLine("*** opening!!!");
						Thread.Sleep(5000);
						Console.WriteLine("*** done opening!!!");
						replyTo(args.Message, "SUCCESS");
						break;
				}

			});
		}

		public override void get(string what, Msg message)
		{
			Console.WriteLine("...SimInstr.get(" + what +")");
			switch (what)
			{
				case "location":
					//get this through the instrument interface
					replyTo(message, location);
					break;
				case "location.now":
					//get this through the instrument interface
					ack(message);
					replyTo(message, location);
					break;
				case "name":
					//get this through the instrument interface
					replyTo(message, name);
					break;
				case "runstate":
					replyTo(message, RunState.ToString());
					break;
				default:
					//not handled
					base.get(what, message);
					break;
			}
		}

		public override void set(string what, string payload, Msg message)
		{
			switch (what)
			{
				case "location":
					//set through instrument interface
					location = payload;

					//always reply with status
					replyTo(message, "success");
					break;
				default:
					//not handled
					base.set(what, payload, message);
					break;
			}
		}

		public override void action(string what, Msg message)
		{
			bool bSuccess = false;
			switch (what)
			{
				case "start":
					//always reply with status
					if ( RunState == InstrRunState.Idle)
						{
						RunState = InstrRunState.Start;
						bSuccess = true;
						}
					break;
				case "end":
					//always reply with status
					if ( RunState == InstrRunState.Run)
						{
						RunState = InstrRunState.Stop;
						bSuccess = true;
						}
					break;
				default:
					base.action(what, message);
					break;
			}
			replyTo(message, bSuccess ? "success" : "failure");

		}


		// Worker thread for status messages
		private void doWork()
			{
				while (true)
				{
					Console.WriteLine("...SimInstr.doWork()");
					sendStatus("instrument.runstate", "Idle");
					//try raising an event
					if ( RunState == InstrRunState.Start)
						RunState = InstrRunState.Run;
					else if ( RunState == InstrRunState.Stop)
						RunState = InstrRunState.Idle;
					Thread.Sleep(3000);
				}
			}

		private InstrRunState _runState;
		private object _runStateLock = new object();

		public InstrRunState RunState
			{
				get { lock(_runStateLock) { return _runState; } }
				private set
				{
				lock(_runStateLock)
					{
					_runState = value;
					raise("INSTRUMENT_RUNSTATE_CHANGED");
					}
				}
			}

	}

}
