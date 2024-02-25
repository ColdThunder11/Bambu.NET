using Bambu.NET.Cloud;

var bambu = new BambuCloud("your account", "your password",true);
bambu.Login();
var name = bambu.GetUserName();
var list = bambu.GetDeviceList();