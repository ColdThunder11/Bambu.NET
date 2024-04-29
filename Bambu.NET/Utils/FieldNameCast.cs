using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bambu.NET.Utils;

public class FieldNameCast
{
    public static string BambuJson2CsPublic(string orig)
    {
        if (orig.Contains("_"))
        {
            int index = 0;
            bool nextNeedBeUpper = true;
            var sb = new StringBuilder();
            while (index < orig.Length)
            {
                if (orig[index] == '_')
                {
                    nextNeedBeUpper = true;
                    index++;
                    continue;
                }

                if (nextNeedBeUpper)
                {
                    nextNeedBeUpper = false;
                    sb.Append(char.ToUpper(orig[index++]));
                    continue;
                }

                sb.Append(orig[index++]);
            }
            return sb.ToString();
        }
        else
        {
            return orig[0..1].ToUpper() + orig[1..];
        }
    }

    /// <summary>
    /// Internal use to build class type on runtime, you should use generic one
    /// </summary>
    /// <param name="type"></param>
    /// <param name="jtoken"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static Object BambuJson2Model(Type type, JToken jtoken)
    {
        if (type == typeof(object))
        {
            return (object) jtoken;
        }
        if (type == typeof(string))
        {
            return jtoken.ToObject<string>();
        }
        else if (type == typeof(bool))
        {
            return jtoken.ToObject<bool>();
        }
        else if (type == typeof(int))
        {
            return jtoken.ToObject<int>();
        }
        else if (type == typeof(long))
        {
            return jtoken.ToObject<long>();
        }
        else if (type == typeof(float))
        {
            return jtoken.ToObject<float>();
        }
        else if (type == typeof(double))
        {
            return jtoken.ToObject<double>();
        }
        var jDict = jtoken.ToObject<Dictionary<string, Object>>();
        var t = Activator.CreateInstance(type);
        foreach (var kv in jDict)
        {
            var key = BambuJson2CsPublic(kv.Key);
            var prop = type.GetProperty(key);
            if (prop != null)
            {
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(t, kv.Value.ToString());
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(t, bool.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(t, int.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(long))
                {
                    prop.SetValue(t, long.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(float))
                {
                    prop.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(double))
                {
                    prop.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType.IsEnum)
                {
                    var enmuParsed = Enum.Parse(prop.PropertyType, kv.Value.ToString());
                    prop.SetValue(t, enmuParsed);
                }
                else if (kv.Value.GetType() == typeof(JToken))
                {
                    var item = BambuJson2Model(prop.PropertyType, (JToken)kv.Value);
                    prop.SetValue(t, item);
                }
                else if (kv.Value.GetType() == typeof(JArray))
                {
                    var propertyType = prop.PropertyType;
                    var interfaces = propertyType.GetInterfaces();
                    Type itemType = null;
                    foreach (var interf in interfaces)
                    {
                        if (interf.IsGenericType &&
                            interf.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            itemType = interf.GetGenericArguments()[0];
                        }
                    }
                    var ja = (JArray)kv.Value;
                    if (itemType == null)
                    {
                        throw new Exception($"Target type of {prop.Name} not support");
                    }
                    var classInst = Activator.CreateInstance(propertyType);
                    var addMethod = propertyType.GetMethod("Add");
                    foreach (var jt in ja)
                    {
                        var item = BambuJson2Model(itemType, jt);
                        addMethod.Invoke(classInst, new object[] { item });
                    }
                    prop.SetValue(t, classInst);
                }
                else
                {
                    try
                    {
                        prop.SetValue(t, kv.Value);
                    }
                    catch
                    {
                        throw new Exception($"Target type of {prop.Name} not support");
                    }
                }
            }
        }
        return t;
    }
    
    public static T BambuJson2Model<T>(string json) where T : new()
    {
        return BambuJson2Model<T>(JsonConvert.DeserializeObject<JObject>(json));
    }
    public static T BambuJson2Model<T>(JObject jo) where T : new()
    {
        var jDict = jo.ToObject<Dictionary<string, Object>>();
        var t = new T();
        foreach (var kv in jDict)
        {
            var key = BambuJson2CsPublic(kv.Key);
            var prop = typeof(T).GetProperty(key);
            if (prop != null)
            {
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(t, kv.Value.ToString());
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(t, bool.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(t, int.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(long))
                {
                    prop.SetValue(t, long.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(float))
                {
                    prop.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(double))
                {
                    prop.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType.IsEnum)
                {
                    var enmuParsed = Enum.Parse(prop.PropertyType, kv.Value.ToString());
                    prop.SetValue(t, enmuParsed);
                }
                else if (kv.Value.GetType() == typeof(JToken))
                {
                    var item = BambuJson2Model(prop.PropertyType, (JToken)kv.Value);
                    prop.SetValue(t, item);
                }
                else if (kv.Value.GetType() == typeof(JObject))
                {
                    var item = BambuJson2Model(prop.PropertyType, ((JObject)kv.Value).ToObject<JToken>());
                    prop.SetValue(t, item);
                }
                else if (kv.Value.GetType() == typeof(JArray))
                {
                    var propertyType = prop.PropertyType;
                    var interfaces = propertyType.GetInterfaces();
                    Type itemType = null;
                    foreach (var interf in interfaces)
                    {
                        if (interf.IsGenericType &&
                            interf.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            itemType = interf.GetGenericArguments()[0];
                        }
                    }
                    var ja = (JArray)kv.Value;
                    if (itemType == null)
                    {
                        throw new Exception($"Target type of {prop.Name} not support");
                    }
                    var classInst = Activator.CreateInstance(propertyType);
                    var addMethod = propertyType.GetMethod("Add");
                    foreach (var jt in ja)
                    {
                        var item = BambuJson2Model(itemType, jt);
                        addMethod.Invoke(classInst, new object[] { item });
                    }
                    prop.SetValue(t, classInst);
                }
                else
                {
                    try
                    {
                        prop.SetValue(t, kv.Value);
                    }
                    catch
                    {
                        throw new Exception($"Target type of {prop.Name} not support");
                    }
                }
            }
        }

        return t;
    }

    private static Object UpdateBambuJson2Model(Type type, JToken jtoken, Object model)
    {
        var jDict = jtoken.ToObject<Dictionary<string, Object>>();
        var t = model;
        foreach (var kv in jDict)
        {
            var key = BambuJson2CsPublic(kv.Key);
            var prop = type.GetProperty(key);
            if (prop != null)
            {
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(t, kv.Value.ToString());
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(t, bool.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(t, int.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(long))
                {
                    prop.SetValue(t, long.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(float))
                {
                    prop.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(double))
                {
                    prop.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (kv.Value.GetType() == typeof(JToken))
                {
                    if (prop.GetValue(t) != null)
                    {
                        var item = UpdateBambuJson2Model(prop.PropertyType, (JToken) kv.Value, prop.GetValue(t));
                        prop.SetValue(t, item);
                    }
                    else
                    {
                        var item = BambuJson2Model(prop.PropertyType, (JToken)kv.Value);
                        prop.SetValue(t, item);
                    }
                }
                else if (kv.Value.GetType() == typeof(JObject))
                {
                    if (prop.GetValue(t) != null)
                    {
                        var item = UpdateBambuJson2Model(prop.PropertyType, ((JObject)kv.Value).ToObject<JToken>(), prop.GetValue(t));
                        prop.SetValue(t, item);
                    }
                    else
                    {
                        var item = BambuJson2Model(prop.PropertyType, ((JObject)kv.Value).ToObject<JToken>());
                        prop.SetValue(t, item);
                    }
                }
                else if (kv.Value.GetType() == typeof(JArray))
                {
                    var propertyType = prop.PropertyType;
                    var interfaces = propertyType.GetInterfaces();
                    Type itemType = null;
                    foreach (var interf in interfaces)
                    {
                        if (interf.IsGenericType &&
                            interf.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            itemType = interf.GetGenericArguments()[0];
                        }
                    }
                    var ja = (JArray)kv.Value;
                    if (itemType == null)
                    {
                        throw new Exception($"Target type of {prop.Name} not support");
                    }
                    var classInst = Activator.CreateInstance(propertyType);
                    var addMethod = propertyType.GetMethod("Add");
                    foreach (var jt in ja)
                    {
                        var item = BambuJson2Model(itemType, jt);
                        addMethod.Invoke(classInst, new object[] { item });
                    }
                    prop.SetValue(t, classInst);
                }
                else
                {
                    try
                    {
                        prop.SetValue(t, kv.Value);
                    }
                    catch
                    {
                        throw new Exception($"Target type of {prop.Name} not support");
                    }
                }
            }
        }
        return t;
    }

    public static T UpdateBambuJson2Model<T>(JObject jo, T model) where T : new()
    {
        var jDict = jo.ToObject<Dictionary<string, Object>>();
        var t = model;
        foreach (var kv in jDict)
        {
            var key = BambuJson2CsPublic(kv.Key);
            var prop = typeof(T).GetProperty(key);
            if (prop != null)
            {
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(t, kv.Value.ToString());
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(t, bool.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(t, int.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(long))
                {
                    prop.SetValue(t, long.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(float))
                {
                    prop.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (prop.PropertyType == typeof(double))
                {
                    prop.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (kv.Value.GetType() == typeof(JToken))
                {
                    if (prop.GetValue(t) != null)
                    {
                        var item = UpdateBambuJson2Model(prop.PropertyType, (JToken) kv.Value, prop.GetValue(t));
                        prop.SetValue(t, item);
                    }
                    else
                    {
                        var item = BambuJson2Model(prop.PropertyType, (JToken)kv.Value);
                        prop.SetValue(t, item);
                    }
                }
                else if (kv.Value.GetType() == typeof(JObject))
                {
                    if (prop.GetValue(t) != null)
                    {
                        var item = UpdateBambuJson2Model(prop.PropertyType, ((JObject)kv.Value).ToObject<JToken>(), prop.GetValue(t));
                        prop.SetValue(t, item);
                    }
                    else
                    {
                        var item = BambuJson2Model(prop.PropertyType, ((JObject)kv.Value).ToObject<JToken>());
                        prop.SetValue(t, item);
                    }
                }
                else if (kv.Value.GetType() == typeof(JArray))
                {
                    var propertyType = prop.PropertyType;
                    var interfaces = propertyType.GetInterfaces();
                    Type itemType = null;
                    foreach (var interf in interfaces)
                    {
                        if (interf.IsGenericType &&
                            interf.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            itemType = interf.GetGenericArguments()[0];
                        }
                    }
                    var ja = (JArray)kv.Value;
                    if (itemType == null)
                    {
                        throw new Exception($"Target type of {propertyType} not support");
                    }
                    var classInst = Activator.CreateInstance(propertyType);
                    var addMethod = propertyType.GetMethod("Add");
                    foreach (var jt in ja)
                    {
                        var item = BambuJson2Model(itemType, jt);
                        addMethod.Invoke(classInst, new object[] { item });
                    }
                    prop.SetValue(t, classInst);
                }
                else
                {
                    try
                    {
                        prop.SetValue(t, kv.Value);
                    }
                    catch
                    {
                        throw new Exception($"Target type of {prop.Name} not support");
                    }
                }
            }
        }

        return t;
    }
}