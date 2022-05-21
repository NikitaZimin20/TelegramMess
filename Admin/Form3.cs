using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using DataWorker;

namespace Admin
{
    public partial class Form3 : Form
    {
        SqlWorker worker = new SqlWorker();
        DataTableWorker data = new();
       
        private string _status;
        
        public Form3(string status="user")
        {
            InitializeComponent();
            _status = status;
            dataGridView1.DataSource = data.Open(worker.TelegramInformation);
            Load += Form3_Load;
            

        }

        private void Form3_Load(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

       
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
            Hide();
        }
        private void MoveToWindow(string format)
        {
            AddForm form = new AddForm(format);
            form.Show();
            Hide();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MoveToWindow("add");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MoveToWindow("delete");
        }
    }
}
