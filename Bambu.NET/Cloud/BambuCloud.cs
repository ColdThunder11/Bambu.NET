using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Bambu.NET.Models;
using Bambu.NET.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bambu.NET.Cloud;

public class BambuCloud
{
    private bool isChinaRegion = true;
    private string account;
    private string password;
    private string accessToken;
    public string AccessToken => accessToken;
    
    public BambuCloud(string account, string password, bool isChinaRegion)
    {
        this.account = account;
        this.password = password;
        this.isChinaRegion = isChinaRegion;
    }
    
    public BambuCloud(string account, string password, string accessToken, bool isChinaRegion):
        this(account,password,isChinaRegion)
    {
        this.accessToken = accessToken;
    }

    public void Login()
    {
        LoginAsync().Wait();
    }

    public async Task LoginAsync()
    {
        var loginData = new Dictionary<string, string>()
        {
            { "account", account },
            { "password", password }
        };
        var result = await RequestAsync<JObject>("v1/user-service/user/login", loginData);
        var token = result["accessToken"].ToString();
        accessToken = token;
    }

    public async Task<List<BambuPrinter>> GetDeviceListAsync()
    {
        var result = await RequestAsync<JObject>("v1/iot-service/api/user/bind");
        var deviceListJArray = result["devices"].ToObject<JArray>();
        var deviceList = new List<BambuPrinter>();
        foreach (var deviceJo in deviceListJArray)
        {
            var device = FieldNameCast.BambuJson2Model<BambuPrinter>(deviceJo.ToObject<JObject>());
            deviceList.Add(device);
        }
        return deviceList;
    }
    public List<BambuPrinter> GetDeviceList()
    {
        return GetDeviceListAsync().Result;
    }

    public async Task<List<BambuTask>> GetTaskListAsync()
    {
        var result = await RequestAsync<JObject>("v1/user-service/my/tasks");
        var taskListJArray = result["hits"].ToObject<JArray>();
        var taskList = new List<BambuTask>();
        foreach (var taskJo in taskListJArray)
        {
            var task = FieldNameCast.BambuJson2Model<BambuTask>(taskJo.ToObject<JObject>());
            taskList.Add(task);
        }
        return taskList;
    }

    public List<BambuTask> GetTaskList()
    {
        return GetTaskListAsync().Result;
    }
    
    public string GetUserName()
    {
        var b64Str = accessToken.Split(".")[1];
        b64Str = b64Str.PadRight((4 - b64Str.Length % 4) % 4, '=');
        var b64Bytes = Convert.FromBase64String(b64Str);
        var jsonStr = Encoding.UTF8.GetString(b64Bytes);
        var userName = JsonConvert.DeserializeObject<JObject>(jsonStr)["username"].ToString();
        return userName;
    }

    public async Task<T> RequestAsync<T>(string apiEndpoint, Object? data = null)
    {
        var json = JsonConvert.SerializeObject(data);
        var url = GetApiUrl(apiEndpoint);
        HttpClient client = new HttpClient();
        if (accessToken != null)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                "Authorization",$"Bearer {accessToken}");
        }

        HttpResponseMessage result;
        if (data == null)
        {
            result = await client.GetAsync(url);
        }
        else
        {
            result = await client.PostAsync(url, 
                new StringContent(json, Encoding.UTF8, "application/json"));
        }
        var respCode = result.StatusCode;
        if (respCode != HttpStatusCode.OK)
        {
            throw new Exception($"Request to {url} return a status code {respCode.ToString()}");
        }
        return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
    }

    private string GetApiUrl(string apiPath)
    {
        if (isChinaRegion) return $"https://api.bambulab.cn/{apiPath}";
        return $"https://api.bambulab.com/{apiPath}";
    }
}