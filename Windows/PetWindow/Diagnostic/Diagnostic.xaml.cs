using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VDC_WPF_T.Windows.PetWindow.HealthState;

namespace VDC_WPF_T
{
    /// <summary>
    /// Interaction logic for Diagnostic.xaml
    /// </summary>
    public partial class Diagnostic : Window
    {        
        public string Email { get; set; } = "daria_kkge@gmail.com";

        public Pet _pet { get; set; } = new Pet();
        public Diagnostic(Pet pet)
        {
            _pet = pet;
            DataContext = this;
            InitializeComponent();
        }

        private void gif_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = new TimeSpan(0, 0, 1);
            ((MediaElement)sender).Play();
        }         

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HealthState npw = new HealthState(_pet);
            npw.ShowDialog();
        }        

        private void AddPic_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFile.ShowDialog() == true)
                _pet.PicSource = new Uri(openFile.FileName, UriKind.Absolute);

        }

        private async void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink != null)
            {
                var email = hyperlink.Inlines.FirstInline as Run;
                if (email != null)
                {
                    Clipboard.SetText(email.Text);

                    // Show the popup
                    popup.IsOpen = true;

                    // Close the popup after a short duration
                    await Task.Delay(2000); // Adjust the duration as needed
                    popup.IsOpen = false;
                }
            }
        }
    }
}
