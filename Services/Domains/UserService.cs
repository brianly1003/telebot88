//using Karamel.LiveChat.Proto;
//using Karamel.Shared.Model;
//using Karamel.TeleBot.Model.User;
//using Karamel.TeleBot.Services.Interfaces;

//namespace Karamel.TeleBot.Services.Domains
//{
//    public class UserService(MessagingService.MessagingServiceClient messagingClient) : IUserService
//    {
//        public async Task<User> GetUserByChatId(long chatId)
//        {
//            var getUserResult = await messagingClient.GetUserByChatIdAsync(new GetUserByChatIdRequest { TelegramChatId = chatId });

//            return getUserResult?.User;
//        }

//        public async Task<OpResult<User>> CreateUser(CreateUserArgs args)
//        {
//            var createUserResult = await messagingClient.CreateUserAsync(new CreateUserRequest
//            {
//                User = new User
//                {
//                    UserType = (int)UserType.Player,
//                    UserName = args.UserName,
//                    FirstName = args.FirstName,
//                    LastName = args.LastName,
//                    TelegramChatId = args.TelegramChatId,
//                    PhoneNumber = args.PhoneNumber ?? string.Empty,
//                    Email = args.Email ?? string.Empty,
//                    AvatarColor = new Random().Next(0, 12),
//                }
//            });

//            if (createUserResult?.User == null)
//            {
//                throw new Exception("Create user failed");
//            }

//            return OpResult<User>.Success(createUserResult.User);
//        }

//        public async Task<OpResult<User>> UpdateUser(UpdateUserArgs args)
//        {
//            var updateUserResult = await messagingClient.UpdateUserAsync(new UpdateUserRequest
//            {
//                User = new User
//                {
//                    Id = args.Id,
//                    UserId = args.UserId,
//                    TelegramChatId = args.TelegramChatId,
//                    PhoneNumber = args.PhoneNumber,
//                    FirstName = args.FirstName,
//                    LastName = args.LastName,
//                    Email = args.Email
//                }
//            });

//            if (updateUserResult?.User == null)
//            {
//                throw new Exception("Update user failed");
//            }

//            return OpResult<User>.Success(updateUserResult.User);
//        }
//    }
//}