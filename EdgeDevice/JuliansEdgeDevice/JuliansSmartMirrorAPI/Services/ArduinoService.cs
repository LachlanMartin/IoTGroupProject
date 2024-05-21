using System.IO.Ports;
using Newtonsoft.Json;
using SmartMirror.Models;

public class ArduinoService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private SerialPort _serialPort;

    public ArduinoService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _serialPort = new SerialPort("/dev/ttyACM0", 9600);
        _serialPort.Open();
        _serialPort.DataReceived += SerialPort_DataReceived;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _serialPort.Close();
        _serialPort.Dispose();

        return Task.CompletedTask;
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var data = _serialPort.ReadLine();
        try
        {
            var sensorData = JsonConvert.DeserializeObject<SensorData>(data);
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SmartMirrorContext>();
            context.SensorData.Add(sensorData);
            context.SaveChanges();
        }
        catch (JsonException)
        {
        }
    }

    public void SendConditionalRuleToArduino(ConditionalRule rule)
    {
        var json = JsonConvert.SerializeObject(new
        {
            u = rule.UpdateInterval,
            t = rule.TemperatureThreshold,
        });

        _serialPort.WriteLine(json);
    }
}