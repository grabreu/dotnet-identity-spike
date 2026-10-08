using System.Text.RegularExpressions;
using Spike.Application.Common.Email;

namespace Spike.Application.FunctionalTests.Testing.Fakes;

public partial class FakeEmailQueue : IEmailQueue
{
    private readonly List<EmailMessage> _messages = [];

    public IReadOnlyList<EmailMessage> Messages => _messages;

    public ValueTask EnqueueAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        _messages.Add(message);

        return ValueTask.CompletedTask;
    }

    public string LastCodeSentTo(string email)
    {
        var message = _messages.Last(message => message.To == email);

        return CodePattern().Match(message.HtmlBody).Groups[1].Value;
    }

    public void Clear()
    {
        _messages.Clear();
    }

    [GeneratedRegex(@"<strong>(\d{6})</strong>")]
    private static partial Regex CodePattern();
}
