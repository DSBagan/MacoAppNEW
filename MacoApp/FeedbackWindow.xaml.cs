using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Windows;
using System.Net;
using Microsoft.Win32;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using MaterialDesignMessageBox;

namespace TBMFurn
{
    /// <summary>
    /// Логика взаимодействия для FeedbackWindow.xaml
    /// </summary>
    public partial class FeedbackWindow : Window
    {
        string filePath;

        private readonly TelegramBotClient _botClient;
        private const string BotToken = "7143209516:AAGQUKwXPZeDZOdrcg1eii6xnQvGITdePqM";
        private const long ChatId = 1145674047;


        public FeedbackWindow()
        {
            InitializeComponent();
            _botClient = new TelegramBotClient(BotToken);
        }
        
        //Вводные данные и вызов метода с отправкой письма
        private async void ButtonSendEmail_Click(object sender, RoutedEventArgs e)
        {

            var message = TextBoxMessage.Text;
            var photoPath = filePath;
            try
            {
                if (!string.IsNullOrWhiteSpace(message)) // Отправляем текст боту
                {
                    await _botClient.SendTextMessageAsync(ChatId, message);
                }

                if (!string.IsNullOrWhiteSpace(photoPath) && File.Exists(photoPath))  // отправляем фото боту
                {
                    using (var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read))
                    {
                        var inputFile = new InputOnlineFile(stream, Path.GetFileName(photoPath));
                        await _botClient.SendPhotoAsync(ChatId, inputFile, "");
                    }
                }
                MaterialMessageBox.ShowDialog("Сообщение отправлено. Возможно когда-нибудь я обращу на него внимание.");
                TextBoxMessage.Text = "";
                TextBlockPath.Text = "";
            }
            catch (Exception)
            {
                MaterialMessageBox.ShowDialog("Сообщение НЕ отправлено, произошла какая-то неприятная ошибка.");
                throw;
            }
            

        }

        //Подгружаем фото или файл к сообщению
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // Загрузка файла
                filePath = openFileDialog.FileName;
                // Дальнейшая обработка файла
                TextBlockPath.Text = filePath;
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

