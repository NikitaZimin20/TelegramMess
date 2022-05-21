using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TLSharp;
using TLSharp.Core;
using System.Windows;
namespace TelegramSender
{
  public  class Messager
    {
        public delegate Task Registrationdelegate();
        public Messager()
        {
            Registrationdelegate registrationdelegate;
            

        }
        public TelegramClient Client = new TelegramClient(Config.ApiId, Config.ApiHash);
       
        public async Task MessageHandler(string username,string message)
        {

            await Client.ConnectAsync(); 
            
            var result = await Client.GetContactsAsync();
             
            var users = result.Users
               .Where(x => x.GetType() == typeof(TLUser))
               .Cast<TLUser>();
          await SendMessage(users, message, username);
        }
        private  async Task SendMessage(IEnumerable<TLUser> users, string message,string username)
        {
            var user = users.FirstOrDefault(c => c.Username == username);
            try
            {
                
                await Client.SendMessageAsync(new TLInputPeerUser() { UserId = user.Id }, message);
            }
            catch
            {
                await Client.SendMessageAsync(new TLInputPeerUser() { UserId = user.Id }, "Ошибка в отправке сообщения");
            }
          

        }
    }
}
