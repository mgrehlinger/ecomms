
#include <BaseParticipant.h>


  BaseParticipant::BaseParticipant(NATS *nats, String id, String name, String location)
  {
    _nats = nats;
    _id = id;
    _name = name;
    _location = location;
  }

  void BaseParticipant::init()
  {
  }

  void BaseParticipant::onGet(NATS::msg msg)
  {
    printf("onGet:");
    printf(msg.data);

    if (String(msg.data) == String("name"))
    {
      printf("posting reply\n");
      _nats->publish(msg.reply, "sensor1");
    }
  }
