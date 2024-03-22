using System.Security.Authentication;
using System.Text;
using Bambu.NET.Models;
using Bambu.NET.Utils;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Bambu.NET.MQTT;

public class BambuMQTTClient : IDisposable
{
    private string _host;
    private int _port;
    private string _userName;
    private string _password;
    private string _serial;

    private Action<string> _reportCallback;

    private MqttFactory _mqttFactory;
    private IMqttClient _mqttClient;
    private BambuPrinterStatus _status;
    private BambuVersionInfo _version;

    public bool Connected => _mqttClient.IsConnected;

    private static DefaultContractResolver _snakeCaseNameContractResolver;

    private static JsonSerializerSettings _snakeCaseNameSetting;

    static BambuMQTTClient()
    {
        _snakeCaseNameContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        _snakeCaseNameSetting = new JsonSerializerSettings()
        {
            ContractResolver = _snakeCaseNameContractResolver,
            Formatting = Formatting.None
        };
    }

    public BambuPrinterStatus Status
    {
        get
        {
            if (_status == null) return null;
            lock (_status)
            {
                return (BambuPrinterStatus) _status.Clone();
            }
        }
    }

    public BambuVersionInfo VersionInfo => _version;
    
    public BambuMQTTClient(string host, int port, string userName, string password, string serial)
    {
        this._host = host;
        this._port = port;
        this._userName = userName;
        this._password = password;
        this._serial = serial;
        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();
    }

    public async Task Connect()
    {
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
        //Update status
        var payloadJo = JsonConvert.DeserializeObject<JObject>(payload);
        if (payloadJo.ContainsKey("print"))
        {
            var printJo = payloadJo["print"].ToObject<JObject>();
            if (_status == null)
            {
                _status = FieldNameCast.BambuJson2Model<BambuPrinterStatus>(printJo);
            }
            else
            {
                lock (_status)
                {
                    _status = FieldNameCast.UpdateBambuJson2Model<BambuPrinterStatus>(printJo, _status);
                }
            }
        }
        if (payloadJo.ContainsKey("info"))
        {
            var versionJo = payloadJo["info"].ToObject<JObject>();
            _version = FieldNameCast.BambuJson2Model<BambuVersionInfo>(versionJo);
        }
        if (_reportCallback != null)
        {
            _reportCallback(payload);
        }
    }

    /// <summary>
    /// Pause current task
    /// </summary>
    public async Task Pause()
    {
        await Publish("{\"print\": {\"sequence_id\": \"0\", \"command\": \"pause\"}}");
    }
    
    /// <summary>
    /// Stop current task
    /// </summary>
    public async Task Stop()
    {
        await Publish("{\"print\": {\"sequence_id\": \"0\", \"command\": \"stop\"}}");
    }
    
    /// <summary>
    /// Resume current task
    /// </summary>
    public async Task Resume()
    {
        await Publish("{\"print\": {\"sequence_id\": \"0\", \"command\": \"resume\"}}");
    }

    /// <summary>
    /// Turn on camera light
    /// </summary>
    public async Task CameraLightOn()
    {
        await Publish("{\"system\": {\"sequence_id\": \"0\", \"command\": \"ledctrl\", \"led_node\": \"chamber_light\", \"led_mode\": \"on\",\"led_on_time\": 500, \"led_off_time\": 500, \"loop_times\": 0, \"interval_time\": 0}}");
    }
    
    /// <summary>
    /// Turn off camera light
    /// </summary>
    public async Task CameraLightOff()
    {
        await Publish("{\"system\": {\"sequence_id\": \"0\", \"command\": \"ledctrl\", \"led_node\": \"chamber_light\", \"led_mode\": \"off\",\"led_on_time\": 500, \"led_off_time\": 500, \"loop_times\": 0, \"interval_time\": 0}}");
    }
    
