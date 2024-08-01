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


namespace TBMFurn
{
    public partial class PortalWindow : Window
    {
        private DispatcherTimer _timer;
        private Thickness _originalMargin;
        string Side;

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
            EntryiWindow entryiWindow = new EntryiWindow();
            entryiWindow.Show();
            this.Close();
        }

        
    }
}
