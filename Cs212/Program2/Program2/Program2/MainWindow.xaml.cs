using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;

namespace BabbleSample
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        Dictionary<string, ArrayList> hashTable; //public hashtable for other functions to use. 

        public MainWindow()
        {
            InitializeComponent();
        }

/*function that builds and returns the hashtable as a Dictionary
 * Input: The array of words that the array should be built from
 * also desired order number should be 1 to 5, 0 will not work properly
 * Output: Hashtable, Keys contain order number words and Values contain 1 word always
 * */
        Dictionary<string, ArrayList> makeHashtable(string[] words)
        {
            Dictionary<string, ArrayList> hashTable
                = new Dictionary<string, ArrayList>();                                  //build a new Dictionary<str,ArrLst>
            for (int i = 0; i < words.Length - orderComboBox.SelectedIndex; i++)
            {
                string Baseword = "";                                                   //Initialize Baseword to empty string
                for (int j = 0; j < orderComboBox.SelectedIndex; j++)
                    Baseword += words[i+j] + " ";                                       //add order number words together for the key
                if (!hashTable.ContainsKey(Baseword))                                   //check if the key is already in hash table
                    hashTable.Add(Baseword, new ArrayList());
                hashTable[Baseword].Add(words[i + orderComboBox.SelectedIndex]);        //Add next word to the current Key
            }

            textBlock1.Text = " ";                                                      //Print out the number of words and the
            textBlock1.Text += "Total number of words: " + words.Length + " \n";        //number of unique keys
            textBlock1.Text += "Unique Keys found: " + hashTable.Keys.Count + " \n";
            return hashTable;
        }


/*opens a window to pick the new .txt file, then loads it into input, and then splits it up
 * and puts the spliced words in string[] words. After all that is done is calls the 
 * makeHashtable(string[] words) function to build the hashtable. 
 * */
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);   // read file
                words = Regex.Split(input, @"\s+");                 // split into array of words
                hashTable = makeHashtable(words);                   //Builds Hashtable
            }
        }

        /* Input checks to see if the order is greater then 0 and rehashes the 
         * table with words string[].
         * Output: new hashTable with correct order number.
         * */
        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                MessageBox.Show("Analyzing at order: " + order);
                hashTable = makeHashtable(words);                   //Builds Hashtable
            }
        }

        /* After Babble Button is clicked it makes a 200 word block of 
         * random text using the hashTable variable. 
         * */
        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            if (orderComboBox.SelectedIndex != 0)       //checks to make sure the current order is not  
            {
                string[] current = new string[orderComboBox.SelectedIndex + 1];     //makes current to keep track of past words for the key. 
                for (int j = 0; j < orderComboBox.SelectedIndex; j++)
                    current[j] = words[j];              //initializes currunt to the first order number words in the .txt file
                string key = hashTable.Keys.First();    //Initializes key to the first key in the hashtable
                Random rnd = new Random();              //makes a random number that will be used for finding the next word to input
                textBlock1.Text = key;                  //prints the initialized key
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                {
                    if (hashTable.Keys.Count == 0)      //if the key has no values(last word in the file) starts from the beginning
                        for (int j = 0; j < orderComboBox.SelectedIndex; j++)
                        {
                            current[j] = words[j];
                            textBlock1.Text += current[j] + " ";
                        }
                    int number = rnd.Next(0, hashTable[key].Count); //makes a random number from 0 to the number of values in key
                    current[orderComboBox.SelectedIndex] = hashTable[key][number].ToString();//next word in current is chosen with number
                    key = "";                           //emptys key
                    for (int j = 0; j < orderComboBox.SelectedIndex; j++)//builds new key using the string[] current 
                    {
                        key += current[j + 1] + " ";
                        current[j] = current[j + 1];
                    }

                    textBlock1.Text += current[orderComboBox.SelectedIndex] + " "; //prints the new word to the text box
                }
            }
            else //if the order is 0 then it just prints out the original file.
            {
                textBlock1.Text = " ";
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                {
                    textBlock1.Text += words[i] + " ";
                }
            }

        }
        //if order selection changes the input is analyzed again
        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
