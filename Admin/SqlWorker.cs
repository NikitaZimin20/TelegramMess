using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Admin
{
    class SqlWorker
    {
        public readonly string TelegramInformation = "select ID, Username from TelegramUsers";
        public readonly string AdminUserInfo = "select ID,Login,Password from Users";
        public readonly string StandartUserInformation = "select ID,Login from Users";
    }
}
