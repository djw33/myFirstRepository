using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FernNamespace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //New function for choosing a random background Image
        private void ChooseBackground()
        {
            canvas.Children.Clear();                                // delete old canvas contents
            Random rand = new Random();                             //creates a new randome number
            int i = rand.Next(1,7);                                 //random number between 1 and 6
            string filename = "Images/BackGrounds/" + i.ToString()+".jpg"; //filename of jpg found in Images folder
            Image Background = new Image();
            Background.Source = new BitmapImage(new Uri(filename, UriKind.Relative));//loads filename
            Background.Width = canvas.Width;
            Background.Height = canvas.Height;
            canvas.Children.Add(Background);    //adds randomly selected picture to background

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChooseBackground(); 
            Fern f = new Fern(sizeSlider.Value, sporesSlider.Value, biasSlider.Value, branchSlider.Value, canvas);
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ChooseBackground();
            Fern f = new Fern(sizeSlider.Value, sporesSlider.Value, biasSlider.Value, branchSlider.Value, canvas);
        }
    }

}
