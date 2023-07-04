using Microsoft.AspNetCore.Identity;

namespace EMRWebAPI.Model
{
    public class UserDto
    {
        public string UserName { get; set; }  // Add this
        public string Password { get; set; }  // Add this
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressDto Address { get; set; }
    }


}
