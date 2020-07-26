
#include <BaseParticipant.h>
#include <ArduinoJson.h>

BaseParticipant::BaseParticipant(NATS *nats, String id, String name, String location)
{
  _ssn = 0;
  _nats = nats;
  _id = id;
  _name = name;
  _location = location;
  _led_pin = LED_BUILTIN;
}

void BaseParticipant::init()
{
}

void BaseParticipant::setup()
{
  pinMode(_led_pin, OUTPUT);
  digitalWrite(_led_pin, LOW);
}

bool BaseParticipant::processHeartbeat()
{
  _nats->publish("heartbeat", _id.c_str());
  digitalWrite(_led_pin, HIGH);
  delay(100);
  digitalWrite(_led_pin, LOW);
  return false;
}

bool BaseParticipant::processNats()
{
  _nats->process();
  return false;
}

bool BaseParticipant::processSensor()
{
  DynamicJsonDocument doc(50);
  char payload[200];
  doc["ssn"] = _ssn;
  serializeJson(doc, payload);
  String topic = _id + ".status.base";
  _nats->publish(topic.c_str(), payload);
  _ssn++;
  return false;
}


void BaseParticipant::onGet(NATS::msg msg)
{

  String reply = "unknown";
  String query = String(msg.data);


  if (query == "name")
    reply = _name;
  else if ( query == "location")
    reply = _location;
  else if ( query == "role")
    reply = "myrole";
  // TODO: others

  printf("onGet: %s = %s\n", query.c_str(), reply.c_str());
  _nats->publish(msg.reply, reply);
}