    /// <summary>
    /// Tells printer to perform a filament change using AMS.
    /// </summary>
    /// <param name="filamentTrayId">ID of filament tray</param>
    /// <param name="currentTemp">Old print temperature</param>
    /// <param name="newTemp">New print temperature</param>
    public async Task AmsChangeFilament(int filamentTrayId, int currentTemp, int newTemp)
    {
        var data = new AmsChangeFilamentData()
        {
            SequenceId = "0",
            Command = "ams_change_filament",
            Target = filamentTrayId,
            CurrTemp = currentTemp,
            TarTemp = newTemp
        };
        var dataJson = JsonConvert.SerializeObject(data, _snakeCaseNameSetting);
        await Publish($"{{\"print\": {dataJson}}}");
    }
    
    /// <summary>
    /// Changes the setting of the given filament in the given AMS.
    /// </summary>
    /// <param name="amsId">Index of the AMS</param>
    /// <param name="trayId">Index of the tray</param>
    /// <param name="trayColor">Formatted as hex RRGGBBAA (alpha is always FF)</param>
    /// <param name="nozzleTempMin">Minimum nozzle temp for filament (in C)</param>
    /// <param name="nozzleTempMax">Maximum nozzle temp for filament (in C)</param>
    /// <param name="trayType">Type of filament, such as "PLA" or "ABS"</param>
    public async Task AmsFilamentSetting(int amsId, int trayId, string trayColor, int nozzleTempMin, int nozzleTempMax, string trayType)
    {
        var data = new AmsFilamentSettingData()
        {
            SequenceId = "0",
            Command = "ams_filament_setting",
            AmsId = amsId,
            TrayId = trayId,
            TrayInfoIdx = "",
            TrayColor = trayColor,
            NozzleTempMin = nozzleTempMin,
            NozzleTempMax = nozzleTempMax,
            TrayType = trayType
        };
        var dataJson = JsonConvert.SerializeObject(data, _snakeCaseNameSetting);
        await Publish($"{{\"print\": {dataJson}}}");
    }
    /// <summary>
    /// Set print speed to one of the 4 presets.
    /// </summary>
    /// <param name="speed">1 = silent, 2 = standard, 3 = sport, 4 = ludicrous</param>
    public async Task SetPrintSpeed(int speed)
    {
        await Publish($"{{\"print\": {{\"sequence_id\": \"0\", \"command\": \"print_speed\", \"param\": \"{speed}\"}}}}");
    }
    
    /// <summary>
    /// Print a gcode file. This takes absolute paths.
    /// </summary>
    /// <param name="filename">Filename (on the printer's filesystem) to print</param>
    public async Task PrintGcodeFile(string filename)
    {
        await Publish($"{{\"print\": {{\"sequence_id\": \"0\", \"command\": \"gcode_file\", \"param\": \"{filename}\"}}}}");
    }
    /// <summary>
    /// Send raw GCode to the printer.
    /// </summary>
    /// <param name="gcode">Gcode to execute, can use \n for multiple lines</param>
    /// <param name="userId">Optional</param>
    public async Task SendGcode(string gcode ,string? userId = null)
    {
        if (userId == null)
        {
            await Publish($"{{\"print\": {{\"sequence_id\": \"0\", \"command\": \"gcode_line\", \"param\": \"{gcode}\"}}}}");
        }
        else
        {
            await Publish($"{{\"print\": {{\"sequence_id\": \"0\", \"command\": \"gcode_line\", \"param\": \"{gcode}\", \"user_id\": \"{userId}\"}}}}");
        }
    }
    
    /// <summary>
    /// Starts calibration process.
    /// </summary>
    public async Task Calibration()
    {
        await Publish("{\"print\": {\"sequence_id\": \"0\", \"command\": \"calibration\"}}");
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
    
    public async Task<bool> Publish(Object jsonObject)
    {
        return await Publish(JsonConvert.SerializeObject(jsonObject));
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
            if (_mqttClient.IsConnected)
            {
                _mqttClient.DisconnectAsync().Wait();
            }
            _mqttClient.Dispose();
        }
    }
    
    
    
}