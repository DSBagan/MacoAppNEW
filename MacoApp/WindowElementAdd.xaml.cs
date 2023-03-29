using System.Windows;

namespace MacoApp
{
    public partial class WindowElementAdd : Window
    {
        public Element Element { get; private set; }
        public WindowElementAdd(Element element)
        {
            InitializeComponent();
            Element = element;
            DataContext = Element;
        }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
