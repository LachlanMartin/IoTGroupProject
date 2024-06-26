using System.IO.Ports;
using System.Text;
using Newtonsoft.Json;
using SmartMirror.Models;
using SmartMirror.Services;

public class ArduinoService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ArduinoService> _logger;
    private SerialPort _serialPort;
    private Timer _timer;
    private string _lastCommand;

    public ArduinoService(IServiceProvider serviceProvider, ILogger<ArduinoService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _serialPort = new SerialPort("/dev/ttyACM0", 9600);
        // _serialPort = new SerialPort("/dev/cu.usbmodem2101", 9600);
        _serialPort.Open();

        // Set up a timer to periodically check the filtered data
        _timer = new Timer(SendCommands, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        _serialPort.Close();
        _serialPort.Dispose();

        return Task.CompletedTask;
    }

    private void SendCommands(object state)
    {
        using var scope = _serviceProvider.CreateScope();
        var mqttService = scope.ServiceProvider.GetRequiredService<MqttService>();
        var data = mqttService.GetData();
        var thresholdConfig = mqttService.GetThresholdConfig();
        var now = DateTime.UtcNow;

        var temperatureData = data.FirstOrDefault(d => d?.SensorData?.Temperature > thresholdConfig?.TemperatureThreshold);
        var lightLevelData = data.FirstOrDefault(d => d?.SensorData?.LightLevel < thresholdConfig?.LightLevelThreshold);
        var cardData = data.FirstOrDefault(d => d?.CardData?.Timestamp > now.AddSeconds(-5));

        var motorState = temperatureData != null;
        var ledState = lightLevelData != null;
        var piezoState = cardData != null;

        var command = new
        {
            motor = motorState,
            led = ledState,
            piezo = piezoState
        };

        var jsonCommand = JsonConvert.SerializeObject(command);
        _logger.LogInformation($"Generated command: {jsonCommand}");

        // Check if the new command is different from the last command
        if (jsonCommand != _lastCommand)
        {
            _serialPort.WriteLine(jsonCommand);
            _logger.LogInformation($"Sent command to Arduino: {jsonCommand}");
            _lastCommand = jsonCommand; // Update the last command
        }
    }
}
