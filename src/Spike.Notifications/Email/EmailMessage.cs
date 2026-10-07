namespace Spike.Notifications.Email;

public record EmailMessage(string To, string Subject, string HtmlBody);
