using Bambu.NET;
using Bambu.NET.Cloud;

//var bambu = new BambuClient("your account", "your password",true);
var bambu = new BambuClient("18358854397", "LSlyj8292616", "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJQUGZuRjNKbEY4eG9sRnhzQmhhYlN5cWp3b3puWi1LRUFPS1Z1Qy1yV2EwIn0.eyJleHAiOjE3MTcwNzg3NjksImlhdCI6MTcwOTMwMjc2OSwianRpIjoiZmM0Y2Q1ODQtZWNkNy00NjM5LThkMzMtMTQ2ZTdiZjY0NzllIiwiaXNzIjoiaHR0cDovL2tleWNsb2FrLWh0dHAua2V5Y2xvYWstcHJvZC1jbi9hdXRoL3JlYWxtcy9iYmwiLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiYzY4NTQyYWItMTc3YS00MmZmLWI4MTEtYzU2OGVhMzIyNjI0IiwidHlwIjoiQmVhcmVyIiwiYXpwIjoidXNlci1zZXJ2aWNlIiwic2Vzc2lvbl9zdGF0ZSI6IjE0MmIxZmM2LWZlOTMtNDAyMi04MDFiLTMwZmI2NWU5MTkwOSIsInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsImRlZmF1bHQtcm9sZXMtYmJsIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6ImVtYWlsIHByb2ZpbGUiLCJzaWQiOiIxNDJiMWZjNi1mZTkzLTQwMjItODAxYi0zMGZiNjVlOTE5MDkiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsInByZWZlcnJlZF91c2VybmFtZSI6IjE1MzY4OTQ3NzkiLCJ1c2VybmFtZSI6InVfMTUzNjg5NDc3OSJ9.eGzcXaOJJ_hWJ4caf4KMUDERoau7Elvc7HBlmX9HgwqCg5HtXXF4mo1BQ8Nlp8pOjruepHTKFbWc-hLfDvw5S4_f89voyl-O3BPFHWKYQKZvQhp3L-NFgkjjTNp6xs4QNwyaSEHCDuQhn3U7KCDMfVk2mXLSWhOrZ5pgMTyPhDjh1ylRhYQ_K-6Xqd-DV5R7xkMGWF-m_mfRUJgCpmpS59OQttnY1-oqiR03FIx3JPv_edJ0iZQYO81imSl-xi5dt4L3M9WNXEkVvxseRKGWUak08Tl26X2MgZZaJhujn_upik25Uzbro5tGa_JwTjqoWYQmMBXZrjCRgMlo-iIcqA",true);

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
    await mqttClient.AmsFilamentSetting(0, 2, "GFG99", "00000000", 220, 270, "PETG");
    //await mqttClient.AmsFilamentSetting(0, 3, "GFL99", "00000000", 190, 240, "PLA");
}

Console.ReadLine();
return 0;