using Bambu.NET;
using Bambu.NET.Cloud;

var bambu = new BambuClient("your account", "your password",true);

bambu.LoginCloud().Wait();
var serial = bambu.GetPrinterList().Result.First().DevId;
bambu.SubscribeReport(serial, e =>
{
    Console.WriteLine("Recieve mqtt message.");
    Console.WriteLine(e);
}).Wait();
Console.ReadLine();
return 0;