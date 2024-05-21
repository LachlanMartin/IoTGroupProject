# Smart Mirror Dashboard

The Smart Mirror Dashboard is a web-based application that provides a user-friendly interface for monitoring and controlling a smart mirror system. It allows users to view real-time sensor data, such as temperature and motion detection, and customize the system's settings according to their preferences.

## Features

- Real-time display of temperature and motion sensor data
- Customizable temperature threshold for triggering actions
- Configurable motion detection sensitivity
- Adjustable update interval for sensor data refresh
- Intuitive settings pane for easy configuration
- Integration with Arduino for sensor data acquisition and control

## Technologies Used

- HTML, CSS, and JavaScript for the web interface
- Bootstrap for responsive design and styling
- C# and ASP.NET Core for the server-side API
- Arduino for sensor data collection and actuator control

## Getting Started

To run the Smart Mirror Dashboard locally, follow these steps:

1. Clone the repository:
   ```
   git clone https://github.com/your-username/smart-mirror-dashboard.git
   ```

2. Set up the Arduino:
    - Connect the necessary sensors (temperature and motion) to the Arduino board
    - Upload the Arduino sketch to the board (provided in the `arduino` directory)

3. Configure the server:
    - Open the `SmartMirror` solution in your preferred IDE
    - Update the `appsettings.json` file with your database connection string
    - Run the database migrations to create the necessary tables
    - Build and run the server application

4. Configure the web interface:
    - Open the `index.html` file in a web browser
    - Ensure that the API endpoints in the JavaScript code match the server's URL and port

5. Start monitoring and controlling your smart mirror:
    - The dashboard will display the current temperature and motion status
    - Adjust the settings using the settings pane to customize the behavior of the smart mirror

## Arduino Setup

To set up the Arduino for sensor data collection and actuator control, follow these steps:

1. Connect the temperature sensor (e.g., DHT11) and motion sensor (e.g., PIR) to the appropriate pins on the Arduino board
2. Open the Arduino sketch provided in the `arduino` directory
3. Customize the pin assignments and sensor configurations according to your setup
4. Upload the sketch to the Arduino board

## Acknowledgements

- [Bootstrap](https://getbootstrap.com/) for the responsive web framework
- [Arduino](https://www.arduino.cc/) for the open-source electronics platform
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) for the server-side web framework