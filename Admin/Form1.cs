using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataWorker;
using TelegramSender;
namespace Admin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            

        }
    
        
        private void MoveToWindow()
        {
            Messager messager = new Messager();
            if (messager.Client.IsUserAuthorized())
            {
                Form2 form = new Form2("admin");
                form.Show();
                

            }
            else
            {
                MainWindow main = new MainWindow();
                main.Show();

            }
            this.Hide();
        } 
        private void button1_Click_1(object sender, EventArgs e)
        {
            
            Messager messager = new Messager();
            SqlCrud sql = new SqlCrud();
            var statement=sql.CheckPassword(textBox1.Text, textBox2.Text).FirstOrDefault(x=>x.IsIDExist==true);
            
            if (statement is not null)
            {
                MoveToWindow();
                
            }
            else
            {
                MessageBox.Show("Incorrect Login or Password");
                Clear();
            }


        }
        private void Clear()
        {
            textBox1.Clear();
            textBox2.Clear();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Clear();
        }
    }
}
