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
        
        public List<BasicContactModel> CheckExistance(string username)
        {
            string sql = "SELECT CASE WHEN EXISTS  (select * FROM TelegramUsers Where Username=@username) THEN 'TRUE' ELSE 'FALSE' END AS IsIDExist ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new { username }, _connectionString);
        }

    
        public void AddNewUser(string username)
        {
            bool IsAdmin = false;
            string sql = "INSERT INTO TelegramUsers(Username) Values(@username,@IsAdmin)";
            db.SaveData(sql, new {username,IsAdmin}, _connectionString);
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
        public void AddAdmin(string username)
        {
            
            string sql = "If Not Exists(select * from TelegramUsers where Username=@username) Begin insert TelegramUsers  values(@username,1)End " +
                        "ELSE Begin update TelegramUsers Set IsAdmin=1 WHERE Username=@username  END";
            db.SaveData(sql, new {  username }, _connectionString);
        }
        public List<BasicContactModel> GetPrivateUsersId()
        {
            string sql = "select Chats.Chaid AS ChatId  FROM Chats  JOIN dbo.TelegramUsers ON Chats.Username=dbo.TelegramUsers.Username ";
            return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
        }
        public List<BasicContactModel> GetPublicUsersId()
        {
            string sql = "select Chaid AS ChatId  FROM Chats ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
        }
        public List<BasicContactModel> GetPrivateUsersName()
        {
            string sql = "select Chats.Username AS PrivateName  FROM Chats  JOIN dbo.TelegramUsers ON Chats.Username=dbo.TelegramUsers.Username ";
            return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
        }
        public List<BasicContactModel> GetPublicUsersName()
        {
            string sql = "select Username AS PublicName  FROM Chats ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
        }


        public List<BasicContactModel> IsAdmin()
        {
            string sql = "select IsAdmin  FROM TelegramUsers ";

            return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
        }
    }
}
