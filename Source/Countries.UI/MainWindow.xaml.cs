using Countries.Data;
using Countries.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Countries.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseContext db;
        private TextBlock txtLegend;

        public MainWindow()
        {
            InitializeComponent();


            //Attach a delegate of type SizeChangedEventHandler to the canvas
            chartCanvas.SizeChanged += delegate(object sender,SizeChangedEventArgs e) {
                DrawChart();
            };


        }

        /// <summary>
        /// Draw the chart when the window resizes
        /// </summary>
        private void DrawChart()
        {

            if (db == null)
            {
                //refresh the grid
                db = new DatabaseContext();
                countriesGrid.AutoGenerateColumns = true;
                countriesGrid.ItemsSource = db.Countries.ToList();
            }

            //take the top 15 countries based on GDP and top 15 countries based on population
            List<Country> countriesList = db.Countries.OrderByDescending(c => c.Population).Take(10).Union(db.Countries.OrderByDescending(c => c.GDP).Take(10)).OrderByDescending(c=>c.Name).ToList();

            int maxPopulation = countriesList.OrderByDescending(c => c.Population).First().Population;
            double maxGDP = countriesList.OrderByDescending(c => c.GDP).First().GDP;

            double width = chartCanvas.ActualWidth;
            double height = chartCanvas.ActualHeight;
            double itemWidth = width / countriesList.Count; //each chart bar has equal width
            
            //clear existing chart
            chartCanvas.Children.Clear();

            double itemX = 0;

            foreach (Country country in countriesList)
            {
                string text = string.Format("{0}\r\n\r\nPop.:\r\n{1}\r\nGDP:\r\n{2}", country.Name, country.Population.ToString("N"), country.GDP.ToString("N"));
                
                //calculate the RED component of the bar RGB color, so that the lighter red color signifies larger GDP
                byte Rcomponent = (byte)(35 +  (byte)(country.GDP / maxGDP * 220));

                #region create a rectangle to represent the chart bar for this country
                Rectangle rect = new Rectangle
                {
                    Fill = new SolidColorBrush(Color.FromArgb(255, Rcomponent, 20, 20)),
                    Width = itemWidth,
                    Height = ((double)country.Population / (double)maxPopulation) * height,
                };

                //attach a MouseEventHandler delegate to the MouseMove event of the rectangle, to highlight the country and show legend data
                rect.MouseMove += delegate(object sender, MouseEventArgs e)
                {
                    Rectangle thisRect = sender as Rectangle;
                    thisRect.Stroke = Brushes.Yellow;
                    thisRect.StrokeThickness = 5;
                    txtLegend.Text = text;
                    txtLegend.Visibility = System.Windows.Visibility.Visible;
                };

                //clear the legend when the cursor leaves the bar
                rect.MouseLeave += delegate(object sender, MouseEventArgs e)
                {
                    Rectangle thisRect = sender as Rectangle;
                    thisRect.Stroke = null;
                    txtLegend.Visibility = System.Windows.Visibility.Hidden;
                };

                Canvas.SetLeft(rect, itemX);
                double itemY = height - rect.Height;
                Canvas.SetTop(rect, itemY);
                chartCanvas.Children.Add(rect);
                #endregion

                #region create a label for the country
                TextBlock barLegend = new TextBlock
                {
                    FontSize = 15,
                    Text = country.Name,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Brushes.Yellow,
                    Width = itemWidth,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(barLegend, itemX);
                Canvas.SetTop(barLegend, itemY+2);
                chartCanvas.Children.Add(barLegend);
                #endregion

                itemX = itemX + itemWidth;

            }

            #region create the transparent legend that is displayed on top of the bars when the mouse hovers over a country
            txtLegend = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Height=157,
                Width=266,
                ClipToBounds = true,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Background = Brushes.LightGray,
                Opacity = 0.5,
                Visibility= System.Windows.Visibility.Hidden
            };
            Canvas.SetLeft(txtLegend, 10);
            Canvas.SetTop(txtLegend, 10);
            chartCanvas.Children.Add(txtLegend);
            #endregion

            chartCanvas.Focus();

        }
        
        
    }
}
