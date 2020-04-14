using PagosGranChapur.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities
{
    public class UserApplication
    {
        public int Id               { get; set; }
        public string EmailAddress  { get; set; }
        public string FullName      { get; set; }     
        public string UserName      { get; set; }
        public EnumRoles Rol        { get; set; }
        public bool ChangePassword  { get; set; }
        
    }
}
