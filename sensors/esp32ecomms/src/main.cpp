#include <Arduino.h>
#include <WiFiClient.h>
#include <WiFi.h>
#include <Eventually.h>
#include <ArduinoJson.h>
#include <utils.h>
#include <BaseParticipant.h>


String ID = "8d1da580-dead-beef-90fc-1ce525e4fa24";
String NAME = "ecomms1";
String LOCATION = "office";
BaseParticipant ecommsOne(&_nats, ID, NAME, LOCATION);

void setup()
{
  init_wifi();
}

int cnt = 0;
void loop()
{
  printf("loop: %d\n", cnt++);
  delay(1000);
}