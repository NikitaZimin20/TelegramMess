using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace Admin
{
    static class Program
    {
       
        [STAThread]
        
        static async Task  Main()
        {
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form3());
        }
        
       


    }

    
}
