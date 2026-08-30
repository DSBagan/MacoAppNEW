using MacoApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using MaterialDesignMessageBox;
using System.Collections;
using System.Data;
using System.IO;


namespace TBMFurn
{
    public partial class PortalWindow : Window
    {
        private DispatcherTimer _timer;
        private Thickness _originalMargin;
        string Side = "";
        public int quantityBar = 1;
        SqlRequests sqlRequests = new SqlRequests();
        ClassError classError = new ClassError();
        public ObservableCollection<ClassList> ClassLists { get; set; }

        private System.Windows.Threading.DispatcherTimer _textTimer;
        private int _currentIndex;
        private readonly string[] _texts = new[]
        {    "Есть предложение по улучшению программы? Напиши \u2192",    "Нашел ошибку? Напиши \u2192",    "Есть неточность? Напиши \u2192",
            "Обязательно проверь расчет после подгрузки в КиС!!!"
        };

        DataTable table1 = new DataTable("Table1"); //Таблица для сохранения расчета

        string SavePathTXT;

        private ObservableCollection<BitmapImage> backgroundsFON = new ObservableCollection<BitmapImage>(); //Коллекция фонов для таблицы

        private bool _isPaused = false;
        private int _pauseCounter = 0;

        List<string> response_bars = new List<string> { "34283", "34943", "V25010102", "V25020102", "V26010102", "V25040102", "V25070102", "260367", "332438", "338019", "332439", "338070", "260360" };

        // Добавляем флаг для отслеживания состояния очистки
        private bool _isCleaningUp = false;

        public PortalWindow()
        {
            InitializeComponent();
            StartTextAnimation(); // Запускаем анимацию текстблока обратной связи

            LoadBackgrounds();
            InitializeDataTable();

            _originalMargin = LabelStvorka.Margin;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.05)
            };
            _timer.Tick += Timer_Tick;

            LabelErrorСode.Visibility = Visibility.Hidden;
            LabelErrorFFB.Visibility = Visibility.Hidden;
            LabelErrorFFH.Visibility = Visibility.Hidden;
            LabelErrorR.Visibility = Visibility.Hidden;
            LabelErrorL.Visibility = Visibility.Hidden;
            ButtonSaveCalc.IsEnabled = false;

