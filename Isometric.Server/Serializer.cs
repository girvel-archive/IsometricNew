using System;
using Newtonsoft.Json;

namespace Isometric.Server
{
    public static class Serializer
    {
        public static JsonSerializer Current { get; }

        static Serializer()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            };

            settings.Converters.Add(new TimeSpanConverter());

            Current = JsonSerializer.Create(settings);
        }



        private class TimeSpanConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TimeSpan);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("$type");
                writer.WriteValue(value.GetType().FullName);
                writer.WritePropertyName("$value");
                writer.WriteValue(value);
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
            {
                return value;
            }
        }
    }
}