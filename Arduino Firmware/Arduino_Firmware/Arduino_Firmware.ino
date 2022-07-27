// Include libraries
#include <WiFi.h>
#include <HTTPClient.h>
#include <PubSubClient.h>

// Specify the pins
const int LED_PIN = 22;
const int POT_PIN = 39;

// Specify the WiFi connection
const char* ssid = "SSID";
const char* password = "password";

// Specify the MQTT broker
const char* mqttServer = "192.168.1.1";
const char* mqttUser = "user";
const char* mqttPassword = "password";
const int mqttPort = 1883;

// WIFi connection
WiFiClient espClient;
PubSubClient client(espClient);

// the last time the output pin was toggled
unsigned long lastDebounceTime = 0;
// the debounce time; increase if the output flickers 
unsigned long debounceDelay = 50;
// the current reading from the input pin
int buttonState;
// the previous reading from the input pin      
int lastButtonState = LOW;
// the current state of the output pin
int ledState = LOW;   

// Internal state
int pot_value = 0;
float pot_voltage = 0;
float pot_position = 0.0;
float pot_lastPosition = 0.0;

// number of readings
const int numReadings = 10;
// the readings from the analog input
float readings[numReadings];
// the index of the current reading
int readIndex = 0;
// the running total           
float total = 0.0;
// the average               
float average = 0.0;   

             
bool analogLastButtonStatePushed = false;

void callback(char* topic, byte* payload, unsigned int length) {
 
  Serial.print("Message arrived in topic: ");
  Serial.println(topic);
 
  Serial.print("Message:");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
 
  Serial.println();
  Serial.println("-----------------------");
}

void setup()
{
  for (int thisReading = 0; thisReading < numReadings; thisReading++) {
    readings[thisReading] = 0;
  }

  pinMode(LED_PIN, OUTPUT);

  Serial.begin(115200);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  
  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
    digitalWrite(LED_PIN, HIGH);
    delay(500);
    digitalWrite(LED_PIN, LOW);
  }
  
  Serial.print("WiFi connected with IP: ");
  Serial.println(WiFi.localIP());

  client.setServer(mqttServer, mqttPort);
  client.setCallback(callback);

  while (!client.connected()) {
    Serial.println("Connecting to MQTT...");

    if (client.connect(mqttUser, mqttUser, mqttPassword )) {

      Serial.println("connected");
      digitalWrite(LED_PIN, HIGH);

    } else {

      Serial.print("failed with state ");
      Serial.print(client.state());
      delay(2000);
    }
  }
}

void loop()
{
  digitalWrite(LED_PIN, HIGH);
  client.loop();

  pot_value = analogRead(POT_PIN);
  pot_voltage = (3.3 / 4095.0) * pot_value;
  pot_position = pot_value / 4095.0;

  // subtract the last reading:
  total = total - readings[readIndex];
  
  // read from the sensor:
  readings[readIndex] = pot_position;
  
  // add the reading to the total:
  total = total + readings[readIndex];
  
  // advance to the next position in the array:
  readIndex = readIndex + 1;

  // if we're at the end of the array...
  if (readIndex >= numReadings) {
    // ...wrap around to the beginning:
    readIndex = 0;
  }

  // calculate the average:
  average = total / numReadings;

  Serial.print(" last Pot Posiiton:");
  Serial.println(pot_lastPosition);
  Serial.print(" Average:");
  Serial.println(average);
  
  if( abs(average - pot_lastPosition) > 0.01){
     client.publish("esp/Slider", String(average).c_str());
     pot_lastPosition = average;
  }

  digitalWrite(LED_PIN, HIGH);
  delay(20); // delay in between reads for stability
}

void button_pressed()
{
  Serial.print("Press");
  digitalWrite(LED_PIN, HIGH);
  client.publish("debug", "Hello from ESP32");

}
