// Initializes the `instruments` service on path `/instruments`
const createService = require('feathers-memory');
const hooks = require('./sensors.hooks');

///////////////////////////////////////////////////////////////////////////////
var _sensors = {};

const Service = require('feathers-memory').Service;

class SensorsService extends Service {
  constructor(options, nats)
  {
    super(options);

    this.nats = nats;
    this.answer = "an answer";
  }

  create(data, params) {
    data.created_at = new Date();

    return super.create(data, params);
  }

  get(id, params){
    //console.log('service get:' + id);
    //return super.get(id, params);

    // this.nats.request(id + '.get', params.query.data, {'max':1},
    // function(data)
    // {
    //   //console.log("get returned:" + data);
    //   this.answer =  data;
    // });

    // return Promise.resolve({
    //   id,
    //   data: this.answer,
    // });

    return new Promise(
      (resolve, reject) => {
        this.nats.request(id + '.get', params.query.data, {'max':1},
        function(data)
        {
          //console.log("get returned:" + data);
          this.answer =  data;
          resolve(data);
        });
      }
    );
  }

  update(id,data,params){
    if(params)
    {
      //console.log(params);
      if(params.query)
      {
        ////console.log(params.query);
        switch(params.query.action){
          case 'action':
            this.nats.request(id + '.action', params.query.data, {'max':1},
            function(status)
            {
              //console.log(status);
            });
            break;
          case 'set':
            break;
        }
      }
    }

    return super.update(id, data, params);
  }
}

module.exports = function (app) {

  const paginate = app.get('paginate');

  const options = {
    paginate
  };

  ///////////////////////////////////////////////////////////////////////////////
  //NATS INTERFACE
  var NATS = require('nats');

  //get the nats url from the app default.json
  var nats = NATS.connect(app.get('nats_url'));
  //console.log(app.get('nats_url'));
  ///////////////////////////////////////////////////////////////////////////////

  // Initialize our service with any options it requires
  //app.use('/instruments', createService(options));
  app.use('/sensors', new SensorsService(options, nats));

  // Get our initialized service so that we can register hooks
  const service = app.service('sensors');

  service.hooks(hooks);

  //console.log('starting up');

  // Simple Subscriber
  nats.subscribe('heartbeat', function(heartbeat) {

    if(!(heartbeat in _sensors))
    {
      console.log(heartbeat + ': not found');

      //create entry in our list to check against
      //could just query the service ( me )
      _sensors[heartbeat] =
        {
          id:heartbeat,
          lastAdvertisement: new Date(),
          name: '---',
          role: '',
          type: '---',
          subtype: '---',
          location: '---',
          currentTemperature: 0,
          signals:[],
          signalNames:[],
          status: ''
        }

      //add object here to avoid requesting role for instruments we have

      //get the participants role
      nats.request(heartbeat + '.get', 'role', {'max':1},
      function(role)
      {
        //test
        //if(role == 'Service')
        //{
        //  nats.request(heartbeat + '.get', 'name', {'max':1},
        //  function(name)
        //  {
        //    //console.log('name: ' + name);
        //  });
        //}

        //if its an instrument get its name and location
        //and create an entry in the instruments dictionary
        if(role == 'Sensor')
        {
          _sensors[heartbeat] =
            {
              id:heartbeat,
              lastAdvertisement: new Date(),
              name: '---',
              role: 'Sensor',
              type: '---',
              subtype: '---',
              location: '---',
              currentTemperature: 0,
              signals:[],
              signalNames:[],
              status:''
            }

          service.create(_sensors[heartbeat]);

          nats.request(heartbeat + '.get', 'name', {'max':1},
          function(name)
          {
            //console.log('name: ' + name);

            _sensors[heartbeat].name = name;

            service.update(heartbeat, _sensors[heartbeat]);
          });

          nats.request(heartbeat + '.get', 'type', {'max':1},
          function(type)
          {
            //console.log('type: ' + type);

            _sensors[heartbeat].type = type;

            service.update(heartbeat, _sensors[heartbeat]);
          });

          nats.request(heartbeat + '.get', 'subtype', {'max':1},
          function(subtype)
          {
            //console.log('subtype: ' + subtype);

            _sensors[heartbeat].subtype = subtype;

            service.update(heartbeat, _sensors[heartbeat]);
          });

          nats.request(heartbeat + '.get', 'location', {'max':1},
          function(location)
          {
            //console.log('location: ' + location);

            _sensors[heartbeat].location = location;

            service.update(heartbeat, _sensors[heartbeat]);
          });

          // nats.request(heartbeat + '.get', 'realtimesignalnames', {'max':1},
          // function(bytes)
          // {
          //   var names = signalNames_pb.MercuryRealTimeSignalNamesPB.deserializeBinary(bytes);
          //
          //   _sensors[heartbeat].signalNames = names.getNamesList();
          // });

          nats.subscribe(heartbeat + '.status',(data)=> {
            try
            {
              _sensors[heartbeat].status = data;
              service.update(heartbeat, _sensors[heartbeat]);
            }
            catch(err)
            {
              //console.log(err);
            }
          });
        }
      });

    }
    else
    {
      //update lastAdvertisement
      _sensors[heartbeat].lastAdvertisement = new Date();

      //console.log(_sensors[heartbeat]);
    }
  });

  //remove stale heartbeats
  function staleAdvertisementCheck() {

    setTimeout(function()
    {
      var now = new Date();

      for (var hb in _sensors)
      {

        ////console.log(mac);
        ////console.log(now - instrumentAdvertisementDict[mac]);

        if( (now - _sensors[hb].lastAdvertisement) > 4000)
        {
          //remove stale advertisement
          //console.log('removing stale heartbeat:' + hb + 'with last: ' + _sensors[hb].lastAdvertisement);

          service.remove(hb);

          delete _sensors[hb];

          //delete instrumentAdvertisementDict[mac];
          //delete instrumentInfoDict[mac];

          //for(var index in instrumentSubscriptions[mac].subscriptions)
          //{
          //  nats.unsubscribe(instrumentSubscriptions[mac].subscriptions[index])
          //}

          //instrumentSubscriptions[mac].subscriptions.forEach(function(sid)
          //{
          //  nats.unsubscribe(sid)
          //});

          //delete instrumentSubscriptions[mac];

          //itemService.remove(mac);
        }
      }
      staleAdvertisementCheck();
    },
    1000);
  }

  //////////////
  staleAdvertisementCheck();
  // Simple Publisher
  //nats.publish('foo', 'hello cruel world');
};
