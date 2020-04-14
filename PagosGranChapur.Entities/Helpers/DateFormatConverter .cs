using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
