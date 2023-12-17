using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using TBMFurn;

namespace MacoApp
{
    public partial class EntryiWindow : Window
    {
        WorkGoogleDrive workGoogleDrive = new WorkGoogleDrive();
        static string path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString()+"\\Furnapp.db";

        public EntryiWindow()
        {
            InitializeComponent();       
            Loaded += EntryiWindow_Loaded;
        }
        /*private bool CheckNet() // Проверка подключения к сети
        {
            bool stats;
            try
            {
                using (var client = new System.Net.WebClient())
                using (var stream = client.OpenRead("https://www.google.com"))
                {
                    stats = true;
                    IntOff.Visibility = Visibility.Collapsed;
                    IntOn.Visibility = Visibility.Visible;
                    ButtonEditor.Visibility = Visibility.Hidden;
                }
            }
            catch
            {
                stats = false;
                IntOn.Visibility = Visibility.Collapsed;
                IntOff.Visibility = Visibility.Visible;
            }
            return stats;
        }*/

        private void EntryiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntOn.Visibility = Visibility.Collapsed;
            IntOff.Visibility = Visibility.Collapsed;
            ButtonEditor.Visibility = Visibility.Collapsed;
            ButtonExcel.Visibility = Visibility.Collapsed;
            if (Directory.Exists(@"X:\aTBMFURN\"))
            {
                string[] files = Directory.GetFiles(@"X:\aTBMFURN\");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                // Удаление папки и всех ее подпапок и файлов
            }

            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
            WebClient webClient = new WebClient();
            //Качаем БД с Google Drive
            webClient.DownloadFile("https://drive.google.com/uc?export=download&id=18KBF6LMWrxoDqy8cUdEUaZYCXC_8SLPu", path);
            webClient.Dispose();

        }



        private void ButtonEditor_Click(object sender, RoutedEventArgs e)
        {
            WindowPassword windowPassword = new WindowPassword();
            windowPassword.Show();
            this.Close();

        }

        private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
        {
            CalculationWindow calculationWindow = new CalculationWindow();
            calculationWindow.Show();
            this.Close();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //workGoogleDrive.DeleteFile("123");
        }

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            LoadInExcelWindow loadExcelWindow = new LoadInExcelWindow();
            loadExcelWindow.Show();
            this.Close();
        }
    }
}