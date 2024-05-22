#include <ArduinoJson.h>

// Define pins for actuators
const int piezoPin = 3;
const int motorPin = 4;
const int ledPin = 5;

void setup() {
  // Initialize serial communication
  Serial.begin(9600);

  // Set pin modes for actuators
  pinMode(piezoPin, OUTPUT);
  pinMode(motorPin, OUTPUT);
  pinMode(ledPin, OUTPUT);

  // Initialize actuators to off state
  digitalWrite(motorPin, LOW);
  digitalWrite(ledPin, LOW);
  noTone(piezoPin);
}

void loop() {
  // Check if there are any incoming commands from Raspberry Pi
  if (Serial.available()) {
    String json = Serial.readString();
    processCommand(json);
  }
  // Small delay to avoid overwhelming the serial buffer
  delay(100);
}

// Function to process incoming commands from Raspberry Pi
void processCommand(String json) {
  // Deserialize the JSON payload
  StaticJsonDocument<200> doc;
  DeserializationError error = deserializeJson(doc, json);

  if (error) {
    // If deserialization fails, print an error message
    Serial.print("Deserialization failed: ");
    Serial.println(error.c_str());
    return;
  }

  // Control motor based on command
  if (doc.containsKey("motor")) {
    bool motorState = doc["motor"];
    digitalWrite(motorPin, motorState ? HIGH : LOW);
  }

  // Control LED based on command
  if (doc.containsKey("led")) {
    bool ledState = doc["led"];
    Serial.println(ledState);
    digitalWrite(ledPin, ledState == 1 ? HIGH : LOW);
  }

  // Control piezo based on command
  if (doc.containsKey("piezo")) {
    bool piezoState = doc["piezo"];
    if (piezoState) {
      tone(piezoPin, 1000);  // Play a tone of 1000Hz
    } else {
      noTone(piezoPin);  // Stop playing the tone
    }
  }
}
