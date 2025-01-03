namespace W88.TeleBot.Model.Conversation;

public class CreateConversationArgs
{
    public string SenderId { get; set; }

    public string[] ReceiverIds { get; set; }

    public bool IsGroup { get; set; }
}