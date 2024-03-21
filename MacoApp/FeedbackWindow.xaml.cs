using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.IO;
using System.Net;
using Microsoft.Win32;

namespace TBMFurn
{
    /// <summary>
    /// Логика взаимодействия для FeedbackWindow.xaml
    /// </summary>
    public partial class FeedbackWindow : Window
    {
        string Email = "furntbm@yandex.ru";
        string Password = "gjmogbttlewbjtew";
        string ToEmail = "furntbm@yandex.ru";
        string ThemeMessage;
        string BodyMessage;
        string Name;

        string filePath;

        public FeedbackWindow()
        {
            InitializeComponent();
        }
        
        //Вводные данные и вызов метода с отправкой письма
        private void ButtonSendEmail_Click(object sender, RoutedEventArgs e)
        {
            ThemeMessage = TextBoxTheme.Text;
            BodyMessage = TextBoxMessage.Text;
            Name = TextBoxName.Text;
            ButtonSend(Name, Email, Password, ToEmail, ThemeMessage, BodyMessage, filePath);
        }

        private void ButtonSend(string Name, string fromEmail, string password, string toEmail, string subject, string body, string attachmentFilePath)
        {
            try
            {
                MailMessage mail = new MailMessage();
                System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient("smtp.yandex.ru");

                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = Name + ": " + subject;
                mail.Body = body;
                if (attachmentFilePath != null)
                {
                    Attachment attachment = new Attachment(attachmentFilePath, MediaTypeNames.Application.Octet);
                    mail.Attachments.Add(attachment);
                }
                else
                {
                    TextBlockPath.Text = "Без вложения";
                }

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential(fromEmail, password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("Сообщение отправлено.");
                TextBlockPath.Text = "";
                TextBoxMessage.Text = "";
                TextBoxName.Text = "";
                TextBoxTheme.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сообщение не отправлено, какая-то ошибка: " + ex.Message);
                TextBoxMessage.Text = "" + ex.Message;
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

    }
}

