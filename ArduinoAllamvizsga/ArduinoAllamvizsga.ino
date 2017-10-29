#include <ESP8266WiFi.h>

const char* ssid = "ESP8266";
const char* password = "password";

const char* host = "192.168.137.1";
String ipAddress;

IPAddress ip(192,168,137,137);
IPAddress gateway(192,168,0,1);
IPAddress subnet(255,255,255,0);

const int httpPort = 5100;
WiFiServer server(5200);

WiFiClient client;


void receiveData();
void transmissionData();


void setup() {
  Serial.begin(115200);
  delay(100);
  
  WiFi.begin(ssid, password);
WiFi.config(ip, gateway, subnet);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(50);
    Serial.print(".");
  }
  
  while (!client.connect(host, httpPort)){Serial.println("INCA NU");}
  delay(50);
  ipAddress=WiFi.localIP().toString();
  client.println(ipAddress);
  delay(100);
  Serial.println("Connected!");
  server.begin();

}


int t1;
int t2;
void loop()
{
  receiveData();
  //Serial.println("Recei end");
  transmissionData();
  //Serial.println("Trans end");
}

char dataR[200];
int counterR;

void receiveData()///////// de la client
{
   WiFiClient clientR = server.available();
  if (clientR) 
  {
    counterR=0;
    delay(50);
    while(clientR.connected())
    {
      while(clientR.available()!=0)
      {
        char byteQ=(char) clientR.read();
        dataR[counterR]=byteQ;
        counterR++;
      }
    }
  
    if(counterR>0)
    {
      clientR.flush();
      dataR[counterR]='\0';
      Serial.println(dataR);
    }
    clientR.stop();
  }
}

char data[200];
int counter;
int nr;
void transmissionData()
{
  if(Serial.available()!=0)
  {
    if(client.connect(host, httpPort))
    {
      delay(10);
      counter=0;
      while(Serial.available()!=0)
      {
         char byteQ=(char) Serial.read();
         data[counter]=byteQ;
         counter++;
      }
      data[counter]='\0';
      if(counter>0)
      {
        client.println(data);
        Serial.flush();
       // Serial.println(data);
      }
      client.stop();
    }
  }
}
