using System.Windows;
using System.Windows.Controls;

namespace TBMFurn
{
    public partial class PasswordDialog : Window
    {
        private const string CORRECT_PASSWORD = "admin123"; // Здесь установите свой пароль

        public bool IsPasswordCorrect { get; private set; }

        public PasswordDialog()
        {
            InitializeComponent();
            IsPasswordCorrect = false;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == CORRECT_PASSWORD)
            {
                IsPasswordCorrect = true;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Неверный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordBox.Clear();
                PasswordBox.Focus();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}