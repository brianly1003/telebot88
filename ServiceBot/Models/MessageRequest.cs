namespace CW88.TeleBot.ServiceBot.Models;

public class MessageRequest
{
    public List<long> ChatIds { get; set; }
    public string Message {get; set; }
}