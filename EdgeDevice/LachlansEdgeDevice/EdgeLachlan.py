import serial
import paho.mqtt.client as mqtt
import json
import time

# Configure the serial connection
ser = serial.Serial('/dev/ttyS0', 9600)

# Thingsboard MQTT configuration
THINGSBOARD_HOST = "mqtt.thingsboard.io"
THINGSBOARD_PORT = 1883
THINGSBOARD_ACCESS_TOKEN = "YOUR_ACCESS_TOKEN"

# MQTT client initialization
client = mqtt.Client()
client.username_pw_set(THINGSBOARD_ACCESS_TOKEN)
client.connect(THINGSBOARD_HOST, THINGSBOARD_PORT)

# Function to publish data to Thingsboard
def publish_data(temperature, light_level):
    payload = {
        "temperature": temperature,
        "lightIntensity": light_level
    }
    client.publish("v1/devices/me/telemetry", json.dumps(payload))

# Function to handle received messages from Thingsboard
def on_message(client, userdata, message):
    command = message.payload.decode("utf-8")
    # Parse the command and perform the corresponding action
    if command == "TRIGGER_BUZZER":
        ser.write(b"BUZZER\n")
    elif command == "ADJUST_LED":
        ser.write(b"LED\n")
    # Send a response back to Thingsboard
    client.publish("v1/devices/me/attributes", json.dumps({"ackCommand": command}))

# Subscribe to the command topic from Thingsboard
client.subscribe("v1/devices/me/rpc/request/+")
client.on_message = on_message

# Start the MQTT loop
client.loop_start()

while True:
    # Read and decode the serial line
    line = ser.readline().decode('utf-8').strip()
    print(line)

    try:
        temp_part, light_part = line.split(" - Light level: ")
        temperature = float(temp_part.split(": ")[1].replace(" C", ""))
        light_level = int(light_part)

        # Publish the sensor data to Thingsboard
        publish_data(temperature, light_level)

        # Determine which LED to turn on based on light level threshold
        if light_level < 100:
            ser.write(b"RED\n")
        elif light_level < 200:
            ser.write(b"YELLOW\n")
        else:
            ser.write(b"GREEN\n")

    except ValueError as e:
        print(f"Error parsing data: {e}")

    # Delay to avoid overwhelming the serial connection and Thingsboard
    time.sleep(1)
