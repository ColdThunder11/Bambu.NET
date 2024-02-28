using Bambu.NET.Cloud;
using Bambu.NET.Models;
using Bambu.NET.MQTT;

namespace Bambu.NET;

public class BambuClient
{
    private BambuCloud _bambuCloud;
    private Dictionary<string, BambuMQTTClient> _bambuMqttClient = new ();

    private List<BambuPrinter> _deviceListCache;

    private string mqttHost;

    public string AccessToken => _bambuCloud.AccessToken;
    
    public BambuClient(string account, string password, bool isChinaRegion)
    {
        _bambuCloud = new BambuCloud(account, password, isChinaRegion);
        if (isChinaRegion) mqttHost = "cn.mqtt.bambulab.com";
        else mqttHost = "us.mqtt.bambulab.com";
    }

    /// <summary>
    /// login to cloud and pull necessary info for mqtt
    /// </summary>
    public async Task LoginCloud()
    {
        await _bambuCloud.LoginAsync();
        _deviceListCache = await _bambuCloud.GetDeviceListAsync();
    }

    public async Task<List<BambuPrinter>> GetPrinterList()
    {
        _deviceListCache = await _bambuCloud.GetDeviceListAsync();
        return _deviceListCache.Select(x=>x).ToList();
    }

    public BambuMQTTClient GetMqttClient(BambuPrinter printer)
    {
        return GetMqttClient(printer.DevId);
    }

    public BambuMQTTClient GetMqttClient(string serial)
    {
        if (_deviceListCache.Select(x => x.DevId == serial).Count() == 0)
        {
            return null;
        }
        if (!_bambuMqttClient.ContainsKey(serial))
        {
            var mqtt = new BambuMQTTClient(mqttHost, 8883, _bambuCloud.GetUserName(),
                _bambuCloud.AccessToken, serial);
            _bambuMqttClient.Add(serial,mqtt);
        }

        return _bambuMqttClient[serial];
    }

    public async Task SubscribeReport(BambuPrinter printer, Action<string> callback)
    {
        await SubscribeReport(printer.DevId, callback);
    }

    public async Task SubscribeReport(string serial, Action<string> callback)
    {
        var mqttClient = GetMqttClient(serial);
        if(!mqttClient.Connected) await mqttClient.Connect();
        await mqttClient.Subscribe(callback);
    }
}