using Entities.Concrete;

namespace Entities.Dtos
{
    public class UserDto : User
    {
        public string Password { get; set; }
        public string? NewPassword { get; set; }
    }
}
