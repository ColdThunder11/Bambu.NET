using System.Security.Authentication;
using System.Text;
using MQTTnet;
using MQTTnet.Client;

namespace Bambu.NET.MQTT;

public class BambuMQTTClient : IDisposable
{
    private string _host;
    private int _port;
    private string _userName;
    private string _password;
    private string _serial;
    private bool _connected;

    private Action<string> _reportCallback;

    private MqttFactory _mqttFactory;
    private IMqttClient _mqttClient;

    public bool Connected => _connected;
    
    
    
    public BambuMQTTClient(string host, int port, string userName, string password, string serial)
    {
        this._host = host;
        this._port = port;
        this._userName = userName;
        this._password = password;
        this._serial = serial;
    }

    public async Task Connect()
    {
        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(_host, _port)
            .WithTlsOptions(o =>
            {
                o.WithCertificateValidationHandler(_ => true);
                o.WithSslProtocols(SslProtocols.Tls12);
            })
            .WithCredentials(_userName, _password).Build();
        _mqttClient.DisconnectedAsync += async e =>
        {
            if (e.ClientWasConnected)
            {
                await _mqttClient.ConnectAsync(_mqttClient.Options);
            }
        };
        // Setup message handling before connecting so that queued messages
        // are also handled properly. When there is no event handler attached all
        // received messages get lost.
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            //Console.WriteLine("Received application message.");
            var payload = e.ApplicationMessage.PayloadSegment.ToArray();
            var payloadStr = Encoding.UTF8.GetString(payload);
            ProcessDeviceReport(payloadStr);
            return Task.CompletedTask;
        };

        await _mqttClient.ConnectAsync(mqttClientOptions);
    }

    private void ProcessDeviceReport(string payload)
    {
        if (_reportCallback != null)
        {
            _reportCallback(payload);
        }
    }
    

    public async Task Subscribe(Action<string> callback)
    {
        _reportCallback = callback;
        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f =>
                {
                    f.WithTopic($"device/{_serial}/report");
                })
            .Build();
        await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        await Publish("{\"info\": {\"sequence_id\": \"0\", \"command\": \"get_version\"}}");
        await Publish("{\"pushing\": {\"sequence_id\": \"0\", \"command\": \"pushall\"}}");
    }

    public async Task<bool> Publish(string payload)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic($"device/{_serial}/request")
            .WithPayload(payload)
            .Build();
        var result = await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        return result.IsSuccess;
    }

    public async Task Disconnect()
    {
        var mqttClientDisconnectOptions = _mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

        await _mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
    }

    public void Dispose()
    {
        if (_mqttClient != null)
        {
            _mqttClient.Dispose();
        }
    }
    
    
    
}