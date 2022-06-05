using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using LendingInform.Models;

namespace LendingInform
{

    class Handlers
    {
        private static SqlCrud _sql = new SqlCrud(GetDescription("Default"));
        private static Commands _command;

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Handlers handlers = new Handlers();
            var ErrorMessage = exception switch
            {

                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {

                UpdateType.Message => BotOnMessageReceived(botClient, update!),
              
                UpdateType.Unknown => UnknownUpdateHandlerAsync(botClient, update),

                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }
        
        private static async Task TextMessage(ITelegramBotClient botClient, Update update)
        {
                    string[] mes = update.Message.Text.Split();
                    var action = mes[0] switch  
                    {
                        "/start" => SendStartMessages(botClient, update.Message),
                        "/help"=>AdminController(botClient,update.Message, GetDescription("Help")),
                        "/public" => AdminController(botClient, update.Message, GetDescription("Rules"),Commands.SendPublic),
                        "/private"=> AdminController(botClient, update.Message, GetDescription("Rules"), Commands.SendPublic),
                        "/add"=>AdminController(botClient, update.Message, "Введите имя удаляемого пользователя", Commands.Add),
                        "/delete"=> AdminController(botClient, update.Message, "Введите имя удаляемого пользователя", Commands.Delete),
                        "/addadmin"=>AdminController(botClient,update.Message,"Введите имя админа",Commands.AddAdmin),
                        "/getpublic"=>AdminController(botClient,update.Message,string.Join(Environment.NewLine,_sql.GetPublicUsersName().Select(x=>x.PublicName))),
                        "/getprivate" => AdminController(botClient, update.Message, string.Join(Environment.NewLine, _sql.GetPrivateUsersName().Select(x => x.PrivateName))),
                        _ => SendUnknowMessage(botClient, update.Message)
                    };
                    await action;
        }
        
        private static async Task AdminController(ITelegramBotClient botClient, Message message,string text,Commands command=Commands.None)
        {
           
            var statement = _sql.IsAdmin().FirstOrDefault(x => x.IsAdmin == true);
            if (statement != null)
            {
              _command = command;
              
              await  botClient.SendTextMessageAsync(message.Chat.Id, text);

            }

            else await botClient.SendTextMessageAsync(message.Chat.Id, "У вас нет таких прав");
        }
        private static async Task PostController(ITelegramBotClient botClient, Message message)
        {
            if (_command == Commands.SendPublic)
                await SendPost(botClient, message, _sql.GetPublicUsersId());

            else if (_command == Commands.SendPrivate)

                await SendPost(botClient, message, _sql.GetPrivateUsersId());
            else
                await botClient.SendTextMessageAsync(message.Chat.Id, "Данное сообщение не является командой");
            
        }
        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Update update)
        {
            var messagetypes = update.Message.Type switch
            {
                MessageType.Text=>TextMessage(botClient,update),
                MessageType.Photo=>PostController(botClient,update.Message),
                MessageType.Document=> PostController(botClient, update.Message),
                MessageType.Video=> PostController(botClient, update.Message),
                _=>botClient.SendTextMessageAsync(update.Message.Chat.Id,"Мы не принимаем такой тип сообщений")
            };
            await messagetypes;
        }
        private static async Task SendPost(ITelegramBotClient botClient, Message message, List<BasicContactModel> chat)
        {
            var items = chat;
            foreach (var item in items)
            {
                
                if (message.Type == MessageType.Photo)
                {
                    var file = await botClient.GetFileAsync(message.Photo[message.Photo.Length - 1].FileId);
                    await botClient.SendPhotoAsync(item.ChatId, file.FileId, parseMode: ParseMode.Html,caption:message.Caption);
                }
                    
                else if (message.Type == MessageType.Text)
                    await botClient.SendTextMessageAsync(item.ChatId, message.Text);
                else if (message.Type == MessageType.Video)
                {
                    var file = await botClient.GetFileAsync(message.Video.FileId);
                    await botClient.SendVideoAsync(item.ChatId, file.FileId, supportsStreaming: true, caption: message.Caption);
                }
                else
                {
                    var file = await botClient.GetFileAsync(message.Document.FileId);
                    await botClient.SendDocumentAsync(item.ChatId, file.FileId, caption: message.Caption);
                }
                Thread.Sleep(1000);
            }
            
        }
        private static async Task SendUnknowMessage(ITelegramBotClient botClient, Message message)
        {
            if (_command==Commands.SendPublic||_command==Commands.SendPrivate)
            {
                PostController(botClient, message);
            }
           else  if (_command == Commands.Add)
            {
                _sql.AddNewUser(message.Text);
               await botClient.SendTextMessageAsync(message.Chat.Id, "Пользователь записан");
                
            }
            else if (_command==Commands.Delete)
            {
                _sql.DeleteUser(message.Text);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Пользователь удален");
            }
            else if (_command==Commands.AddAdmin)
            {
                _sql.AddAdmin(message.Text);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Админ добавлен");
            }
            else
                await botClient.SendTextMessageAsync(message.Chat.Id, "по всем вопросам обращайтесь в тех поддержку");
        }
    
        private static async Task SendStartMessages(ITelegramBotClient botClient, Message message)
        {
            _sql.AddChatId(message.Chat.Id, message.From.Username);
           
            string namereplacer = GetDescription("Introdaction").Replace("[имя]", message.From.FirstName);
            var ob =new string[] { "Hello", "Results", "Doit", "SeeU",  };
            await botClient.SendTextMessageAsync(message.Chat.Id, text: namereplacer);
            foreach (var item in ob)
            {

                if (item=="Doit")
                {
                    using (var saveImageStream = new FileStream("file.jpg", FileMode.Open))
                    {
                       
                        await botClient.SendPhotoAsync(message.Chat.Id, saveImageStream, caption: message.Caption);

                    }
                 
                }
                Thread.Sleep(4000);
                await botClient.SendTextMessageAsync(message.Chat.Id, text: GetDescription(item).ToString());
            }
        }
        private static async Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)=> Console.WriteLine("Неизвестная ошибка чел");
        private static string GetDescription(string message)
        {
           
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Configuration.json");
            var config = builder.Build();
            var items= config[message];
            return items ;
        }
       
    }
}
