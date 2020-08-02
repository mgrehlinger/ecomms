using ECOMMS_Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestConsoleApp
{
    /// <summary>
    /// example of how to create an instrument client
    /// </summary>
    class SimClient : InstrumentClient
    {
        public SimClient(string address) : base(address, ECOMMS_Entity.Type.Thermal)
        {
            Console.WriteLine("SimClient.ctor() id = " + id);
        }

        /// <summary>
        /// override to do something special in the instrument client
        /// when a status is received - user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public override void statusReceived(string name, byte[] data)
        {
            Console.WriteLine("SimClient.status() name = " + name);
            //base class instrument client will notify of RUNSTATE_CHANGED
            //do something here that is required for this derived client
            //maybe look for states that are specific to the derived client
            //and notify
            base.statusReceived(name, data);
        }
    }

}
