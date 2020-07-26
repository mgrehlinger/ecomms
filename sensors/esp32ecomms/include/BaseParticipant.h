#include <ArduinoNATS.h>
#include <Eventually.h>

#ifndef BASE_PARTICICANT_DEF
#define BASE_PARTICIPANT_DEF

class BaseParticipant
{

public:
    BaseParticipant(NATS *nats, String id, String name, String location);

    void init();
    void setup();
    void onGet(NATS::msg msg);

public:
    virtual bool processHeartbeat();
    virtual bool processNats();
    virtual bool processSensor();

protected:
    int _ssn;
    int _led_pin;
    NATS *_nats;
    String _id;
    String _name;
    String _location;
};
#endif