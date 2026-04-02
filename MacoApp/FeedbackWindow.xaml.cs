using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            btnLogin.Visibility = Visibility.Collapsed;
            chkAdminMode.Visibility = Visibility.Collapsed;
        }

        private async void InitializeAsync()
        {
            _db = new SupabaseHelper();
            await _db.Initialize();
            await LoadMessages();

            // Автообновление каждые 10 секунд
            _refreshTimer = new System.Timers.Timer(10000);
            _refreshTimer.Elapsed += async (s, e) =>
            {
                Dispatcher.Invoke(async () => await LoadMessages());
            };
            _refreshTimer.Start();
        }

        private async Task LoadMessages()
        {
            try
            {
                var messages = await _db.GetMessagesAsync();
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        spMessages.Children.Clear();
                        foreach (var msg in messages)
                        {
                            try
                            {
                                var card = CreateMessageCard(msg);
                                if (card != null)
                                {
                                    card.Tag = msg.id;
                                    spMessages.Children.Add(card);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Ошибка создания карточки для {msg.id}: {ex.Message}");
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

        private Border CreateMessageCard(MessageItem msg)
        {
            try
            {
                if (msg == null) return new Border();

                var card = new Border
                {
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    CornerRadius = new CornerRadius(5),
                    BorderBrush = new SolidColorBrush(Colors.LightGray),
                    BorderThickness = new Thickness(1),
                    Background = new SolidColorBrush(Colors.White)
                };

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Заголовок
                var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 5) };
                headerPanel.Children.Add(new TextBlock
                {
                    Text = msg.user_name ?? "Аноним",
                    FontWeight = FontWeights.Bold,
                    FontSize = 14
                });
                headerPanel.Children.Add(new TextBlock
                {
                    Text = $" ({msg.user_contact ?? ""})",
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize = 11
                });
                headerPanel.Children.Add(new TextBlock
                {
                    Text = $" • {msg.timestamp:dd.MM.yyyy HH:mm}",
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize = 11
                });

                // Статус
                var statusColor = GetStatusColor(msg.status);
                var statusText = GetStatusText(msg.status);
                var statusBorder = new Border
                {
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(8, 2, 8, 2),
                    Margin = new Thickness(10, 0, 0, 0),
                    Background = new SolidColorBrush(statusColor)
                };
                statusBorder.Child = new TextBlock
                {
                    Text = statusText,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 10
                };
                headerPanel.Children.Add(statusBorder);

                Grid.SetRow(headerPanel, 0);
                Grid.SetColumnSpan(headerPanel, 2);
                grid.Children.Add(headerPanel);

                // Текст сообщения
                var textBlock = new TextBlock
                {
                    Text = msg.text ?? "",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                Grid.SetRow(textBlock, 1);
                Grid.SetColumnSpan(textBlock, 2);
                grid.Children.Add(textBlock);

                // Вложение
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
                        Stretch = System.Windows.Media.Stretch.Uniform
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
                                        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
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

                    var fileInfoPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                    var fileNameText = new TextBlock
                    {
                        Text = msg.attachment_name ?? "Файл",
                        Foreground = new SolidColorBrush(Colors.Blue),
                        Cursor = System.Windows.Input.Cursors.Hand,
                        TextDecorations = TextDecorations.Underline
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

                // Админ панель
                if (_isAdmin)
                {
                    var adminPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 0) };

                    var statusCombo = new ComboBox { Width = 120, Margin = new Thickness(2) };
                    statusCombo.Items.Add(new ComboBoxItem { Content = "🟡 Новое", Tag = 0 });
                    statusCombo.Items.Add(new ComboBoxItem { Content = "🟠 В работе", Tag = 1 });
                    statusCombo.Items.Add(new ComboBoxItem { Content = "🟢 Выполнено", Tag = 2 });
                    statusCombo.Items.Add(new ComboBoxItem { Content = "🔴 Отложено", Tag = 3 });
                    statusCombo.SelectedIndex = msg.status;

                    var currentMessageId = msg.id;
                    var currentStatus = msg.status;

                    statusCombo.SelectionChanged += async (s, e) =>
                    {
                        var selected = (ComboBoxItem)statusCombo.SelectedItem;
                        var newStatus = (int)selected.Tag;

                        System.Diagnostics.Debug.WriteLine($"Изменение статуса: {currentMessageId} с {currentStatus} на {newStatus}");

                        if (newStatus == currentStatus) return;

                        var result = await _db.UpdateStatusAsync(currentMessageId, newStatus);
                        if (result)
                        {
                            currentStatus = newStatus;

                            // Обновляем визуальный индикатор статуса
                            var newStatusColor = GetStatusColor(newStatus);
                            var newStatusText = GetStatusText(newStatus);
                            statusBorder.Background = new SolidColorBrush(newStatusColor);
                            ((TextBlock)statusBorder.Child).Text = newStatusText;

                            // Обновляем карточку
                            await UpdateSingleMessage(currentMessageId);
                        }
                        else
                        {
                            statusCombo.SelectedIndex = currentStatus;
                            MessageBox.Show("Не удалось обновить статус!", "Ошибка",
                                          MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };
                    adminPanel.Children.Add(statusCombo);

                    // Кнопка комментария (временно скрыта)
                    var commentBtn = new Button
                    {
                        Content = "💬 Комментарий",
                        Margin = new Thickness(2),
                        Padding = new Thickness(8, 3, 8, 3),
                        Tag = currentMessageId,
                        Visibility = Visibility.Collapsed
                    };
                    adminPanel.Children.Add(commentBtn);

                    // Кнопка удаления
                    var deleteBtn = new Button
                    {
                        Content = "🗑️ Удалить",
                        Margin = new Thickness(2),
                        Padding = new Thickness(8, 3, 8, 3),
                        Foreground = new SolidColorBrush(Colors.Red)
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
                                    var card = spMessages.Children[i] as Border;
                                    if (card?.Tag?.ToString() == currentMessageId)
                                    {
                                        spMessages.Children.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Не удалось удалить сообщение!", "Ошибка",
                                              MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };
                    adminPanel.Children.Add(deleteBtn);

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
                return new Border { Background = new SolidColorBrush(Colors.LightPink), Child = new TextBlock { Text = "Ошибка отображения" } };
            }
        }

        private void ShowFullImage(byte[] imageData, string fileName)
        {
            var window = new Window
            {
                Title = fileName,
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowStyle = WindowStyle.ToolWindow,
                Background = new SolidColorBrush(Colors.Black),
                Content = new Grid()
            };

            var image = new System.Windows.Controls.Image
            {
                Stretch = System.Windows.Media.Stretch.Uniform,
                Margin = new Thickness(10)
            };

            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
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
                case 0: return Colors.Gray;
                case 1: return Color.FromRgb(255, 193, 7);
                case 2: return Color.FromRgb(76, 175, 80);
                case 3: return Color.FromRgb(244, 67, 54);
                default: return Colors.Gray;
            }
        }

        private string GetStatusText(int status)
        {
            switch (status)
            {
                case 0: return "Новое";
                case 1: return "В работе";
                case 2: return "Выполнено";
                case 3: return "Отложено";
                default: return "Новое";
            }
        }

        private void ChkAdminMode_Checked(object sender, RoutedEventArgs e)
        {
            txtAdminPassword.Visibility = Visibility.Visible;
        }

        private void ChkAdminMode_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAdminPassword.Visibility = Visibility.Collapsed;
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (chkAdminMode.IsChecked == true)
            {
                var isValid = await _db.VerifyAdminPasswordAsync(txtAdminPassword.Password);
                if (isValid)
                {
                    _isAdmin = true;
                    lblUserStatus.Text = "Режим администратора";
                    lblUserStatus.Foreground = new SolidColorBrush(Colors.Orange);
                    await LoadMessages();
                }
                else
                {
                    MessageBox.Show("Неверный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                _isAdmin = false;
                _currentUser = string.IsNullOrWhiteSpace(txtUserName.Text) ? "Аноним" : txtUserName.Text;
                _currentContact = txtUserContact.Text;
                lblUserStatus.Text = $"Вы вошли как {_currentUser}";
                lblUserStatus.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private void BtnAttach_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif|Все файлы|*.*";
            if (dialog.ShowDialog() == true)
            {
                _attachedFileData = File.ReadAllBytes(dialog.FileName);
                _attachedFileName = System.IO.Path.GetFileName(dialog.FileName);
                lblAttachedFile.Text = _attachedFileName;
            }
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewMessage.Text))
            {
                MessageBox.Show("Введите сообщение!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"=== ОТПРАВКА СООБЩЕНИЯ ===");
            System.Diagnostics.Debug.WriteLine($"Текст: {txtNewMessage.Text}");
            System.Diagnostics.Debug.WriteLine($"Имя: {_currentUser}");
            System.Diagnostics.Debug.WriteLine($"Файл: {_attachedFileName ?? "нет"}, размер: {_attachedFileData?.Length ?? 0}");

            var success = await _db.SendMessageAsync(
                txtNewMessage.Text,
                _isAdmin ? "Admin" : _currentUser,
                _currentContact,
                _attachedFileData,
                _attachedFileName
            );

            if (success)
            {
                txtNewMessage.Text = "";
                lblAttachedFile.Text = "";
                _attachedFileData = null;
                _attachedFileName = null;
                await LoadMessages();
                MessageBox.Show("Сообщение отправлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка отправки! Проверьте консоль Visual Studio для деталей.",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}