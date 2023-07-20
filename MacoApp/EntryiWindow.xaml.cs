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
        private bool CheckNet() // Проверка подключения к сети
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
        }

        private void EntryiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CheckNet() == true)  // Проверяем подключение к сети
            {
                try
                {
                    //Если доступ к сети есть, то удаляем файл с БД и далее качаем новый
                    FileInfo fileInf = new FileInfo(path);
                    if (fileInf.Exists)
                    {
                        fileInf.Delete();                   
                    }
                    WebClient webClient = new WebClient();
                    //Качаем БД с Google Drive
                    webClient.DownloadFile("https://drive.google.com/uc?export=download&id=10-tB1E0wbCRfdfcXlpxbz5s2-jTnDpHa", path);
                    webClient.Dispose();
                }
                catch (System.Exception)
                {                   
                    return;
                }
            }
            else 
            {
                return;
            }
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

    }
}