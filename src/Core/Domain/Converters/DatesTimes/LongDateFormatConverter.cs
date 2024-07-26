using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Converters.DatesTimes
{
    public class LongDateFormatConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Use o formato longo de data com cultura pt-BR ao escrever a data de volta ao JSON.
            writer.WriteStringValue(value.ToString("Y", new CultureInfo("pt-BR")));
        }
    }
}