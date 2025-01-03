using CW88.TeleBot.Services.Interfaces;

namespace CW88.TeleBot.Services.Domains;

public class BankService : IBankService
{
    public async Task<List<string>> GetBankInfos()
    {
        return
        [
            "Aboitiz-led Union Bank",
            "AYALA-LED BANK",
            "Bank of Makati",
            "BPI Direct BanKo",
            "City Saving Bank",
            "Gokongwei-owned Robinsons Bank Corp",
            "Philippine Savings Bank",
            "Sy-led BDO Unibank",
            "Yuchengco-led Rizal Commercial Banking Corp"
        ];
    }
}