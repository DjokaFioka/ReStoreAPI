using Microsoft.AspNetCore.Identity;

namespace ReStoreAPI.Entities
{
    public class User : IdentityUser<int>
    {
        public UserAddress Address { get; set; }
    }
}
