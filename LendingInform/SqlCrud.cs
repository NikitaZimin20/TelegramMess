using LendingInform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LendingInform
{
   public class SqlCrud
    {
        private readonly string _connectionString;
        private SqliteDataAccess db = new SqliteDataAccess();
        

        public SqlCrud(string connection)
        {
            _connectionString = connection;
        }
        public List<BasicContactModel> CheckPassword(string login,string password)
        {
            string sql = "SELECT CASE WHEN EXISTS  (select * FROM Users Where Login=@login AND Password=@password) THEN 'TRUE' ELSE 'FALSE' END AS IsIDExist ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new {login,password }, _connectionString);
        }
        public List<BasicContactModel> CheckExistance(string username)
        {
            string sql = "SELECT CASE WHEN EXISTS  (select * FROM TelegramUsers Where Username=@username) THEN 'TRUE' ELSE 'FALSE' END AS IsIDExist ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new { username }, _connectionString);
        }

        public List<BasicContactModel> GetAllTelegramUsers()
        {
            string sql = "select Username AS TelergamUsers FROM TelegramUsers ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new {  }, _connectionString);
        }
        public void AddNewUser(string username)
        {
            string sql = "INSERT INTO TelegramUsers(Username) Values(@username)";
            db.SaveData(sql, new {username  }, _connectionString);
        }
        public void DeleteUser(string username)
        {
            string sql = "DELETE FROM TelegramUsers WHERE Username=@username";
            db.SaveData(sql, new { username }, _connectionString);
        }
        public void AddChatId(long chatid,string username)
        {
            string sql = "If Not Exists(select * from Chats where Chaid=@chatid) Begin insert  Chats values(@chatid,@username)End ";
            db.SaveData(sql, new { chatid, username }, _connectionString);
        }
        public List<BasicContactModel> GetAllChats()
        {
            string sql = "select Chaid AS ChatId  FROM Chats ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
        }
        public List<BasicContactModel> GetAdmin()
        {
            string sql = "select IsAdmin  FROM TelegramUsers ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
        }
    }
}
