using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramSender;
using DataWorker;
using System.IO;

namespace Admin
{
   
    public partial class Form2 : Form
    {
        private string _status;
        public Form2(string status="user")
        {
            InitializeComponent();
            _status = status;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                await SendMessage();
                richTextBox1.Clear();
                MessageBox.Show("Сообщение отпраленно");
            }
            catch (Exception)
            {
                DialogResult result = MessageBox.Show("Ссесия потухла,нажмите ок");
                if (result==DialogResult.OK)
                {
                    string path = @"session.dat";
                    FileInfo fileInf = new FileInfo(path);
                    if (fileInf.Exists)
                    {
                        fileInf.Delete();
                    }
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Hide();
                }
                
            }
         


        }

        private async Task SendMessage()
        {
          
            SqlCrud sql = new SqlCrud();
            Messager messager = new Messager();
            var users = sql.GetAllTelegramUsers();
            foreach (var user in users)
            {
               await messager.MessageHandler(user.TelergamUsers, richTextBox1.Text);
            }
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3(_status);
            form.Show();
            this.Hide();
        }
    }
}
