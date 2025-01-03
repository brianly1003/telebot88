//using W88.LiveChat.Proto;
//using W88.TeleBot.Model.Conversation;
//using W88.TeleBot.Services.Interfaces;

//namespace W88.TeleBot.Services.Domains
//{
//    public class ConversationService(LiveChat.Proto.ConversationService.ConversationServiceClient conversationClient)
//        : IConversationService
//    {
//        public async Task<Conversation?> GetConversation(GetConversationArgs args)
//        {
//            var conversation = (await conversationClient.GetConversationAsync(new GetConversationRequest
//            {
//                SenderId = args.SenderId,
//                ReceiverIds = { args.ReceiverIds },
//                IsGroup = args.IsGroup,
//            })).Conversation;

//            return conversation;
//        }

//        public async Task<Conversation> CreateConversation(CreateConversationArgs args)
//        {
//            return (await conversationClient.CreateConversationAsync(new CreateConversationRequest
//            {
//                SenderId = args.SenderId,
//                ReceiverIds = { args.ReceiverIds },
//                IsGroup = args.IsGroup,
//            })).Conversation;
//        }
//    }
//}