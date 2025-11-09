using Nastaran_bot.Repositories.User;

namespace Nastaran_bot.Services.User;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
}
