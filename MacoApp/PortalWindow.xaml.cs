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
using Microsoft.Vbe.Interop.Forms;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;


namespace TBMFurn
{
    public partial class PortalWindow : Window
    {
        private DispatcherTimer _timer;
        private Thickness _originalMargin;
        string Side = "";

        SqlRequests sqlRequests = new SqlRequests();
        ClassError classError = new ClassError();
        public ObservableCollection<ClassList> ClassLists { get; set; }

        public PortalWindow()
        {
            InitializeComponent();
            StartTextAnimation(); // Запускаем анимацию текстблока обратной связи

            _originalMargin = LabelStvorka.Margin;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.05)
            };
            _timer.Tick += Timer_Tick;
        }


        private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
        {
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



                /*if (classError.Err(Furn, FFH, FFB, quantity, rotation, framuga, konst, shtulp, shtulpTreeArg) == 1)
                {
                    return;
                }*/

                queryString = $"Select * from Portal where (Furn like '" + Furn + "') and(Profil  = 'Не имеет значения' or Profil  = '" + System + "') and(Side like 'Не имеет значения' or Side like '" + Side + "') " +
                    "and(FFH_before = 0 or '" + FFH + "'>=FFH_before) and(FFH_from = 0 or '" + FFH + "' <= FFH_from)" +
                    " and(FFB_before = 0 or '" + FFB + "'>=FFB_before) and(FFB_from = 0 or '" + FFB + "' <= FFB_from) " +
                    " and(Color  = 'Не имеет значения' or Color  = '" + color + "')";
                /*quantityBar = sqlRequests.Que(rotation, framuga, Furn, FFH, FFB); //Вытаскиваем из класса количество ответных планок
                quantitySrPr = sqlRequests.QueSrPr(rotation, Furn, FFH); //Количество средних прижимов на поворотной створке
                quantityShtulp = sqlRequests.QueShtup(shtulp, Furn, FFH, shtulpTreeArg);  //Количество штульповых ответных планок 
                quantityBarShtulp = sqlRequests.QueShtupOtvet(shtulp, Furn, FFH);  //Количество обычных ответных планок в штульповом окне*/

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
                                /*if (shtulp != "Да" && response_bars.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityBar });
                                }
                                else if (shtulp == "Да" && response_bars.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityBarShtulp });
                                }
                                else if (SrPr.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantitySrPr });
                                }
                                else if (ListStulpOtv.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityShtulp });
                                }
                                else if (framuga == "Да" && ArticleFram1.Contains(reader.GetValue(3).ToString()) && FFH <= 1600)
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * 2 });
                                }
                                else if (framuga == "Да" && ArticleFram1.Contains(reader.GetValue(3).ToString()) && FFH >= 1601 && FFH <= 2400)
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * 3 });
                                }
                                else if (framuga == "Да" && ArticleFram2.Contains(reader.GetValue(3).ToString()) && FFH >= 1101)
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * 2 });
                                }
                                else if (shtulp == "Да" && ListStulp.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * 2 });
                                }*/

                                collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) });

                            }
                        }
                    }
                    connection.Close();
                }
                ButtonSaveCalc.IsEnabled = true;
            }
            catch
            {
                //MaterialMessageBox.ShowDialog("Одно или несколько полей пустое");
                return;
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
            Side = "Left";
            LabelStvorka.Margin = _originalMargin;
            _timer.Start();
        }

        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            Side = "Right";
            LabelStvorka.Margin = _originalMargin;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Side == "Left")
            {
                if (LabelStvorka.Margin.Left > 38)
                {
                    LabelStvorka.Margin = new Thickness(LabelStvorka.Margin.Left - 1, LabelStvorka.Margin.Top, LabelStvorka.Margin.Right + 1, LabelStvorka.Margin.Bottom);
                }
                else
                {
                    LabelStvorka.Margin = _originalMargin;
                }
            }
            else if (Side == "Right")
            {
                if (LabelStvorka.Margin.Right > 38)
                {
                    LabelStvorka.Margin = new Thickness(LabelStvorka.Margin.Left + 1, LabelStvorka.Margin.Top, LabelStvorka.Margin.Right -1, LabelStvorka.Margin.Bottom);
                }
                else
                {
                    LabelStvorka.Margin = _originalMargin;
                }
            }
            
        }

        private void ButtonFeedback_Click(object sender, RoutedEventArgs e)
        {
            FeedbackWindow feedbackWindow = new FeedbackWindow();
            feedbackWindow.Show();
        }

        //Анимация текстблока обратной связи
        private void StartTextAnimation()
        {
            string[] texts = new string[] { "Есть предложение по улучшению программы? Напиши \u2192", "Нашел ошибку? Напиши \u2192", "Есть неточность? Напиши \u2192", "Обязательно проверь расчет после подгрузки в КиС!!!" }; // Замените на свои тексты
            int currentIndex = 0;

            // Создаем таймер, который будет вызывать смену текста каждые 20 секунд
            System.Windows.Threading.DispatcherTimer textTimer = new System.Windows.Threading.DispatcherTimer();
            textTimer.Interval = TimeSpan.FromSeconds(30);
            textTimer.Tick += (sender, e) =>
            {
                var fadeOut = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
                fadeOut.Completed += (s, a) =>
                {
                    TextBlockFeedback.Text = texts[currentIndex];
                    var fadeIn = new DoubleAnimation(1, (Duration)TimeSpan.FromSeconds(1));
                    TextBlockFeedback.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };
                TextBlockFeedback.BeginAnimation(UIElement.OpacityProperty, fadeOut);

                currentIndex = (currentIndex + 1) % texts.Length;
            };

            textTimer.Start();
        }

        private void ComboBoxColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxColor.SelectedIndex >= 0)
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
                    else if (ComboBoxColor.SelectedIndex == 4)
                    {
                        ButtonColor.Background = Brushes.Gray;
                    }
                    else if (ComboBoxColor.SelectedIndex == 5)
                    {
                        ButtonColor.Background = Brushes.Black;
                    }
                    else if (ComboBoxColor.SelectedIndex == 6)
                    {
                        ButtonColor.Background = Brushes.Gold;
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
            /*EntryiWindow entryiWindow = new EntryiWindow();
            entryiWindow.Show();*/
            this.Close();
        }

        
    }
}
