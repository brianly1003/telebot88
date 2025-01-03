using CW88.TeleBot.Model;

namespace CW88.TeleBot.Services.Interfaces;

public interface IPromotionService
{
    Task<List<Promotion>> GetPromotions();

    Task<PromotionDetail> GetPromotionDetail(string? promotionName);
}