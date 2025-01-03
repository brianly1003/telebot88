namespace W88.TeleBot.Model.User;

public class CreateUserArgs
{
    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PhoneNumber { get; set; }

    public long TelegramChatId { get; set; }

    public string Email { get; set; }
}