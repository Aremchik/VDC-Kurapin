using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using VDC_WPF_T.Windows.PetWindow.HealthState;
using VDC_WPF_T.Windows.PetWindow.TreatmentPlan;

namespace VDC_WPF_T
{
    /// <summary>
    /// Логика взаимодействия для NewPetWindow.xaml
    /// </summary>
    public partial class PetWindow : Window
    {
        /*        private Uri _mediaUri;

                public Uri MediaUri
                {
                    get { return _mediaUri; }
                    set
                    {
                        _mediaUri = value;
                        OnPropertyChanged(nameof(MediaUri)); // Notify property change
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;

                protected virtual void OnPropertyChanged(string propertyName)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
        */

        public string Email { get; set; } = "daria_kkge@gmail.com";

        public Pet _pet { get; set; } = new Pet();
        public PetWindow(Pet pet)
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


        private void Button_Click()
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void AddNewPet_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                NewPetWindow npw = new NewPetWindow();
                npw.ShowDialog();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HealthState npw = new HealthState(_pet);
            npw.ShowDialog();
        }

        private void Button_Click_Current_Health_State(object sender, RoutedEventArgs e)
        {
            Diagnostic dia = new Diagnostic(_pet);
            dia.Loaded += async (sender, e) =>
            {
                await dia.RefreshTest();
            };
            dia.ShowDialog();
            //Task.Run(async () => await dia.RefreshTest());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            TreatmentPlan npw = new TreatmentPlan(_pet);
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
