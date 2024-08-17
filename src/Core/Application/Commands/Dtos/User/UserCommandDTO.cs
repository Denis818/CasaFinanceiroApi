using Application.Commands.Services.Base;

namespace Application.Commands.Dtos
{
    public class UserCommandDto : CommandBaseDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
