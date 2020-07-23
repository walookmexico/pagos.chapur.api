using PagosGranChapur.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PagosGranChapur.API.Models
{
    public class CreditCardsList
    {
        private List<CreditCard> _list;

        public CreditCardsList() {

            this._list = new List<CreditCard> {
                new CreditCard { Id= 1, Number="6484993100578580", DateStart="0810", CVV="902", Name ="PACHECO AVILES ROMEL D" },
                new CreditCard { Id= 2, Number="6484993101002150", DateStart="0810", CVV="672", Name ="GRAJALES CHAN RUSSELL G" },
                new CreditCard { Id= 3, Number="6484993100587110", DateStart="0810", CVV="943", Name ="PECH HOIL LEYLA FLORA" },
                new CreditCard { Id= 4, Number="6484993100593720", DateStart="0810", CVV="157", Name ="VALDEZ FRANCO RUBEN JESUS" },
                new CreditCard { Id= 5, Number="6484993101205860", DateStart="0810", CVV="118", Name ="RODRIGUEZ BORGES EMILIO M" },
                new CreditCard { Id= 6, Number="6484993100597200", DateStart="0810", CVV="383", Name ="TAPIA PECH EDGAR RAMON" },
                new CreditCard { Id= 7, Number="6484993100600640", DateStart="0810", CVV="907", Name ="BALAM PEREZ RUBI ANAHI" }};
        }

        public CreditCard GetCreditCard(int id) {
            return this._list.FirstOrDefault(x => x.Id == id);
        }
    }

    
}