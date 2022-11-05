using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dbacr;

public static class JsonValidator
{
    /// <summary>
    /// Checks if the Json is valid
    /// </summary>
    public static bool IsValidJson(this string json)
    {
        try
        {
            JToken.Parse(json);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Try to deserialize the json to the specified type
    /// </summary>
    public static T? TryDeserializeJson<T>(this string json)
    {
        if(string.IsNullOrEmpty(json)) return default;

        if (typeof(T) == typeof(object))
            return (T)(object)json;

        return json.IsValidJson() ?
            JsonConvert.DeserializeObject<T>(json) : default;
    }
}