using Bambu.NET;
using Bambu.NET.Cloud;

var bambu = new BambuClient("your account", "your password",true);

//Login to Bambu Lab cloud, if use access token, DO NOT need call login
//bambu.LoginCloud().Wait();
Console.WriteLine(bambu.AccessToken);
//Get printer info via bambu lab cloud
var serial = bambu.GetPrinterList().Result.First().DevId;
//Subscribe report message， string is json format
bambu.SubscribeReport(serial, e =>
{
    Console.WriteLine("Recieve mqtt message.");
    Console.WriteLine(e);
}).Wait();

var mqttClient = bambu.GetMqttClient(serial);
//mqtt client should be connected when you subscribe report, if not subscribe, you should connect it first
if (mqttClient.Connected)
{
    //Turn off camera light
    await mqttClient.CameraLightOff();
}

Console.ReadLine();
return 0;