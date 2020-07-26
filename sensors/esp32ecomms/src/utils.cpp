#include <Arduino.h>
#include <WiFiClient.h>
#include <WiFi.h>
#include <ArduinoNATS.h>
#include <credentials.h>

WiFiClient _client;
NATS _nats(&_client, nats_server, nats_port, nats_user, nats_password);

String ip2string(IPAddress ip)
{
    String ret = String(ip[0]) + "." + String(ip[1]) + "." + String(ip[2]) + "." + String(ip[3]);
    return ret;
}

int init_wifi(const char *ssid, const char *password, const char *hostname)
{
    WiFi.disconnect(true);
    WiFi.config(INADDR_NONE, INADDR_NONE, INADDR_NONE);
    WiFi.mode(WIFI_STA);
    WiFi.setHostname(hostname);
    WiFi.begin(ssid, password);

    printf("Connecting to Wifi");
    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        printf(".");
    }
    printf("  Connected to '%s'\n", ssid);

    String ipaddr = ip2string(WiFi.localIP());
    printf("IP address   : %s\r\n", ipaddr.c_str());
    printf("MAC address  : %s \r\n", WiFi.macAddress().c_str());
    return 1;
}

int init_wifi()
{
    return init_wifi(wifi_ssid, wifi_password, host_name);
}