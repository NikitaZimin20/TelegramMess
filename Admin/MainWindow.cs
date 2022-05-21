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

namespace Admin
{
    public partial class MainWindow : Form
    {
        private string _hash;
        private string _code;
        private Messager _messager = new Messager();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

       
        private void TextRedactor_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.ShowDialog();
            
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            await _messager.Client.ConnectAsync();
            _hash = await _messager.Client.SendCodeRequestAsync("+79852692747");

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            _code = textBox1.Text;
             await _messager.Client.MakeAuthAsync("+79852692747", _hash, _code);
            var isAuth = _messager.Client.IsUserAuthorized();
            if (isAuth)
            {
                Form2 form = new Form2();
                form.Show();
                this.Hide();

            }
        }
    }
}
