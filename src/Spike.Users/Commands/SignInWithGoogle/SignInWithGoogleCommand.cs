using Spike.Users.Identity;

namespace Spike.Users.Commands.SignInWithGoogle;

public record SignInWithGoogleCommand(string IdToken) : ICommand<Result<TokenDto>>;
