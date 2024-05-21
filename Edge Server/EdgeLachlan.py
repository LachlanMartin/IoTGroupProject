import serial
import mariadb
import sys
import time

# Configure the serial connection
ser = serial.Serial('/dev/ttyS0', 9600)
# How often to refresh settings in seconds
refresh_rate = 60
last_refresh_time = time.time()

# Function to get threshold values from the database
def get_thresholds(cur):
    cur.execute("SELECT temperature_threshold, light_threshold FROM Settings ORDER BY id DESC LIMIT 1")
    row = cur.fetchone()
    if row:
        return row[0], row[1]
    else:
        print("No threshold settings found. Using default values.")
        return 25.00, 100  # Default threshold values

# Try to connect to the database
try:
    conn = mariadb.connect(user="root", password="password123", host="127.0.0.1", port=3306, database="iot_db")
except mariadb.Error as e:
    print(f"There is an issue connecting to the database: {e}")
    sys.exit(1)

# Create a cursor object
cur = conn.cursor()

# Fetch the current threshold settings from the database
temperature_threshold, light_threshold = get_thresholds(cur)

while True:
    # Read and decode the serial line
    line = ser.readline().decode('utf-8').strip()
    print(line)

    # Fetch the latest threshold settings if needed
    current_time = time.time()
    if current_time - last_refresh_time > refresh_rate:
        temperature_threshold, light_threshold = get_thresholds(cur)
        last_refresh_time = current_time
    
    try:
        temp_part, light_part = line.split(" - Light level: ")
        temperature = float(temp_part.split(": ")[1].replace(" C", ""))
        light_level = int(light_part)
        
        # Insert the parsed data into the database
        cur.execute("INSERT INTO SensorData (temperature, light_level) VALUES (?, ?)", (temperature, light_level))
        conn.commit()

        # Fetch the latest threshold settings if needed (for example, periodically or based on some condition)
        # temperature_threshold, light_threshold = get_thresholds(cur)

        # Determine which LED to turn on based on light level threshold
        if light_level < light_threshold:
            ser.write(b"RED\n")
        elif light_level < light_threshold + 100:  # Assumes a range of 100 for the yellow threshold
            ser.write(b"YELLOW\n")
        else:
            ser.write(b"GREEN\n")
            
    except ValueError as e:
        print(f"Error parsing or inserting data: {e}")