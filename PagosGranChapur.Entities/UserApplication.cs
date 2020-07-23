using PagosGranChapur.Entities.Enums;

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
