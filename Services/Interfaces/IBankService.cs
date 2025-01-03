namespace W88.TeleBot.Services.Interfaces;

public interface IBankService
{
    Task<List<string>> GetBankInfos();
}