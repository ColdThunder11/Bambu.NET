using System.Text;
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
        var jDict = jtoken.ToObject<Dictionary<string, Object>>();
        var t = Activator.CreateInstance(type);
                foreach (var kv in jDict)
        {
            var key = BambuJson2CsPublic(kv.Key);
            var field = type.GetField(key);
            if (field != null)
            {
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(t, kv.Value.ToString());
                }
                else if (field.FieldType == typeof(bool))
                {
                    field.SetValue(t, bool.Parse(kv.Value.ToString()));
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(t, int.Parse(kv.Value.ToString()));
                }
                else if (field.FieldType == typeof(long))
                {
                    field.SetValue(t, long.Parse(kv.Value.ToString()));
                }
                else if (field.FieldType == typeof(float))
                {
                    field.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (field.FieldType == typeof(double))
                {
                    field.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (kv.Value.GetType() == typeof(JToken))
                {
                    var item = BambuJson2Model(field.FieldType, (JToken)kv.Value);
                    field.SetValue(t, item);
                }
                else if (kv.Value.GetType() == typeof(JArray))
                {
                    var fieldType = field.FieldType;
                    var interfaces = fieldType.GetInterfaces();
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
                        throw new Exception($"Target type of {field.Name} not support");
                    }
                    var classInst = Activator.CreateInstance(fieldType);
                    var addMethod = fieldType.GetMethod("Add");
                    foreach (var jt in ja)
                    {
                        var item = BambuJson2Model(itemType, jt);
                        addMethod.Invoke(classInst, new object[] { item });
                    }
                    field.SetValue(t, classInst);
                }
                else
                {
                    try
                    {
                        field.SetValue(t, kv.Value);
                    }
                    catch
                    {
                        throw new Exception($"Target type of {field.Name} not support");
                    }
                }
            }
        }
        return t;
    }

    public static T BambuJson2Model<T>(JObject jo) where T : new()
    {
        var jDict = jo.ToObject<Dictionary<string, Object>>();
        var t = new T();
        foreach (var kv in jDict)
        {
            var key = BambuJson2CsPublic(kv.Key);
            var field = typeof(T).GetField(key);
            if (field != null)
            {
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(t, kv.Value.ToString());
                }
                else if (field.FieldType == typeof(bool))
                {
                    field.SetValue(t, bool.Parse(kv.Value.ToString()));
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(t, int.Parse(kv.Value.ToString()));
                }
                else if (field.FieldType == typeof(long))
                {
                    field.SetValue(t, long.Parse(kv.Value.ToString()));
                }
                else if (field.FieldType == typeof(float))
                {
                    field.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (field.FieldType == typeof(double))
                {
                    field.SetValue(t, float.Parse(kv.Value.ToString()));
                }
                else if (kv.Value.GetType() == typeof(JToken))
                {
                    var item = BambuJson2Model(field.FieldType, (JToken)kv.Value);
                    field.SetValue(t, item);
                }
                else if (kv.Value.GetType() == typeof(JArray))
                {
                    var fieldType = field.FieldType;
                    var interfaces = fieldType.GetInterfaces();
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
                        throw new Exception($"Target type of {field.Name} not support");
                    }
                    var classInst = Activator.CreateInstance(fieldType);
                    var addMethod = fieldType.GetMethod("Add");
                    foreach (var jt in ja)
                    {
                        var item = BambuJson2Model(itemType, jt);
                        addMethod.Invoke(classInst, new object[] { item });
                    }
                    field.SetValue(t, classInst);
                }
                else
                {
                    try
                    {
                        field.SetValue(t, kv.Value);
                    }
                    catch
                    {
                        throw new Exception($"Target type of {field.Name} not support");
                    }
                }
            }
        }

        return t;
    }
}