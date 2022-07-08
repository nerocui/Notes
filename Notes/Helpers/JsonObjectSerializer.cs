using CommunityToolkit.Common.Helpers;
using System;
using System.Text.Json;

namespace Notes.Helpers;

public class JsonObjectSerializer : IObjectSerializer
{
    public string Serialize<T>(T value) => JsonSerializer.Serialize(value);

    public T Deserialize<T>(string value)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(value);
        }
        catch
        {
            return default(T);
        }
    }
}
