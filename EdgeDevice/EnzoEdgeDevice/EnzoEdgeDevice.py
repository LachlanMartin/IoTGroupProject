import tkinter as tk
from tkinter import ttk
import serial
import serial.tools.list_ports
import paho.mqtt.client as mqtt
import json

class SerialMQTTCommunicator(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("Serial MQTT Communicator")
        self.geometry("800x600")
        self.ports = serial.tools.list_ports.comports()
        self.port_list = [str(port) for port in self.ports]
        self.create_widgets()
        
        # MQTT configuration
        self.THINGSBOARD_HOST = "mqtt.thingsboard.cloud"
        self.THINGSBOARD_PORT = 1883
        self.THINGSBOARD_ACCESS_TOKEN = "5fu51w8c9n5mkmd8infj"
        
        # MQTT client initialization
        self.mqtt_client = mqtt.Client()
        self.mqtt_client.username_pw_set(self.THINGSBOARD_ACCESS_TOKEN)
        self.mqtt_client.connect(self.THINGSBOARD_HOST, self.THINGSBOARD_PORT)
        self.mqtt_client.loop_start()
        
        # Sensor data
        self.temperature = 0
        self.humidity = 0
        self.pressure = 0
        
    def create_widgets(self):
        # Port selection dropdown
        port_label = tk.Label(self, text="Select Port:")
        port_label.pack(pady=5)
        self.port_var = tk.StringVar()
        port_dropdown = ttk.Combobox(self, textvariable=self.port_var, values=self.port_list)
        port_dropdown.pack(pady=5)
        
        # Data display area
        data_label = tk.Label(self, text="Received Data:")
        data_label.pack(pady=5)
        self.data_text = tk.Text(self, height=10, wrap=tk.WORD)
        self.data_text.pack(side=tk.TOP, fill=tk.BOTH, expand=True)
        
        # Serial instance
        self.serial_inst = None
        
        # Configure the window to resize the Text widget
        self.bind("<Configure>", self.resize_text)
        
    def resize_text(self, event):
        text_width = event.width - 20
        text_height = event.height - 150
        self.data_text.config(width=text_width, height=text_height)
        
    def update_data(self):
        if self.serial_inst and self.serial_inst.is_open:
            if self.serial_inst.in_waiting:
                packet = self.serial_inst.readline()
                data = packet.decode('utf').rstrip('\n')
                self.data_text.insert(tk.END, data + '\n')
                self.data_text.see(tk.END)
                self.process_data(data)
        
        self.after(100, self.update_data)
        
    def process_data(self, data):
        if data.startswith("card_uid:"):
            card_uid = data.split(":")[1].strip()
            self.publish_data({"card_uid": card_uid})
        elif data.startswith("temperature:"):
            self.temperature = float(data.split(":")[1].strip())
        elif data.startswith("humidity:"):
            self.humidity = float(data.split(":")[1].strip())
        elif data.startswith("pressure:"):
            self.pressure = float(data.split(":")[1].strip())
        
        # Publish sensor data every second
        self.publish_sensor_data()
        
    def publish_data(self, payload):
        self.mqtt_client.publish("v1/devices/me/telemetry", json.dumps(payload))
        
    def publish_sensor_data(self):
        payload = {
            "temperature": self.temperature,
            "humidity": self.humidity,
            "pressure": self.pressure
        }
        self.mqtt_client.publish("v1/devices/me/telemetry", json.dumps(payload))
        self.after(1000, self.publish_sensor_data)
        
    def start_serial(self, event=None):
        port_var = self.port_var.get()
        if port_var:
            self.serial_inst = serial.Serial()
            self.serial_inst.baudrate = 9600
            self.serial_inst.port = port_var.split(' ')[0]  # Extract the port name from the string
            try:
                self.serial_inst.open()
                self.update_data()
            except serial.SerialException as e:
                print(f"Error opening serial port: {e}")

if __name__ == "__main__":
    app = SerialMQTTCommunicator()
    app.port_var.trace_add("write", lambda *args: app.start_serial())
    app.mainloop()