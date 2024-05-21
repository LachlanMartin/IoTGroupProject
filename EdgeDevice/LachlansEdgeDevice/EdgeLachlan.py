import serial
import subprocess

# Configure the serial connection
ser = serial.Serial('/dev/ttyS0', 9600)

while True:
    # Read and decode the serial line
    line = ser.readline().decode('utf-8').strip()
    print(line)
    
    try:
        temp_part, light_part = line.split(" - Light level: ")
        temperature = float(temp_part.split(": ")[1].replace(" C", ""))
        light_level = int(light_part)

        # Define the MQTT message payload with both temperature and light level data
        payload = '{"temperature": 25, "light_level": 300}'

        command = [
            "mosquitto_pub",
            "-d",
            "-q", "1",
            "-h", "mqtt.thingsboard.cloud",
            "-p", "1883",
            "-t", "v1/devices/me/telemetry",
            "-u", "cxu1uyvnupclytc6p3w4",
            "-m", payload
        ]

        # Execute the command
        result = subprocess.run(command, capture_output=True, text=True)

        # Print the output
        print("stdout:", result.stdout)
        print("stderr:", result.stderr)

        # Determine which LED to turn on based on light level threshold
        if light_level < light_threshold:
            ser.write(b"RED\n")
        elif light_level < light_threshold + 100:  # Assumes a range of 100 for the yellow threshold
            ser.write(b"YELLOW\n")
        else:
            ser.write(b"GREEN\n")
            
    except ValueError as e:
        print(f"Error parsing or inserting data: {e}")