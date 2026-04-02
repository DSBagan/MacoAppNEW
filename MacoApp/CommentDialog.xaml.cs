// CommentDialog.xaml.cs
using System.Windows;

namespace TBMFurn
{
    public partial class CommentDialog : Window
    {
        public string CommentText => txtComment.Text;

        public CommentDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}