using System;
using System.Collections.Generic;
using System.Windows;

namespace MacoApp
{
    public partial class ChangelogWindow : Window
    {
        public ChangelogWindow()
        {
            InitializeComponent();
            LoadChangelog();
        }

        private void LoadChangelog()
        {
            var changelog = new List<VersionChanges>
            {
                new VersionChanges
                {
                    Version = "Подгрузка в КИС",
                    Date = "02.09.2026",
                    Changes = new List<string>
                    {
                        " Добавлена кнопка 'Сохранить для подгрузки в счет'\n" +
                        "Она создает файл в формате .txt со списком обработанных артикулов, который можно\n " +
                        "подгрузить в счет привычным способом.",
                        " Добавлена кнопка 'Сохранить в Excel для КП в КИС'-\n" +
                        "она сохраненяет список артикулов в формате .xls для КП в старом КИСе (запоминаем\n" +
                        "количество строк в обработанном списке, это нужно указать при подгрузке в КП),\n" +
                        "старую кнопку для КИС 2.0 тоже оставил (вдруг есть любители мучений),\n" +
                        "переименовал её в 'Сохранить в Excel для КП в КИС 2.0'\n",
                        " Добавлена информация о дублированных артикулах при обработке и о округлении\n" +
                        "крепежа и уплотнителя.",
                        " Все расчеты и файлы для подгрузки теперь сохраняются в папку 'Подгрузка в КИС',\n" +
                        "которая создаётся на вашем диске X, а при его недоступности на диске C.",
                        " Уплотнитель округляется по формуле:\n" +
                        "  -Если количество > 2/3 бухты → округляем до бухты;\n" +
                        "  -Если количество < 2/3 бухты → увеличиваем на 10%, округляя до 5м;\n" +
                        "  -Исключение для артикулов ALM770071-02/1 и ALM770071-02 → округляем до 35\n" +
                        "  т.к. огромная неудобная для размотки резина и бухта соответственно.",
                        " Крепеж округляется до сотен."
                    }
                },
                new VersionChanges
                {
                    Version = "Фурнитура ПВХ",
                    Date = "02.09.2026",
                    Changes = new List<string>
                    {
                        " Добавлена фурнитура 'Internika NEW', полностью идентичная обычной\n" +
                        "Интернике, только считается с основными запорами, у которых бОльшее количество\n" +
                        "цапф, а так же взамен нижнего шпингалета считается угловик и прямой средний запор.\n"
                        
                    }
                },
                new VersionChanges
                {
                    /*Version = "v0.8.0",
                    Date = "20.08.2026",
                    Changes = new List<string>
                    {
                        "🚀 Первый релиз программы",
                        "📥 Загрузка каталога из локальной БД",
                        "📤 Экспорт в Excel (.xlsx)",
                        "🎨 Базовый дизайн с MaterialDesign"
                    }*/
                }
            };

            ChangelogItems.ItemsSource = changelog;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class VersionChanges
    {
        public string Version { get; set; }
        public string Date { get; set; }
        public List<string> Changes { get; set; }
    }
}