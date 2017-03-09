#include <ESP8266WiFi.h>

const char* ssid = "Megkur-LAK";
const char* password = "nevetkozz";

//Variables
int chk;
int incomingByte = 0;
String data = "";
String szoveg = "Helloka";
String str;
WiFiServer server(80);

void setup() {
  Serial.begin(115200);
  delay(10);

  // Connect to WiFi network
  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(500);

  }
  Serial.println("");
  Serial.println("WiFi connected");

  // Start the server
  server.begin();
  Serial.println("Server started");

  // Print the IP address
  Serial.print("Use this URL to connect: ");
  Serial.print("http://");
  Serial.print(WiFi.localIP());
  Serial.println("/");
}

void loop() {

  // Check if a client has connected
  WiFiClient client = server.available();
  if (!client) {
    return;
  }
  if (client) {
    //Serial.println("Szerussz");
    //client.write(szoveg.length());
    data = client.readStringUntil('\r');
    Serial.println(data);
    //if (Serial.available() > 0) {
      //incomingByte = Serial.read();
      str = Serial.readStringUntil('\r');
      client.println(str);

      
      //client.println(data);
    //}
  }


  delay(1);

}
