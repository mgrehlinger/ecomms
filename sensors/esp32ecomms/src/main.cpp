#include <Arduino.h>
#include <WiFiClient.h>
#include <WiFi.h>
#include <Eventually.h>
#include <ArduinoJson.h>
#include <utils.h>
#include <BaseParticipant.h>

EvtManager mgr;

String ID = "8d1da580-dead-beef-f00d-1ce525e4fa24";
String NAME = "ecomms1";
String LOCATION = "office";
BaseParticipant ecommsOne(&_nats, ID, NAME, LOCATION);
bool processHeartbeat() { return ecommsOne.processHeartbeat(); }
bool processNats()      { return ecommsOne.processNats();}
bool processSensor()    { return ecommsOne.processSensor();}
void nats_get_handler(NATS::msg msg) {ecommsOne.onGet(msg);}

void nats_on_connect() 
{
  String topic = ID + ".get";
  printf("...subscribing to %s\n", topic.c_str());
  _nats.subscribe(topic.c_str(), nats_get_handler);
}

void setup()
{
  init_wifi();
  ecommsOne.setup();
  mgr.addListener(new EvtTimeListener(1000, true, (EvtAction)processNats));
  mgr.addListener(new EvtTimeListener(2000, true, (EvtAction)processHeartbeat));
  mgr.addListener(new EvtTimeListener(2000, true, (EvtAction)processSensor));
  _nats.on_connect = nats_on_connect;
  bool bConnect = _nats.connect();
  printf("nats connect: %s\n", bConnect ? "true" : "false");
}

USE_EVENTUALLY_LOOP(mgr) // replaces loop()
