using Countries.Data;
using Countries.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Countries.UI
{
    /// <summary>
    /// Interaction logic for InitDatabaseWindow.xaml
    /// </summary>
    public partial class InitDatabaseWindow : Window
    {

        BackgroundWorker _backgroundWorker;

        public InitDatabaseWindow()
        {
            InitializeComponent();

        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            Mouse.OverrideCursor = Cursors.Wait;

            //use a background worker to do the time-consuming task
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.ProgressChanged += _backgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerAsync();

        }


        /// <summary>
        /// This worker method contains the code that will be executed in the background thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            _backgroundWorker.ReportProgress(-1, "Connecting to database...");

            //When this line is executed for the first time, the database will be created by Entity Framework
            using (var db = new DatabaseContext())
            {
                int count = db.Countries.ToList().Count;
                if (count == 0)
                {
                    _backgroundWorker.ReportProgress(-1, "Reading JSon data...");

                    dynamic countriesObj = JSonHelper.GetJsonObject();

                    _backgroundWorker.ReportProgress(0, countriesObj.Length);

                    #region initialize the database with rows that originate from the json input string 
                    for (int i = 0; i < countriesObj.Length; i++)
                    {


                        var newCountry = new Country();
                        newCountry.Name = countriesObj[i].Name;
                        newCountry.GDP = countriesObj[i].GDP;
                        newCountry.Population = countriesObj[i].Population;

                        db.Countries.Add(newCountry);

                        db.SaveChanges();

                        _backgroundWorker.ReportProgress(i + 1, string.Format("Inserting row {0} of {1}", i, countriesObj.Length));

                    }
                    #endregion
                }

            }

        }

        /// <summary>
        /// The background thread was finished. Close the modal window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.UpArrow;

            if (e.Cancelled || e.Error != null)
            {
                this.DialogResult = false;
                this.Close();
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// This is the only place where we can update the UI because it runs in the main thread. The e.UserState
        /// object can be used to pass any kind of state object to this method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                lblProgress.Content = e.UserState.ToString();
            }
            else if (e.ProgressPercentage == 0)
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = Convert.ToInt32(e.UserState);
                progressBar.Value = 0;

            }
            else
            {
                progressBar.Value = e.ProgressPercentage;
                lblProgress.Content = e.UserState.ToString();
            }
        }




    }
}
