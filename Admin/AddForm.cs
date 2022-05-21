using System;
using System.Windows.Forms;
using DataWorker;


namespace Admin
{
   
    public partial class AddForm : Form
    {
        private string _controler;
        public AddForm(string controler)
        {
            InitializeComponent();
            _controler = controler;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            SqlCrud sql = new SqlCrud();
            if (_controler=="add")
            {
                sql.AddNewUser(textBox1.Text);
                Form3 form = new Form3();
                form.Show();
            }
            else
            {
                sql.DeleteUser(textBox1.Text);
                Form3 form = new Form3();
                form.Show();

            }
            Hide();
        }
    }
}
