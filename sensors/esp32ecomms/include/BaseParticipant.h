#include <ArduinoNATS.h>

class BaseParticipant
{

public:
    BaseParticipant(NATS *nats, String id, String name, String location);

    void init();
    void onGet(NATS::msg msg);

protected:
    NATS *_nats;
    String _id;
    String _name;
    String _location;
};