            // Подписываемся на событие закрытия окна
            this.Closed += OnWindowClosed;
        }

        private void LoadBackgrounds()
        {
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/MacoFonPortal.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/vorneFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/RotoFonPortal.png")));
        }

        private void InitializeDataTable()
        {
            table1.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table1.Columns.Add(new DataColumn("Название", typeof(string)));
            table1.Columns.Add(new DataColumn("Шт", typeof(int)));
        }

        private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
        {
            table1.Rows.Clear();
            GridList.ItemsSource = null;

            if (Side == "")
            {
                // Показываем изображение стрелки и запускаем анимацию
                LabelErrorR.Visibility = Visibility.Visible;
                LabelErrorL.Visibility = Visibility.Visible;
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                LabelErrorR.BeginAnimation(UIElement.OpacityProperty, animation);
                LabelErrorL.BeginAnimation(UIElement.OpacityProperty, animation);
                return;
            }
            else
            {
                LabelErrorR.Visibility = Visibility.Hidden;
                LabelErrorL.Visibility = Visibility.Hidden;
            }

            if (TextBoxFFB.Text == "" || TextBoxFFH.Text == "")
            {
                if (TextBoxFFB.Text == "")
                {
                    // Показываем изображение стрелки и запускаем анимацию
                    LabelErrorFFB.Visibility = Visibility.Visible;
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    LabelErrorFFB.BeginAnimation(UIElement.OpacityProperty, animation);
                }
                else
                {
                    LabelErrorFFB.Visibility = Visibility.Hidden;
                }
                if (TextBoxFFH.Text == "")
                {
                    // Показываем изображение стрелки и запускаем анимацию
                    LabelErrorFFH.Visibility = Visibility.Visible;
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    LabelErrorFFH.BeginAnimation(UIElement.OpacityProperty, animation);
                }
                else
                {
                    LabelErrorFFH.Visibility = Visibility.Hidden;
                }
                return;
            }
            else
            {
                LabelErrorFFB.Visibility = Visibility.Hidden;
                LabelErrorFFH.Visibility = Visibility.Hidden;
            }

            try
            {
                int count = 0;
                string queryString;
                string Furn = ComboBoxFurn.Text;
                int quantity = Int32.Parse(TextBoxColvo.Text);
                string System = ComboBoxProfile.Text;
                int FFH = Int32.Parse(TextBoxFFH.Text);
                int FFB = Int32.Parse(TextBoxFFB.Text);
                string color = ComboBoxColor.Text;

                queryString = $"Select * from Portal where (Furn like '" + Furn + "') and(Profil  = 'Не имеет значения' or Profil  = '" + System + "') and(Side like 'Не имеет значения' or Side like '" + Side + "') " +
                    "and(FFH_before = 0 or '" + FFH + "'<=FFH_before) and(FFH_from = 0 or '" + FFH + "' >= FFH_from)" +
                    " and(FFB_before = 0 or '" + FFB + "'<=FFB_before) and(FFB_from = 0 or '" + FFB + "' >= FFB_from) " +
                    " and(Color  = 'Не имеет значения' or Color  = '" + color + "')";

                if (classError.ErrorPortal(Furn, FFB, FFH, quantity) == 1)
                {
                    return;
                }

                using (var connection = new SqliteConnection("Data Source=Furnapp.db"))
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridList.ItemsSource = collection;
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(queryString, connection);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                count++;
                                //Записываем данные в объект класса и ,тем самым, передаём в таблицу 
                                if (response_bars.Contains(reader.GetValue(2).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(2).ToString(), Название = "" + reader.GetValue(3).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityBar });
                                }
                                else
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(2).ToString(), Название = "" + reader.GetValue(3).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) });
                                }
                            }
                            CopyCollectionToDataTable(collection);
                        }
                    }
                    connection.Close();
                }
                ButtonSaveCalc.IsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в ButtonCalculation_Click: {ex.Message}");
                return;
            }
        }

        public DataTable CopyCollectionToDataTable(ObservableCollection<ClassList> collection)
        {
            // Копирование данных из коллекции в DataTable
            foreach (ClassList item in collection)
            {
                DataRow row = table1.NewRow();
                row["Артикул"] = item.Артикул;
                row["Название"] = item.Название;
                row["Шт"] = item.Шт;
                table1.Rows.Add(row);
            }

            // Возврат DataTable
            return table1;
        }

        private void ButtonSaveCalc_Click(object sender, RoutedEventArgs e)
        {
            int CTlangth = TextBoxCode.Text.Length;
            if (CTlangth == 0)
            {
                // Показываем изображение стрелки и запускаем анимацию
                LabelErrorСode.Visibility = Visibility.Visible;
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, animation);
                return;
            }
            else if (CTlangth < 6)
            {
                for (int i = 0; i < 6 - CTlangth; i++)
                {
                    TextBoxCode.Text = "0" + TextBoxCode.Text;
                }
            }

            try
            {
                // Проверяем есть диск X и папка aTBMFURN, если нет- создаем
                Directory.CreateDirectory(@"X:\aTBMFURN\");
                SavePathTXT = @"X:\aTBMFURN\";
                SaveMetod();
            }
            catch (System.Exception)
            {
                // Проверяем есть ли на диске C папка aTBMFURN, если нет- создаем
                Directory.CreateDirectory(@"C:\aTBMFURN\");
                MessageBox.Show("Нет доступа к диску X, файл будет сохранен в папку aTBMFURN на диске C");
                SavePathTXT = @"C:\aTBMFURN\";
                SaveMetod();
            }
        }

        private void SaveMetod()
        {
            String date = DateTime.Now.ToString(" dd.MM.yyyy HH-mm-ss");

            using (StreamWriter streamWriter = new StreamWriter(SavePathTXT + "Z" + TextBoxCode.Text + " " + date + " Портал" + ".txt", false, Encoding.Default))
            {
                streamWriter.WriteLine("                    Шифр фирмы " + TextBoxCode.Text);
                streamWriter.WriteLine("                    Фирма 123");
                streamWriter.WriteLine("                    Заявка №");
                streamWriter.WriteLine("                    Название");
                streamWriter.WriteLine("                    Дата заявки" + date);
                streamWriter.WriteLine("--------------------------------------------------------------------------------");
                streamWriter.WriteLine("    Артикул                       Название                      Кол.  Ед.изм.");
                streamWriter.WriteLine("--------------------------------------------------------------------------------");
                try
                {
                    foreach (DataRow row in table1.Rows)
                    {
                        string art = Convert.ToString(row["Артикул"]);
                        string nam = Convert.ToString(row["Название"]);
                        int qua = Convert.ToInt32(row["Шт"]);

                        if (art.Length < 16)
                        {
                            int b = 16 - art.Length;
                            for (int i = 0; i < b; i++)
                            {
                                art += " ";
                            }
                        }
                        string n = "                                                ";
                        streamWriter.WriteLine(art + n + qua);
                    }

                    streamWriter.WriteLine("--------------------------------------------------------------------------------");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("                    Заявку составил________________________");

                    streamWriter.Close();

                    MaterialMessageBox.ShowDialog("Файл успешно сохранен");
                    // если в TextBox есть символы
                    // Скрываем изображение стрелки
                    LabelErrorСode.Visibility = Visibility.Hidden;
                    LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
                }
                catch
                {
                    MaterialMessageBox.ShowDialog("Ошибка при сохранении файла!");
                }
                ButtonSaveCalc.IsEnabled = false;
                table1.Rows.Clear();
                GridList.ItemsSource = null;
            }
        }

        //Обработчик комбобокс
        private void ComboBoxFurn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Меняем фон Gridlist в зависимости от выбранной фурнитуры
            int index = ComboBoxFurn.SelectedIndex;
            if (index >= 0 && index < backgroundsFON.Count)
            {
                BitmapImage image = backgroundsFON[index];
                ImageBrush brush = new ImageBrush(image);
                GridList.Background = brush;
            }

            if (ComboBoxFurn != null && ComboBoxFurn.SelectedIndex == 0)
            {
                if (ComboBoxProfile != null)
                {
                    ComboBoxProfile.Items.Clear();
                    ComboBoxProfile.Items.Add("KBEAD70");
                    ComboBoxProfile.Items.Add("Veka");
                    ComboBoxProfile.Items.Add("Дерево");
                    ComboBoxProfile.SelectedIndex = 0;
                }

                if (ComboBoxColor != null)
                {
                    ComboBoxColor.Items.Clear();
                    ComboBoxColor.Items.Add("Белый");
                    ComboBoxColor.Items.Add("Коричневый");
                    ComboBoxColor.Items.Add("Бронза");
                    ComboBoxColor.Items.Add("Серебро");
                    ComboBoxColor.SelectedIndex = 0;
                }
            }
            else if (ComboBoxFurn != null && ComboBoxFurn.SelectedIndex == 1)
            {
                if (ComboBoxProfile != null)
                {
                    ComboBoxProfile.Items.Clear();
                    ComboBoxProfile.Items.Add("KBEAD70");
                    ComboBoxProfile.Items.Add("KBE58");
                    ComboBoxProfile.Items.Add("Rehau");
                    ComboBoxProfile.Items.Add("Thyssen");
                    ComboBoxProfile.Items.Add("Gealan");
                    ComboBoxProfile.SelectedIndex = 0;
                }
                if (ComboBoxColor != null)
                {
                    ComboBoxColor.Items.Clear();
                    ComboBoxColor.Items.Add("Белый");
                    ComboBoxColor.Items.Add("Коричневый");
                    ComboBoxColor.SelectedIndex = 0;
                }

            }
            else if (ComboBoxFurn != null && ComboBoxFurn.SelectedIndex == 2)
            {
                if (ComboBoxProfile != null)
                {
                    ComboBoxProfile.Items.Clear();
                    ComboBoxProfile.Items.Add("KBEAD70");
                    ComboBoxProfile.Items.Add("KBE58");
                    ComboBoxProfile.Items.Add("Rehau");
                    ComboBoxProfile.Items.Add("Дерево");
                    ComboBoxProfile.Items.Add("Gealan");
                    ComboBoxProfile.SelectedIndex = 0;
                }
                if (ComboBoxColor != null)
                {
                    ComboBoxColor.Items.Clear();
                    ComboBoxColor.Items.Add("Белый");
                    ComboBoxColor.Items.Add("Бронза");
                    ComboBoxColor.Items.Add("Серебро");
                    ComboBoxColor.SelectedIndex = 0;
                }
            }
        }

        //ввод только цифр в текстбоксы
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ButtonSaveCalc.IsEnabled = false;
            int val;
            if (!Int32.TryParse(e.Text, out val) && e.Text != "-")
            {
                e.Handled = true; // отклоняем ввод
            }
        }

        //ввод только цифр в текстбоксы (пробел тоже нам не нужен)
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ButtonSaveCalc.IsEnabled = false;
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }
        }

        private void ButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            ChangeDirection("Влево");
        }

        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            ChangeDirection("Вправо");
        }

        private void ChangeDirection(string direction)
        {
            if (ButtonLeft != null && ButtonRight != null)
            {
                if (direction == "Влево")
                {
                    ButtonLeft.BorderBrush = Brushes.Red;
                    ButtonRight.BorderBrush = Brushes.White;
                }
                else if (direction == "Вправо")
                {
                    ButtonLeft.BorderBrush = Brushes.White;
                    ButtonRight.BorderBrush = Brushes.Red;
                }

                Side = direction;
                if (LabelStvorka != null)
                {
                    LabelStvorka.Margin = _originalMargin;
                }
                if (_timer != null)
                {
                    _timer.Start();
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isPaused && LabelStvorka != null)
            {
                if (Side == "Влево")
                {
                    if (LabelStvorka.Margin.Left > 30)
                    {
                        LabelStvorka.Margin = new Thickness(LabelStvorka.Margin.Left - 1, LabelStvorka.Margin.Top, LabelStvorka.Margin.Right + 1, LabelStvorka.Margin.Bottom);
                    }
                }
                else if (Side == "Вправо")
                {
                    if (LabelStvorka.Margin.Right > 30)
                    {
                        LabelStvorka.Margin = new Thickness(LabelStvorka.Margin.Left + 1, LabelStvorka.Margin.Top, LabelStvorka.Margin.Right - 1, LabelStvorka.Margin.Bottom);
                    }
                }
            }
        }

        private void ButtonFeedback_Click(object sender, RoutedEventArgs e)
        {
            FeedbackWindow feedbackWindow = new FeedbackWindow();
            feedbackWindow.Show();
        }

        private void StartTextAnimation()
        {
            _currentIndex = 0;

            // Создаем таймер
            _textTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _textTimer.Tick += OnTextTimerTick;
            _textTimer.Start();
        }

        private void OnTextTimerTick(object sender, EventArgs e)
        {
            // Анимация исчезновения текста
            var fadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            fadeOut.Completed += OnFadeOutCompleted;
            TextBlockFeedback.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            // Обновляем текст
            TextBlockFeedback.Text = _texts[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _texts.Length;

            // Анимация появления текста
            var fadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(1));
            TextBlockFeedback.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            if (_isCleaningUp) return;
            _isCleaningUp = true;

            try
            {
                // Останавливаем и очищаем таймер анимации текста
                if (_textTimer != null)
                {
                    _textTimer.Stop();
                    _textTimer.Tick -= OnTextTimerTick;
                    _textTimer = null;
                }

                // Останавливаем и очищаем таймер движения
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= Timer_Tick;
                    _timer = null;
                }

                // Отписываемся от событий
                this.Closed -= OnWindowClosed;

                // Отписываемся от событий кнопок
                ButtonLeft.Click -= ButtonLeft_Click;
                ButtonRight.Click -= ButtonRight_Click;
                ButtonSaveCalc.Click -= ButtonSaveCalc_Click;
                ButtonFeedback.Click -= ButtonFeedback_Click;
                ButtonExit.Click -= ButtonExit_Click;

                // Отписываемся от событий ComboBox
                ComboBoxFurn.SelectionChanged -= ComboBoxFurn_SelectionChanged;
                ComboBoxColor.SelectionChanged -= ComboBoxColor_SelectionChanged;

                // Отписываемся от событий TextBox
                TextBoxFFB.PreviewTextInput -= TextBox_PreviewTextInput;
                TextBoxFFB.PreviewKeyDown -= TextBox_PreviewKeyDown;
                TextBoxFFH.PreviewTextInput -= TextBox_PreviewTextInput;
                TextBoxFFH.PreviewKeyDown -= TextBox_PreviewKeyDown;
                TextBoxColvo.PreviewTextInput -= TextBox_PreviewTextInput;
                TextBoxColvo.PreviewKeyDown -= TextBox_PreviewKeyDown;
                TextBoxCode.PreviewTextInput -= TextBox_PreviewTextInput;
                TextBoxCode.PreviewKeyDown -= TextBox_PreviewKeyDown;

                // Останавливаем анимации
                TextBlockFeedback.BeginAnimation(UIElement.OpacityProperty, null);
                LabelErrorR.BeginAnimation(UIElement.OpacityProperty, null);
                LabelErrorL.BeginAnimation(UIElement.OpacityProperty, null);
                LabelErrorFFB.BeginAnimation(UIElement.OpacityProperty, null);
                LabelErrorFFH.BeginAnimation(UIElement.OpacityProperty, null);
                LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, null);

                // Очищаем коллекцию фонов
                if (backgroundsFON != null)
                {
                    foreach (var bg in backgroundsFON)
                    {
                        bg.UriSource = null;
                    }
                    backgroundsFON.Clear();
                }

                // Очищаем таблицу
                if (table1 != null)
                {
                    table1.Clear();
                    table1.Dispose();
                }

                // Очищаем список ответных планок
                response_bars?.Clear();

                // Очищаем GridList
                GridList.ItemsSource = null;

                // Вызываем GC для принудительной сборки мусора
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при очистке ресурсов PortalWindow: {ex.Message}");
            }
        }

        private void ComboBoxColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxFurn.SelectedIndex == 0)
                {
                    if (ComboBoxColor.SelectedIndex == 0)
                    {
                        ButtonColor.Background = Brushes.White;
                    }
                    else if (ComboBoxColor.SelectedIndex == 1)
                    {
                        ButtonColor.Background = Brushes.Brown;
                    }
                    else if (ComboBoxColor.SelectedIndex == 2)
                    {
                        ButtonColor.Background = Brushes.Orange;
                    }
                    else if (ComboBoxColor.SelectedIndex == 3)
                    {
                        ButtonColor.Background = Brushes.Silver;
                    }
                }
                else if (ComboBoxFurn.SelectedIndex == 1)
                {
                    if (ComboBoxColor.SelectedIndex == 0)
                    {
                        ButtonColor.Background = Brushes.White;
                    }
                    else if (ComboBoxColor.SelectedIndex == 1)
                    {
                        ButtonColor.Background = Brushes.Brown;
                    }
                }
                else if (ComboBoxFurn.SelectedIndex == 2)
                {
                    if (ComboBoxColor.SelectedIndex == 0)
                    {
                        ButtonColor.Background = Brushes.White;
                    }
                    else if (ComboBoxColor.SelectedIndex == 1)
                    {
                        ButtonColor.Background = Brushes.Orange;
                    }
                    else if (ComboBoxColor.SelectedIndex == 2)
                    {
                        ButtonColor.Background = Brushes.Silver;
                    }
                }
            }
            catch (System.Exception)
            {
                return;
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}