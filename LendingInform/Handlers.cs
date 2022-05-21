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

namespace LendingInform
{

    class Handlers
    {
        private static SqlCrud _sql = new SqlCrud(GetDescription("Default"));
        private static string _lastmsg = string.Empty;

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
                var statement = _sql.CheckExistance(update.Message.From.Username).FirstOrDefault(x => x.IsIDExist == true);

                if (!statement.Equals(null))
                {
                    var action = mes[0] switch
                    {
                        "/start" => GetStart(botClient, update.Message),
                        "/startreciving" => botClient.SendTextMessageAsync(update.Message.Chat.Id, GetDescription("StartReciving").ToString()),
                        "/stopreciving" => botClient.SendTextMessageAsync(update.Message.Chat.Id, GetDescription("StopReciving").ToString()),
                        "Admin" => GetAdminPanel(botClient, update.Message),
                        "/adduser"=>AddNewUser(botClient,update.Message),
                        _ => SendUnknowMessage(botClient, update.Message)
                    };
                    await action;
                }
                else
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, text: GetDescription("Unregister").ToString());
                }
            
        }

        private static async Task AddNewUser(ITelegramBotClient botClient, Message message)
        {
            var statement = _sql.GetAdmin().FirstOrDefault(x => x.IsAdmin == true);
            if (statement != null)
            {
                _lastmsg = "adduser";
                botClient.SendTextMessageAsync(message.Chat.Id,"Напишите никнейм добовляемого юзера");

            }
            
            else botClient.SendTextMessageAsync(message.Chat.Id, "У вас нет таких прав");
        }

        private static async Task MediaMessage(ITelegramBotClient botClient, Update update)
        {
            if (_lastmsg == "Admin")
            {
                await SendPost(botClient, update.Message);

            }
            else
            {
               await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Данное сообщение не является командой");
            }
        }
        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Update update)
        {
            var messagetypes = update.Message.Type switch
            {
                MessageType.Text=>TextMessage(botClient,update),
                MessageType.Photo=>MediaMessage(botClient,update),
                MessageType.Document=> MediaMessage(botClient, update),
                MessageType.Video=> MediaMessage(botClient, update),
                _=>botClient.SendTextMessageAsync(update.Message.Chat.Id,"Мы не принимаем такой тип сообщений")
            };
            
           
           

        }
        
        private static async Task GetAdminPanel(ITelegramBotClient botClient, Message message)
        {
            var statement = _sql.GetAdmin().FirstOrDefault(x => x.IsAdmin == true);
            if (statement != null)
            {
                _lastmsg = message.Text;

                await botClient.SendTextMessageAsync(message.Chat.Id, 
                    GetDescription("Rules").ToString() );
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная комманда");
            }


        }
        private static async Task SendPost(ITelegramBotClient botClient, Message message)
        {
            var items = _sql.GetAllChats();
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

            }
            
        }
        private static async Task SendUnknowMessage(ITelegramBotClient botClient, Message message)
        {
            if (_lastmsg == "Admin")
            {
                 await SendPost(botClient,message);
            }
            else if (_lastmsg == "adduser")
            {
                _sql.AddNewUser(message.Text);
                botClient.SendTextMessageAsync(message.Chat.Id, "Пользователь записан");
                
            }
            else
                await botClient.SendTextMessageAsync(message.Chat.Id, "по всем вопросам обращайтесь в тех поддержку");
        }
    
        private static async Task GetStart(ITelegramBotClient botClient, Message message)
        {
            _sql.AddChatId(message.Chat.Id, message.From.Username);
            string namereplacer = GetDescription("Introdaction").Replace("[имя]", message.From.FirstName);
            var ob =new string[] { "Hello", "Results", "Doit", "SeeU",  };
            await botClient.SendTextMessageAsync(message.Chat.Id, text: namereplacer);
            foreach (var item in ob)
            {
                Thread.Sleep(10000);
                await botClient.SendTextMessageAsync(message.Chat.Id, text: GetDescription(item).ToString());
            }
            

        }
        private static async Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {

            Console.WriteLine("Неизвестная ошибка чел");

        }
        public static dynamic LoadJson()
        {
            dynamic array;
            using (StreamReader r = new StreamReader("Configuration.json"))
            {

                string json = r.ReadToEnd();
                array = JsonConvert.DeserializeObject(json);
            }
            return array;
        }
        private static string GetDescription(string message)
        {
           
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Configuration.json");
            var config = builder.Build();
            var items= config[message];


            return items ;
        }
       
    }
}
