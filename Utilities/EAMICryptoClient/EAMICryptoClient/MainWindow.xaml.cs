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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OHC.EAMI;
using OHC.EAMI.Util;

namespace EAMICryptoClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtClearText.KeyUp += EnterClicked;
            txtEncryptedValue.IsEnabled = false;
            UpdateUI();
        }

        void UpdateUI()
        {
            btnResetForm.IsEnabled = !string.IsNullOrEmpty(txtClearText.Text);
            btnEncrypt.IsEnabled = !string.IsNullOrEmpty(txtClearText.Text);
            btnCopyToClipboard.IsEnabled = !string.IsNullOrEmpty(txtEncryptedValue.Text);
            txtEncryptedValue.IsEnabled = !string.IsNullOrEmpty(txtEncryptedValue.Text);
        }
        void EnterClicked(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnEncrypt_Click(null, null);
                e.Handled = true;
            }

            UpdateUI();
        }

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            txtEncryptedValue.Text = Crypto.EncryptString(Crypto.ToSecureString(txtClearText.Text.Trim()));

            UpdateUI();
        }

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEncryptedValue.Text))
            {
                Clipboard.SetText(txtEncryptedValue.Text);
                MessageBox.Show("Encrypted text copied to the Clipboard.");
            }
            else
            {
                MessageBox.Show("There is no encrypted text to copy to the Clipboard.");
            }
        }

        private void btnResetForm_Click(object sender, RoutedEventArgs e)
        {
            txtClearText.Clear();
            txtEncryptedValue.Clear();
            txtEncryptedValue.IsEnabled = false;
            Clipboard.Clear();
            UpdateUI();
        }
    }
}
