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

    public async Task SubscribeReport(string serial, Action<string> callback)
    {
        if (!_bambuMqttClient.ContainsKey(serial))
        {
            var mqtt = new BambuMQTTClient(mqttHost, 8883, _bambuCloud.GetUserName(),
                _bambuCloud.AccessToken, serial);
            _bambuMqttClient.Add(serial,mqtt);
        }

        var mqttClient = _bambuMqttClient[serial];
        if(!mqttClient.Connected) await mqttClient.Connect();
        await mqttClient.Subscribe(callback);
    }
}