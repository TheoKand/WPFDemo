using Countries.Data;
using Countries.Data.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Countries.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            // Create the startup window
            InitDatabaseWindow wnd = new InitDatabaseWindow();
            bool? ok = wnd.ShowDialog();

            if (ok == null || ok == false)
            {

                MessageBox.Show("The database initialization failed!");
                return;
            }
            else
            {
                //Show the main window
                mainWindow.Show();
            }


        }


    }
}
