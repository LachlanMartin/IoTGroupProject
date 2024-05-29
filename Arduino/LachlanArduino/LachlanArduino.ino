#include <OneWire.h>
#include <DallasTemperature.h>

// Data wire is connected to Arduino pin 2
#define ONE_WIRE_BUS 8

// Setup a oneWire instance to communicate with any OneWire devices
OneWire oneWire(ONE_WIRE_BUS);

// Pass our oneWire reference to Dallas Temperature sensor 
DallasTemperature sensors(&oneWire);

const int lightSensorPin = A0; // Photoresistor connected to analog pin A0

void setup() {
  Serial.begin(9600); // Start serial communication at 9600 bps
  sensors.begin(); // Start up the library for the temperature sensor
  cli();                      //stop interrupts for till we make the settings
  /*1. First we reset the control register to make sure we start with everything disabled.*/
  TCCR1A = 0;                 // Reset entire TCCR1A to 0 
  TCCR1B = 0;                 // Reset entire TCCR1B to 0
 
  /*2. We set the prescalar to the desired value by changing the CS10 CS12 and CS12 bits. */  
  TCCR1B |= B00000100;        //Set CS12 to 1 so we get prescalar 256  
  
  /*3. We enable compare match mode on register A*/
  TIMSK1 |= B00000010;        //Set OCIE1A to 1 so we enable compare match A 
  
  /*4. Set the value of register A to 31250*/
  OCR1A = 65535;             //Finally we set compare register A to this value  
  sei();                     //Enable back the interrupts
}

void loop() {
  //REFER TO ISR
}

//With the settings above, this IRS will trigger each 1000ms.
ISR(TIMER1_COMPA_vect){
  TCNT1  = 0;                  //First, set the timer back to 0 so it resets for next interrupt
  
  sensors.requestTemperatures(); // Send the command to get temperatures
  float temperature = sensors.getTempCByIndex(0); // Read temperature in Celsius
  Serial.print("Temperature: ");
  Serial.print(temperature);
  Serial.print(" C  - ");
  
  // Reading from the analog light sensor
  int lightLevel = analogRead(lightSensorPin); // Read the light level (0 to 1023)
  Serial.print("Light level: ");
  Serial.println(lightLevel);
}
