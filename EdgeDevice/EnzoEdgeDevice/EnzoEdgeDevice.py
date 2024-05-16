import paho.mqtt.client as mqtt
import json
import time

# Thingsboard MQTT configuration
THINGSBOARD_HOST = "mqtt.thingsboard.io"
THINGSBOARD_PORT = 1883
THINGSBOARD_ACCESS_TOKEN = "5fu51w8c9n5mkmd8infj"

# MQTT client initialization
client = mqtt.Client()
client.username_pw_set(THINGSBOARD_ACCESS_TOKEN)
client.connect(THINGSBOARD_HOST, THINGSBOARD_PORT)

# Function to publish data to Thingsboard
def publish_data(temperature, humidity):
    payload = {
        "temperature": temperature,
        "humidity": humidity
    }
    client.publish("v1/devices/me/telemetry", json.dumps(payload))

# Function to retrieve latest data from Thingsboard
def retrieve_latest_data():
    client.subscribe("v1/devices/me/attributes")
    client.on_message = on_message
    client.loop_start()
    time.sleep(1)  # Wait for the data to be received
    client.loop_stop()

# Callback function to handle received messages
def on_message(client, userdata, message):
    data = json.loads(message.payload.decode("utf-8"))
    print("Latest data:")
    print("Temperature:", data["temperature"])
    print("Humidity:", data["humidity"])

# Publish sample data
publish_data(25.5, 60)
publish_data(26.2, 58)
publish_data(24.8, 62)

# Retrieve the latest data
retrieve_latest_data()