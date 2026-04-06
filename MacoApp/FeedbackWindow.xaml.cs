using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace TBMFurn
{
    public partial class FeedbackWindow : Window
    {
        private SupabaseHelper _db;
        private bool _isAdmin = false;
        private string _currentUser = "Аноним";
        private string _currentContact = "";
        private byte[] _attachedFileData;
        private string _attachedFileName;
        private System.Timers.Timer _refreshTimer;

        public FeedbackWindow()
        {
            InitializeComponent();
            InitializeAsync();


            // Скрываем админские элементы сразу
            panelMessagesHeader.Visibility = Visibility.Collapsed;
            scrollMessages.Visibility = Visibility.Collapsed;
            panelUserInfo.Visibility = Visibility.Visible;

            txtUserName.ToolTip = "👤 Введите ваше имя (например: Иван Петров)";
            txtUserContact.ToolTip = "📧 Email или телефон для связи (например: ivan@mail.ru или +7 999 123-45-67)";
            txtNewMessage.ToolTip = "💬 Напишите ваш вопрос, предложение или отзыв";
        }

        private async void InitializeAsync()
        {
            _db = new SupabaseHelper();
            await _db.Initialize();

            // Не загружаем сообщения при старте для обычного пользователя
            // await LoadMessages(); - убираем автоматическую загрузку

            _refreshTimer = new System.Timers.Timer(10000);
            _refreshTimer.Elapsed += async (s, e) =>
            {
                if (_isAdmin)
                {
                    Dispatcher.Invoke(async () => await LoadMessages());
                }
            };
            _refreshTimer.Start();
        }

        private async Task LoadMessages()
        {
            if (!_isAdmin) return;

            try
            {
                var messages = await _db.GetMessagesAsync();
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        spMessages.Children.Clear();
                        panelMessagesHeader.Visibility = Visibility.Visible;
                        scrollMessages.Visibility = Visibility.Visible;
                        panelUserInfo.Visibility = Visibility.Collapsed;

                        lblMessageCount.Text = $"({messages.Count})";

                        foreach (var msg in messages.OrderByDescending(m => m.timestamp))
                        {
                            var card = CreateMessageCard(msg);
                            if (card != null)
                            {
                                card.Tag = msg.id;
                                spMessages.Children.Add(card);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка в Dispatcher: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка LoadMessages: {ex.Message}");
            }
        }

        // Метод для загрузки только сообщений текущего пользователя (не используется пока)
        private async Task LoadUserMessages()
        {
            try
            {
                var messages = await _db.GetMessagesAsync();
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        spMessages.Children.Clear();
                        var userMessages = messages.Where(m => m.user_name == _currentUser);

                        if (userMessages.Any())
                        {
                            scrollMessages.Visibility = Visibility.Visible;
                            panelUserInfo.Visibility = Visibility.Collapsed;

                            foreach (var msg in userMessages.OrderByDescending(m => m.timestamp))
                            {
                                var card = CreateMessageCard(msg);
                                if (card != null)
                                {
                                    card.Tag = msg.id;
                                    spMessages.Children.Add(card);
                                }
                            }
                        }
                        else
                        {
                            scrollMessages.Visibility = Visibility.Collapsed;
                            panelUserInfo.Visibility = Visibility.Visible;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка в Dispatcher: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка LoadUserMessages: {ex.Message}");
            }
        }

        private Border CreateMessageCard(MessageItem msg)
        {
            try
            {
                if (msg == null) return new Border();

                var card = new Border
                {
                    Margin = new Thickness(5),
                    Padding = new Thickness(12),
                    CornerRadius = new CornerRadius(8),
                    BorderBrush = new SolidColorBrush(Colors.LightGray),
                    BorderThickness = new Thickness(1),
                    Background = new SolidColorBrush(Colors.White),
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        BlurRadius = 3,
                        ShadowDepth = 1,
                        Opacity = 0.1
                    }
                };

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };

                var userIcon = new TextBlock
                {
                    Text = msg.user_name == "Admin" ? "👑 " : "👤 ",
                    FontSize = 14
                };
                headerPanel.Children.Add(userIcon);

                headerPanel.Children.Add(new TextBlock
                {
                    Text = msg.user_name ?? "Аноним",
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 5, 0)
                });

                if (!string.IsNullOrEmpty(msg.user_contact))
                {
                    headerPanel.Children.Add(new TextBlock
                    {
                        Text = $"📧 {msg.user_contact}",
                        Foreground = new SolidColorBrush(Colors.Gray),
                        FontSize = 11,
                        Margin = new Thickness(0, 0, 10, 0)
                    });
                }

                headerPanel.Children.Add(new TextBlock
                {
                    Text = $"🕐 {msg.timestamp:dd.MM.yyyy HH:mm}",
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize = 11
                });

                var statusColor = GetStatusColor(msg.status);
                var statusText = GetStatusText(msg.status);
                var statusBorder = new Border
                {
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(10, 3, 10, 3),
                    Margin = new Thickness(10, 0, 0, 0),
                    Background = new SolidColorBrush(statusColor)
                };
                statusBorder.Child = new TextBlock
                {
                    Text = statusText,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
                headerPanel.Children.Add(statusBorder);

                Grid.SetRow(headerPanel, 0);
                Grid.SetColumnSpan(headerPanel, 2);
                grid.Children.Add(headerPanel);

                var textBlock = new TextBlock
                {
                    Text = msg.text ?? "",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 5, 0, 8),
                    FontSize = 16,  // Увеличили с 13 до 16
                    LineHeight = 24 // Добавили межстрочный интервал для удобства чтения
                };
                Grid.SetRow(textBlock, 1);
                Grid.SetColumnSpan(textBlock, 2);
                grid.Children.Add(textBlock);

                if (!string.IsNullOrEmpty(msg.attachment_url))
                {
                    var attachPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                    var imageContainer = new Border
                    {
                        Width = 80,
                        Height = 80,
                        CornerRadius = new CornerRadius(5),
                        Background = new SolidColorBrush(Colors.LightGray),
                        Margin = new Thickness(0, 0, 10, 0),
                        Cursor = System.Windows.Input.Cursors.Hand
                    };

                    var thumbnail = new System.Windows.Controls.Image
                    {
                        Width = 80,
                        Height = 80,
                        Stretch = Stretch.Uniform
                    };

                    Task.Run(async () =>
                    {
                        try
                        {
                            var imageData = await _db.DownloadFileAsync(msg.attachment_url);
                            if (imageData != null)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        var bitmap = new BitmapImage();
                                        bitmap.BeginInit();
                                        bitmap.StreamSource = new MemoryStream(imageData);
                                        bitmap.DecodePixelWidth = 80;
                                        bitmap.EndInit();
                                        thumbnail.Source = bitmap;
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Ошибка загрузки миниатюры: {ex.Message}");
                                    }
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки файла: {ex.Message}");
                        }
                    });

                    imageContainer.Child = thumbnail;

                    imageContainer.MouseLeftButtonDown += async (s, e) =>
                    {
                        try
                        {
                            var fullImageData = await _db.DownloadFileAsync(msg.attachment_url);
                            if (fullImageData != null)
                            {
                                ShowFullImage(fullImageData, msg.attachment_name ?? "Файл");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Ошибка открытия файла: {ex.Message}");
                        }
                    };

                    attachPanel.Children.Add(imageContainer);

                    var fileInfoPanel = new StackPanel { VerticalAlignment = System.Windows.VerticalAlignment.Center };
                    var fileNameText = new TextBlock
                    {
                        Text = $"📎 {msg.attachment_name ?? "Файл"}",
                        Foreground = new SolidColorBrush(Colors.Blue),
                        Cursor = System.Windows.Input.Cursors.Hand,
                        TextDecorations = TextDecorations.Underline,
                        FontSize = 11
                    };
                    fileNameText.MouseLeftButtonDown += async (s2, e2) =>
                    {
                        try
                        {
                            var fileData = await _db.DownloadFileAsync(msg.attachment_url);
                            if (fileData != null)
                            {
                                var dialog = new SaveFileDialog { FileName = msg.attachment_name ?? "file" };
                                if (dialog.ShowDialog() == true)
                                {
                                    File.WriteAllBytes(dialog.FileName, fileData);
                                    MessageBox.Show("Файл сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
                        }
                    };
                    fileInfoPanel.Children.Add(fileNameText);
                    attachPanel.Children.Add(fileInfoPanel);

                    Grid.SetRow(attachPanel, 2);
                    Grid.SetColumnSpan(attachPanel, 2);
                    grid.Children.Add(attachPanel);
                }

                if (_isAdmin)
                {
                    var adminPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 8, 0, 0) };

                    var statusCombo = new ComboBox { Width = 130, Margin = new Thickness(2), FontSize = 11 };
                    statusCombo.Items.Add(new ComboBoxItem { Content = "🟡 Новое", Tag = 0 });
                    statusCombo.Items.Add(new ComboBoxItem { Content = "🟠 В работе", Tag = 1 });
                    statusCombo.Items.Add(new ComboBoxItem { Content = "🟢 Выполнено", Tag = 2 });
                    statusCombo.Items.Add(new ComboBoxItem { Content = "🔴 Отложено", Tag = 3 });
                    statusCombo.SelectedIndex = msg.status;

                    var currentMessageId = msg.id;
                    var currentStatus = msg.status;

                    /*statusCombo.SelectionChanged += async (s, e) =>
                    {
                        var selected = (ComboBoxItem)statusCombo.SelectedItem;
                        var newStatus = (int)selected.Tag;

                        if (newStatus == currentStatus) return;

                        var result = await _db.UpdateStatusAsync(currentMessageId, newStatus);
                        if (result)
                        {
                            currentStatus = newStatus;
                            var newStatusColor = GetStatusColor(newStatus);
                            var newStatusText = GetStatusText(newStatus);
                            statusBorder.Background = new SolidColorBrush(newStatusColor);
                            ((TextBlock)statusBorder.Child).Text = newStatusText;
                            await UpdateSingleMessage(currentMessageId);
                        }
                        else
                        {
                            statusCombo.SelectedIndex = currentStatus;
                            MessageBox.Show("Не удалось обновить статус!", "Ошибка",
                                          MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };
                    adminPanel.Children.Add(statusCombo);/*

                    /*var deleteBtn = new Button
                    {
                        Content = "🗑️ Удалить",
                        Margin = new Thickness(5, 2, 2, 2),
                        Padding = new Thickness(10, 3, 10, 3),
                        Foreground = new SolidColorBrush(Colors.Red),
                        FontSize = 11,
                        ToolTip = "Удалить сообщение"
                    };
                    deleteBtn.Click += async (s, e) =>
                    {
                        if (MessageBox.Show("Удалить сообщение?", "Подтверждение",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            var result = await _db.DeleteMessageAsync(currentMessageId);
                            if (result)
                            {
                                for (int i = 0; i < spMessages.Children.Count; i++)
                                {
                                    var cardItem = spMessages.Children[i] as Border;
                                    if (cardItem?.Tag?.ToString() == currentMessageId)
                                    {
                                        spMessages.Children.RemoveAt(i);
                                        break;
                                    }
                                }
                                lblMessageCount.Text = $"({spMessages.Children.Count})";
                            }
                            else
                            {
                                MessageBox.Show("Не удалось удалить сообщение!", "Ошибка",
                                              MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };
                    adminPanel.Children.Add(deleteBtn);*/

                    Grid.SetRow(adminPanel, 0);
                    Grid.SetColumn(adminPanel, 1);
                    grid.Children.Add(adminPanel);
                }

                card.Child = grid;
                return card;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в CreateMessageCard: {ex.Message}");
                return new Border { Background = new SolidColorBrush(Colors.LightPink), Child = new TextBlock { Text = "⚠️ Ошибка отображения сообщения" } };
            }
        }

        private async Task UpdateSingleMessage(string messageId)
        {
            try
            {
                var messages = await _db.GetMessagesAsync();
                var updatedMessage = messages.FirstOrDefault(m => m.id == messageId);

                if (updatedMessage != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        for (int i = 0; i < spMessages.Children.Count; i++)
                        {
                            var card = spMessages.Children[i] as Border;
                            if (card?.Tag?.ToString() == messageId)
                            {
                                var newCard = CreateMessageCard(updatedMessage);
                                newCard.Tag = messageId;
                                spMessages.Children[i] = newCard;
                                break;
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка UpdateSingleMessage: {ex.Message}");
                await LoadMessages();
            }
        }

        private async void BtnExportMessages_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
            {
                MessageBox.Show("Только администратор может экспортировать сообщения", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var messages = await _db.GetMessagesAsync();
            if (messages.Count == 0)
            {
                MessageBox.Show("Нет сообщений для экспорта", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xls",
                FileName = $"feedback_{DateTime.Now:yyyyMMdd_HHmmss}.xls"
            };

            if (saveDialog.ShowDialog() == true)
            {
                ExportToExcel(messages, saveDialog.FileName);
            }
        }

        private void ExportToExcel(List<MessageItem> messages, string filePath)
        {
            try
            {
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Обратная связь");

                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("ID");
                headerRow.CreateCell(1).SetCellValue("Дата");
                headerRow.CreateCell(2).SetCellValue("Имя");
                headerRow.CreateCell(3).SetCellValue("Контакт");
                headerRow.CreateCell(4).SetCellValue("Сообщение");
                headerRow.CreateCell(5).SetCellValue("Статус");
                headerRow.CreateCell(6).SetCellValue("Вложение");

                for (int i = 0; i < messages.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.CreateCell(0).SetCellValue(messages[i].id);
                    row.CreateCell(1).SetCellValue(messages[i].timestamp.ToString("dd.MM.yyyy HH:mm"));
                    row.CreateCell(2).SetCellValue(messages[i].user_name ?? "");
                    row.CreateCell(3).SetCellValue(messages[i].user_contact ?? "");
                    row.CreateCell(4).SetCellValue(messages[i].text ?? "");
                    row.CreateCell(5).SetCellValue(GetStatusText(messages[i].status));
                    row.CreateCell(6).SetCellValue(messages[i].attachment_name ?? "");
                }

                for (int i = 0; i < 7; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }

                MessageBox.Show($"Экспорт завершен!\nФайл сохранен: {filePath}", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowFullImage(byte[] imageData, string fileName)
        {
            var window = new Window
            {
                Title = $"📷 {fileName}",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowStyle = WindowStyle.ToolWindow,
                Background = new SolidColorBrush(Colors.Black),
                Content = new Grid()
            };

            var image = new System.Windows.Controls.Image
            {
                Stretch = Stretch.Uniform,
                Margin = new Thickness(10)
            };

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(imageData);
            bitmap.EndInit();
            image.Source = bitmap;

            image.MouseLeftButtonDown += (s, e) => window.Close();

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = image
            };

            (window.Content as Grid).Children.Add(scrollViewer);
            window.ShowDialog();
        }

        private Color GetStatusColor(int status)
        {
            switch (status)
            {
                case 0: return Color.FromRgb(158, 158, 158);
                case 1: return Color.FromRgb(255, 193, 7);
                case 2: return Color.FromRgb(76, 175, 80);
                case 3: return Color.FromRgb(244, 67, 54);
                default: return Color.FromRgb(158, 158, 158);
            }
        }

        private string GetStatusText(int status)
        {
            switch (status)
            {
                case 0: return "🟡 Новое";
                case 1: return "🟠 В работе";
                case 2: return "🟢 Выполнено";
                case 3: return "🔴 Отложено";
                default: return "🟡 Новое";
            }
        }

        private void ChkAdminMode_Checked(object sender, RoutedEventArgs e)
        {
            txtAdminPassword.Visibility = Visibility.Visible;
            txtAdminPassword.Focus();
        }

        private void ChkAdminMode_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAdminPassword.Visibility = Visibility.Collapsed;
            txtAdminPassword.Password = "";
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (chkAdminMode.IsChecked == true)
            {
                var isValid = await _db.VerifyAdminPasswordAsync(txtAdminPassword.Password);
                if (isValid)
                {
                    _isAdmin = true;
                    _currentUser = "Admin";
                    lblUserStatus.Text = "👑 Режим администратора активен";
                    lblUserStatus.Foreground = new SolidColorBrush(Colors.Orange);

                    // Показываем админскую панель и скрываем пользовательскую
                    await LoadMessages();
                    txtNewMessage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("❌ Неверный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtAdminPassword.Password = "";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtUserName.Text))
                {
                    MessageBox.Show("👤 Пожалуйста, введите ваше имя", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUserName.Focus();
                    return;
                }

                _isAdmin = false;
                _currentUser = txtUserName.Text.Trim();
                _currentContact = txtUserContact.Text.Trim();
                lblUserStatus.Text = $"✅ Вы вошли как {_currentUser}";
                lblUserStatus.Foreground = new SolidColorBrush(Colors.Green);

                // Скрываем админские элементы
                panelMessagesHeader.Visibility = Visibility.Collapsed;
                scrollMessages.Visibility = Visibility.Collapsed;
                panelUserInfo.Visibility = Visibility.Visible;
                txtNewMessage.Visibility = Visibility.Hidden;
            }
        }

        private void BtnAttach_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Выберите файл для прикрепления";
            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Все файлы|*.*";
            if (dialog.ShowDialog() == true)
            {
                _attachedFileData = File.ReadAllBytes(dialog.FileName);
                _attachedFileName = System.IO.Path.GetFileName(dialog.FileName);
                lblAttachedFile.Text = $"📎 {_attachedFileName}";
                lblAttachedFile.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewMessage.Text))
            {
                MessageBox.Show("💬 Пожалуйста, введите сообщение!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNewMessage.Focus();
                return;
            }

            // Проверяем, что пользователь ввел имя (для не-админа)
            if (!_isAdmin && string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("👤 Пожалуйста, введите ваше имя перед отправкой сообщения",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUserName.Focus();
                return;
            }

            btnSend.IsEnabled = false;
            btnSend.Content = "⏳ Отправка...";

            // Определяем имя и контакт для отправки
            string userName;
            string userContact;

            if (_isAdmin)
            {
                userName = "Admin";
                userContact = "admin@system";
            }
            else
            {
                userName = txtUserName.Text.Trim();
                userContact = txtUserContact.Text.Trim();
            }

            System.Diagnostics.Debug.WriteLine($"=== ОТПРАВКА СООБЩЕНИЯ ===");
            System.Diagnostics.Debug.WriteLine($"Имя: {userName}");
            System.Diagnostics.Debug.WriteLine($"Контакт: {userContact}");
            System.Diagnostics.Debug.WriteLine($"Текст: {txtNewMessage.Text}");
            System.Diagnostics.Debug.WriteLine($"Файл: {_attachedFileName ?? "нет"}");

            var success = await _db.SendMessageAsync(
                txtNewMessage.Text,
                userName,
                userContact,
                _attachedFileData,
                _attachedFileName
            );

            if (success)
            {
                txtNewMessage.Text = "";
                lblAttachedFile.Text = "";
                lblAttachedFile.Foreground = new SolidColorBrush(Colors.Gray);
                _attachedFileData = null;
                _attachedFileName = null;

                if (_isAdmin)
                {
                    await LoadMessages();
                }

                MessageBox.Show("✅ Сообщение успешно отправлено!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("❌ Ошибка отправки! Проверьте подключение к интернету.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            btnSend.IsEnabled = true;
            btnSend.Content = "📤 Отправить";
        }

        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            base.OnClosed(e);
        }
    }
}