using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using SmartMirror.Models;
using System.Collections.Generic;

namespace SmartMirror.Services
{
    public class MqttService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MqttService> _logger;
        private ClientWebSocket _webSocket;
        private readonly List<SensorData> _sensorDataList = new List<SensorData>();
        private readonly List<CardData> _cardDataList = new List<CardData>();
        private CancellationTokenSource _cancellationTokenSource;
        private ThresholdConfig _thresholdConfig = new ThresholdConfig();

        public MqttService(IConfiguration configuration, ILogger<MqttService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _ = ConnectAsync();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _webSocket?.Dispose();
            return Task.CompletedTask;
        }

        private async Task ConnectAsync()
        {
            _webSocket = new ClientWebSocket();
            Console.WriteLine("peepoopeepoo");
            Console.WriteLine(_configuration["Thingsboard:Server"]);
            var uri = new Uri($"ws://{_configuration["Thingsboard:Server"]}/api/ws");

            try
            {
                _logger.LogInformation("Attempting to connect to WebSocket at {Uri}", uri);
                await _webSocket.ConnectAsync(uri, _cancellationTokenSource.Token);
                _logger.LogInformation("WebSocket connection established.");
                await AuthenticateWebSocket();
                await ReceiveMessages();
            }
            catch (Exception ex)
            {
                _logger.LogError($"WebSocket connection failed: {ex.Message}");
            }
        }

        private async Task AuthenticateWebSocket()
        {
            var loginUrl = $"https://{_configuration["Thingsboard:Server"]}/api/auth/login";
            var loginData = new
            {
                username = _configuration["Thingsboard:Username"],
                password = _configuration["Thingsboard:Password"]
            };

            using (var httpClient = new HttpClient())
            {
                var loginDataJson = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(loginDataJson, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(loginUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<LoginResult>(responseContent);

                var token = loginResult.Token;

                var authMessage = new
                {
                    authCmd = new
                    {
                        cmdId = 0,
                        token = token
                    },
                    cmds = new[]
                    {
                        new
                        {
                            cmdId = 1,
                            entityType = "DEVICE",
                            entityId = "72200690-10f8-11ef-bef7-5131a0fcf1e6",
                            scope = "LATEST_TELEMETRY",
                            type = "TIMESERIES"
                        },
                        new
                        {
                            cmdId = 2,
                            entityType = "DEVICE",
                            entityId = "e67d5010-1323-11ef-9e10-4570a5104a0d",
                            scope = "LATEST_TELEMETRY",
                            type = "TIMESERIES"
                        },
                        new
                        {
                            cmdId = 3,
                            entityType = "DEVICE",
                            entityId = "d36fd010-182d-11ef-ba49-7338e27601c4",
                            scope = "SERVER_SCOPE",
                            type = "ATTRIBUTES"
                        },
                    }
                };

                var authMessageString = JsonConvert.SerializeObject(authMessage);
                var authMessageBuffer = Encoding.UTF8.GetBytes(authMessageString);
                await _webSocket.SendAsync(new ArraySegment<byte>(authMessageBuffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
                _logger.LogInformation("WebSocket authentication message sent.");
            }
        }

        private async Task ReceiveMessages()
        {
            var buffer = new byte[1024 * 4];

            while (_webSocket.State == WebSocketState.Open && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                try
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                }
                catch (WebSocketException ex)
                {
                    _logger.LogError($"WebSocket receive error: {ex.Message}");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _cancellationTokenSource.Token);
                    _logger.LogInformation("WebSocket connection closed.");
                }
                else
                {
                    HandleReceivedMessage(message);
                }
            }
        }

        private void HandleReceivedMessage(string message)
        {
            _logger.LogInformation($"Message received: {message}");

            try
            {
                var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(message);

                if (jsonResponse?.Data == null) return;

                switch (jsonResponse)
                {
                    case { Data.Temperature: not null, Data.LightLevel: not null }:
                        AddSensorData(jsonResponse);
                        break;
                    case { Data.CardUid: not null }:
                        AddCardData(jsonResponse);
                        break;
                    default:
                        UpdateThresholdConfig(jsonResponse.Data);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        }

        private void AddSensorData(JsonResponse jsonResponse)
        {
            var sensorData = new SensorData
            {
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(jsonResponse.LatestValues.Temperature).UtcDateTime,
                Temperature = float.Parse(jsonResponse.Data.Temperature[0][1].ToString()),
                LightLevel = float.Parse(jsonResponse.Data.LightLevel[0][1].ToString())
            };

            _sensorDataList.Add(sensorData);
            _logger.LogInformation($"Sensor data added: {JsonConvert.SerializeObject(sensorData)}");
        }

        private void AddCardData(JsonResponse jsonResponse)
        {
            var cardData = new CardData
            {
                Timestamp = DateTime.UtcNow,
                CardUid = jsonResponse.Data.CardUid[0][1].ToString()
            };

            _cardDataList.Add(cardData);
            _logger.LogInformation($"Card data added: {JsonConvert.SerializeObject(cardData)}");
        }

        private void UpdateThresholdConfig(DataContainer data)
        {
            if (data.TemperatureThreshold != null)
            {
                _thresholdConfig.TemperatureThreshold = float.Parse(data.TemperatureThreshold[0][1].ToString());
                _logger.LogInformation($"Temperature threshold updated: {_thresholdConfig.TemperatureThreshold}");
            }

            if (data.LightLevelThreshold != null)
            {
                _thresholdConfig.LightLevelThreshold = float.Parse(data.LightLevelThreshold[0][1].ToString());
                _logger.LogInformation($"Light level threshold updated: {_thresholdConfig.LightLevelThreshold}");
            }
        }

        
        public List<Data> GetData()
        {
            var cardData = _cardDataList.LastOrDefault();
            var sensorData = _sensorDataList.LastOrDefault();
            return new List<Data>
            {
                new Data
                {
                    CardData = cardData,
                    SensorData = sensorData
                }
            };
        }

        public ThresholdConfig GetThresholdConfig()
        {
            return _thresholdConfig;
        }

        private class JsonResponse
        {
            public int SubscriptionId { get; set; }
            public int ErrorCode { get; set; }
            public string ErrorMsg { get; set; }
            public DataContainer Data { get; set; }
            public LatestValues LatestValues { get; set; }
        }

        private class DataContainer
        {
            [JsonProperty("light_level")]
            public List<List<object>> LightLevel { get; set; }

            [JsonProperty("temperature")]
            public List<List<object>> Temperature { get; set; }

            [JsonProperty("card_uid")]
            public List<List<object>> CardUid { get; set; }

            [JsonProperty("temperatureThreshold")]
            public List<List<object>> TemperatureThreshold { get; set; }
            
            [JsonProperty("lightLevelThreshold")]
            public List<List<object>> LightLevelThreshold { get; set; }
        }

        private class LatestValues
        {
            [JsonProperty("light_level")]
            public long LightLevel { get; set; }

            [JsonProperty("temperature")]
            public long Temperature { get; set; }
        }

        private class LoginResult
        {
            public string Token { get; set; }
        }
    }

    public class ThresholdConfig
    {
        public float TemperatureThreshold { get; set; }
        public float LightLevelThreshold { get; set; }
    }
}
