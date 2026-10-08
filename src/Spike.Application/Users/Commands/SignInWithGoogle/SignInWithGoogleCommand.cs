using Spike.Application.Common.Identity;

namespace Spike.Application.Users.Commands.SignInWithGoogle;

public record SignInWithGoogleCommand(string IdToken) : ICommand<Result<TokenDto>>;
