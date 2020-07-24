using System.Linq;

namespace PagosGranChapur.Entities.Helpers
{
    public class Converter
    {
        public static int[] StringIdToArrayIntId(string id)
        {
            return id.Split(',').Where(s => s != "0").Select(s => int.Parse(s)).ToArray();
        }

    }
}
