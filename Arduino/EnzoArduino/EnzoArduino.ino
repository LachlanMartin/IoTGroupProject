// RFID library
#include <SPI.h>
#include <MFRC522.h>

// Servo library
#include <Servo.h>

// DHT/Temp & humidity libraries
#include <DHT.h>
#include <DHT_U.h>

// Barometer libraries
#include <SPL06-007.h>
#include <Wire.h>

// Pin definitions
#define RST_PIN 5
#define SS_PIN 53
#define SERVO_PIN 3
#define FAN_PWM 2
#define FAN_ANALOG A0
#define POTENTIOMETER A15
#define DHT_TYPE DHT11
#define DHT_PIN 40

// Create instances
MFRC522 mfrc522(SS_PIN, RST_PIN);
Servo myServo;
DHT_Unified dht(DHT_PIN, DHT_TYPE);

void setup() {
  Serial.begin(9600);

  // Initialize servo
  myServo.attach(SERVO_PIN);

  // Initialize RFID module
  SPI.begin();
  mfrc522.PCD_Init();
  mfrc522.PCD_DumpVersionToSerial();

  // Initialize DHT sensor
  dht.begin();

  // Initialize barometer
  Wire.begin();
  SPL_init();
}

void loop() {
  if (Serial.available()) {
    String instruction = Serial.readStringUntil('\n');
    instruction.trim();

    if (instruction == "read temperature") {
      readTemperature();
    } else if (instruction == "read humidity") {
      readHumidity();
    } else if (instruction == "read pressure") {
      readPressure();
    } else if (instruction == "read card uid") {
      readCardUID();
    } else if (instruction.startsWith("set servo ")) {
      int angle = instruction.substring(10).toInt();
      setServoAngle(angle);
    } else if (instruction.startsWith("set fan ")) {
      int speed = instruction.substring(8).toInt();
      setFanSpeed(speed);
    } else {
      Serial.println("Invalid instruction");
    }
  }
}



void readTemperature() {
  sensors_event_t event;
  dht.temperature().getEvent(&event);

  String output;
  if (!isnan(event.temperature)) {
    output = "Temperature: " + String(event.temperature) + " °C";
  } else {
    output = "Error reading temperature";
  }
  Serial.println(output);
}

void readHumidity() {
  sensors_event_t event;
  dht.humidity().getEvent(&event);

  String output;
  if (!isnan(event.relative_humidity)) {
    output = "Humidity: " + String(event.relative_humidity) + "%";
  } else {
    output = "Error reading humidity";
  }
  Serial.println(output);
}

void readPressure() {
  float pressure = get_pressure();

  String output;
  if (!isnan(pressure)) {
    output = "Pressure: " + String(pressure) + " Pa";
  } else {
    output = "Error reading pressure";
  }
  Serial.println(output);
}

void readCardUID() {
  String output;
  if (mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial()) {
    String uidString = "";
    for (byte i = 0; i < mfrc522.uid.size; i++) {
      uidString += String(mfrc522.uid.uidByte[i], DEC);
    }
    mfrc522.PICC_HaltA();

    output = "Card UID: " + uidString;
  } else {
    output = "No card detected";
  }
  Serial.println(output);
}

void setServoAngle(int angle) {
  myServo.write(angle);
  String output = "Servo angle set to " + String(angle);
  Serial.println(output);
}

void setFanSpeed(int speed) {
  analogWrite(FAN_PWM, speed);
  String output = "Fan speed set to " + String(speed);
  Serial.println(output);
}