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

    public static T BambuJson2Model<T>(JObject jo) where T : new()
    {
        var jDict = jo.ToObject<Dictionary<string, string>>();
        var t = new T();
        foreach (var kv in jDict)
        {
            var key = BambuJson2CsPublic(kv.Key);
            var field = typeof(T).GetField(key);
            if (field != null)
            {
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(t, kv.Value);
                }
                else if (field.FieldType == typeof(bool))
                {
                    field.SetValue(t, bool.Parse(kv.Value));
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(t, int.Parse(kv.Value));
                }
                else
                {
                    throw new Exception($"Target type of {field.Name} not support");
                }
            }
        }

        return t;
    }
}