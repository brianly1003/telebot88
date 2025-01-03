using W88.TeleBot.Model;

namespace W88.TeleBot.Services.Interfaces;

public interface IPromotionService
{
    Task<List<Promotion>> GetPromotions();

    Task<PromotionDetail> GetPromotionDetail(string? promotionName);
}