using W88.TeleBot.Model;
using W88.TeleBot.Services.Interfaces;

namespace W88.TeleBot.Services.Domains;

public class PromotionService : IPromotionService
{
    public Task<List<Promotion>> GetPromotions()
    {
        return Task.FromResult(new List<Promotion>
        {
            new() { PromotionType = "WELCOME", PromotionItems = new List<string> { "Welcome Bonus $58" }},
            new() { PromotionType = "SPORT", PromotionItems = new List<string> { "Sport Bonus" }},
            new() { PromotionType = "FIRST TIME BONUS", PromotionItems = new List<string> { "Welcome Bonus $58", "First Deposit FREE 30% Bonus" }},
            new() { PromotionType = "WEEKLY", PromotionItems = new List<string> { "Weekly deposit free 20% bonus" }},
            new() { PromotionType = "CASINO BONUS", PromotionItems = new List<string> { "Subsequent Deposit Bonus 5%" }},
        });
    }

    public async Task<PromotionDetail> GetPromotionDetail(string? promotionName)
    {
        var result = new PromotionDetail();
        string? markdownContent;
        switch (promotionName)
        {
            case "Welcome Bonus $58":
            {
                markdownContent = await File.ReadAllTextAsync("./Data/welcome-bonus.md");
                result.Content = markdownContent;
                break;
            }
            case "Sport Bonus":
                markdownContent = await File.ReadAllTextAsync("./Data/sport-bonus.md");
                result.Content = markdownContent;
                break;
            case "First Deposit FREE 30% Bonus":
                markdownContent = await File.ReadAllTextAsync("./Data/welcome-bonus.md");
                result.Content = markdownContent;
                break;
            case "Weekly deposit free 20% bonus":
                markdownContent = await File.ReadAllTextAsync("./Data/welcome-bonus.md");
                result.Content = markdownContent;
                break;
            case "Subsequent Deposit Bonus 5%":
                markdownContent = await File.ReadAllTextAsync("./Data/welcome-bonus.md");
                result.Content = markdownContent;
                break;
            default:
                throw new NotImplementedException();
        }

        return result;
    }
}