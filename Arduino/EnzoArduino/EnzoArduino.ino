// RFID library
#include <SPI.h>
#include <MFRC522.h>

// DHT/Temp & humidity libraries
#include <DHT.h>
#include <DHT_U.h>

// Barometer libraries
#include <SPL06-007.h>
#include <Wire.h>

// Pin definitions
#define RST_PIN 5
#define SS_PIN 53
#define DHT_TYPE DHT11
#define DHT_PIN 40

// Create instances
MFRC522 mfrc522(SS_PIN, RST_PIN);
DHT_Unified dht(DHT_PIN, DHT_TYPE);

unsigned long previousMillis = 0;
const long interval = 1000; // 1 second interval

void setup() {
  Serial.begin(9600);
  
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
  unsigned long currentMillis = millis();

  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;

    // Read and send sensor data
    readTemperature();
    readHumidity();
    readPressure();
  }

  // Check for RFID card
  if (mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial()) {
    String uidString = "card_uid: ";
    for (byte i = 0; i < mfrc522.uid.size; i++) {
      uidString += String(mfrc522.uid.uidByte[i], DEC);
    }
    mfrc522.PICC_HaltA();
    Serial.println(uidString);
  }
}

void readTemperature() {
  sensors_event_t event;
  dht.temperature().getEvent(&event);
  if (!isnan(event.temperature)) {
    Serial.print("temperature: ");
    Serial.println(event.temperature);
  }
}

void readHumidity() {
  sensors_event_t event;
  dht.humidity().getEvent(&event);
  if (!isnan(event.relative_humidity)) {
    Serial.print("humidity: ");
    Serial.println(event.relative_humidity);
  }
}

void readPressure() {
  float pressure = get_pressure();
  if (!isnan(pressure)) {
    Serial.print("pressure: ");
    Serial.println(pressure);
  }
}