using Newtonsoft.Json.Converters;

namespace PagosGranChapur.Entities.Helpers
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        // FORMATEADOR DE FECHA AL SERIALIZAR EN JSON
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
