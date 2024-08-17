using Domain.Dtos.Base;

namespace Application.Queries.Dtos
{
    public class UserQueryDto : QueryBaseDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}