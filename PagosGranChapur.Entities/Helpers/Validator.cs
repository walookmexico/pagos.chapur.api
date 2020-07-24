using System;


namespace PagosGranChapur.Entities.Helpers
{
    public class Validator
    {

        public static string ValidateDates(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null)
            {
                return "La fecha de inicio es requerida";
            }

            if (endDate == null)
            {
                return "La fecha de termino es requerida";
            }

            return null;
        }
    }
}
