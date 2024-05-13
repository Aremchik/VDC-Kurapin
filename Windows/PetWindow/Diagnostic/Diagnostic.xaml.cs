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
using VDC_WPF_T.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts.Wpf; // Ставите с NuGet LiveCharts и LiveCharts.Wpf для графиков
using LiveCharts;
using System.Collections.ObjectModel;
using LiveCharts.Configurations;
using System.Threading;

namespace VDC_WPF_T
{    
    public partial class Diagnostic : Window, INotifyPropertyChanged
    {       
        private static DiagnosticData data = new DiagnosticData(0, 0, 0, new List<MeasureModel>(), new List<MeasureModel>(), new List<MeasureModel>());

        private ChartValues<MeasureModel> temperatureValues = new ChartValues<MeasureModel>(data.TemperatureList);
        private ChartValues<MeasureModel> respiratoryValues = new ChartValues<MeasureModel>(data.RespiratoryRateList);
        private ChartValues<MeasureModel> heartValues = new ChartValues<MeasureModel>(data.HeartRateList);
        private AxesCollection xAxesTemperature = new AxesCollection()
        {
            new Axis // Основная ось
            {
                MinValue = axisMinX,
                MaxValue = axisMaxX,
                Unit = AxisUnitX,
                LabelFormatter = value => new DateTime((long)value).ToString("HH:mm"),
                Separator = new LiveCharts.Wpf.Separator()
                {
                    Step = TimeSpan.FromHours(6).Ticks,
                    StrokeThickness = 0
                },
                Foreground = new SolidColorBrush(Colors.Black),                
            },
            new Axis // Ось для сепараторов
            {
                MinValue = axisMinX,
                MaxValue = axisMaxX,
                Unit = AxisUnitX,
                LabelFormatter = (value) => "",
                Separator = new LiveCharts.Wpf.Separator()
                {
                    Step = TimeSpan.FromHours(1).Ticks,
                    StrokeThickness = 3,
                    Stroke = new SolidColorBrush(Colors.LightSeaGreen),
                },
            }
        };
        private AxesCollection xAxesRespiratory = new AxesCollection() // Каждый график хочет свои оси, иначе они дерутся за них
        {
            new Axis // Основная ось
            {
                MinValue = axisMinX,
                MaxValue = axisMaxX,
                Unit = AxisUnitX,
                LabelFormatter = value => new DateTime((long)value).ToString("HH:mm"),
                Separator = new LiveCharts.Wpf.Separator()
                {
                    Step = TimeSpan.FromHours(6).Ticks,
                    StrokeThickness = 0
                },
                Foreground = new SolidColorBrush(Colors.Black),
            },
            new Axis // Ось для сепараторов
            {
                MinValue = axisMinX,
                MaxValue = axisMaxX,
                Unit = AxisUnitX,
                LabelFormatter = (value) => "",
                Separator = new LiveCharts.Wpf.Separator()
                {
                    Step = TimeSpan.FromHours(1).Ticks,
                    StrokeThickness = 3,
                    Stroke = new SolidColorBrush(Colors.LightSeaGreen),
                },
            }
        };
        private AxesCollection xAxesHeart = new AxesCollection()
        {
            new Axis // Основная ось
            {
                MinValue = axisMinX,
                MaxValue = axisMaxX,
                Unit = AxisUnitX,
                LabelFormatter = value => new DateTime((long)value).ToString("HH:mm"),
                Separator = new LiveCharts.Wpf.Separator()
                {
                    Step = TimeSpan.FromHours(6).Ticks,
                    StrokeThickness = 0
                },
                Foreground = new SolidColorBrush(Colors.Black),
            },
            new Axis // Ось для сепараторов
            {
                MinValue = axisMinX,
                MaxValue = axisMaxX,
                Unit = AxisUnitX,
                LabelFormatter = (value) => "",
                Separator = new LiveCharts.Wpf.Separator()
                {
                    Step = TimeSpan.FromHours(1).Ticks,
                    StrokeThickness = 3,
                    Stroke = new SolidColorBrush(Colors.LightSeaGreen),
                },
            }
        };

        private static bool minModifierToggled = false;
        private static int minModifier = 23;

        private static double axisMinX = DateTime.Now.Date.Ticks + TimeSpan.FromHours(DateTime.Now.Hour).Ticks + TimeSpan.FromHours(1).Ticks - TimeSpan.FromHours(minModifier).Ticks; // Там короче нужно вырезать минуты и секунды чтобы нормально считало
        private static double axisMaxX = DateTime.Now.Date.Ticks + TimeSpan.FromHours(DateTime.Now.Hour).Ticks + TimeSpan.FromHours(1).Ticks; // поэтому тут идет текущая дата + текущий час и потом уже остальное

        public DiagnosticData Data { get { return data; } private set { data = value; OnPropertyChanged("Data");  } }
        public ChartValues<MeasureModel> TemperatureValues { get { return temperatureValues; } private set { temperatureValues = value; OnPropertyChanged("TemperatureValues"); } }
        public ChartValues<MeasureModel> RespiratoryValues { get { return respiratoryValues; } private set { respiratoryValues = value; OnPropertyChanged("RespiratoryValues"); } }
        public ChartValues<MeasureModel> HeartValues { get { return heartValues; } private set { heartValues = value; OnPropertyChanged("HeartValues"); } }

        public double AxisMinX { get { return axisMinX; } private set { axisMinX = value; OnPropertyChanged("AxisMinX"); } }
        public double AxisMaxX { get { return axisMaxX; } private set { axisMaxX = value; OnPropertyChanged("AxisMaxX"); } }
        public static double AxisUnitX { get; set; } = TimeSpan.TicksPerHour;                       
        
        public AxesCollection XAxesTemperature { get { return xAxesTemperature; } private set { xAxesTemperature = value; OnPropertyChanged("XAxesTemperature"); } }
        public AxesCollection XAxesRespiratory { get { return xAxesRespiratory; } private set { xAxesRespiratory = value; OnPropertyChanged("XAxesRespiratory"); } }
        public AxesCollection XAxesHeart { get { return xAxesHeart; } private set { xAxesHeart = value; OnPropertyChanged("XAxesHeart"); } }
        public string Email { get; set; } = "daria_kkge@gmail.com";

        public Pet _pet { get; set; } = new Pet();
        public Diagnostic(Pet pet)
        {            
            InitializeComponent();

            _pet = pet;
            /*Data = new DiagnosticData((float)36.6, 80, 60,
                new List<MeasureModel>() { new MeasureModel(DateTime.Now - TimeSpan.FromHours(2), 35), new MeasureModel(DateTime.Now - TimeSpan.FromHours(1), 36), new MeasureModel(DateTime.Now - TimeSpan.FromHours(0), 38), },
                new List<MeasureModel>() { new MeasureModel(DateTime.Now - TimeSpan.FromHours(2), 20), new MeasureModel(DateTime.Now - TimeSpan.FromHours(1), 30), new MeasureModel(DateTime.Now - TimeSpan.FromHours(0), 25), },
                new List<MeasureModel>() { new MeasureModel(DateTime.Now - TimeSpan.FromHours(2), 20), new MeasureModel(DateTime.Now - TimeSpan.FromHours(1), 30), new MeasureModel(DateTime.Now - TimeSpan.FromHours(0), 25), });*/
            // потом сюда поставите данные с сервера, передавайте дату через параметры конструктора сюда

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            TemperatureValues = new ChartValues<MeasureModel>(Data.TemperatureList);
            RespiratoryValues = new ChartValues<MeasureModel>(Data.RespiratoryRateList);
            HeartValues = new ChartValues<MeasureModel>(Data.HeartRateList);

            XAxesTemperature[0].RangeChanged += Axis_RangeChanged; // Привязка метода для обработки зума
            XAxesRespiratory[0].RangeChanged += Axis_RangeChanged;
            XAxesHeart[0].RangeChanged += Axis_RangeChanged;

            DataContext = this;            
        }

        private void gif_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = new TimeSpan(0, 0, 1);
            ((MediaElement)sender).Play();
        }         

        private void Button_Click_Refresh(object sender, RoutedEventArgs e, DiagnosticData _data)
        {
            var now = DateTime.Now;

            if ((Data.TemperatureList.Count > 0 ? Data.TemperatureList.Last().DateTime : null) != _data.TemperatureList.Last().DateTime) // Если данные новые
            {
                TemperatureValues.Add(_data.TemperatureList.Last());
                OnPropertyChanged("TemperatureValues");
            }
            if ((Data.RespiratoryRateList.Count > 0 ? Data.RespiratoryRateList.Last().DateTime : null) != _data.RespiratoryRateList.Last().DateTime)
            {
                RespiratoryValues.Add(_data.RespiratoryRateList.Last());
                OnPropertyChanged("RespiratoryValues");
            }
            if ((Data.HeartRateList.Count > 0 ? Data.HeartRateList.Last().DateTime : null) != _data.HeartRateList.Last().DateTime)
            {
                HeartValues.Add(_data.HeartRateList.Last());
                OnPropertyChanged("HeartValues");
            }
            
            Data = _data.Clone() as DiagnosticData; // Здесь надо передать по значению а не по ссылке

            AxisMinX = now.Date.Ticks + TimeSpan.FromHours(now.Hour).Ticks - TimeSpan.FromHours(minModifier).Ticks;
            XAxesTemperature[0].MinValue = AxisMinX;
            XAxesTemperature[1].MinValue = AxisMinX; // Для зума сепараторов, чтобы апдейтить
            XAxesRespiratory[0].MinValue = AxisMinX;
            XAxesRespiratory[1].MinValue = AxisMinX;
            XAxesHeart[0].MinValue = AxisMinX;
            XAxesHeart[1].MinValue = AxisMinX;

            AxisMaxX = now.Date.Ticks + TimeSpan.FromHours(now.Hour).Ticks + TimeSpan.FromHours(1).Ticks;
            XAxesTemperature[0].MaxValue = AxisMaxX;
            XAxesTemperature[1].MaxValue = AxisMaxX;
            XAxesRespiratory[0].MaxValue = AxisMaxX;
            XAxesRespiratory[1].MaxValue = AxisMaxX;
            XAxesHeart[0].MaxValue = AxisMaxX;
            XAxesHeart[1].MaxValue = AxisMaxX;



            // Здесь не идет контроля количества значений в графике, ожидается что поступающие данные уже прошли контроль
        }
            
        public async Task RefreshTest() // Заглушка, я не знаю как будут получаться данные, передавайте в метод выше. Аналог данного метода только реальный должен быть реализован где то снаружи, не на этой странице
        {
            List<MeasureModel> testTemperature = new List<MeasureModel>();
            List<MeasureModel> testRespiratory = new List<MeasureModel>();
            List<MeasureModel> testHeart = new List<MeasureModel>();

            DateTime now = DateTime.Now;
            int ms = 1000; // Настройка скорости симуляции, чем больше тем медленнее
            
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (testTemperature.Count >= 1440) // убирать последние когда достигли одного дня
                {
                    testTemperature.RemoveAt(0);
                    testTemperature.RemoveAt(0);
                    testTemperature.RemoveAt(0);
                    testTemperature.RemoveAt(0);
                    testTemperature.RemoveAt(0);

                    testRespiratory.RemoveAt(0);
                    testRespiratory.RemoveAt(0);
                    testRespiratory.RemoveAt(0);
                    testRespiratory.RemoveAt(0);
                    testRespiratory.RemoveAt(0);

                    testHeart.RemoveAt(0);
                    testHeart.RemoveAt(0);
                    testHeart.RemoveAt(0);
                    testHeart.RemoveAt(0);
                    testHeart.RemoveAt(0);
                }
                if ((i * 5 + 5) / (int)60 != 0)
                {
                    Console.WriteLine("");
                }

                testTemperature.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5), 30)); // где то там у них в листе добавляются новые данные в список данных
                testRespiratory.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5), 36));
                testHeart.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5), 105));

                Button_Click_Refresh(this, new RoutedEventArgs(), new DiagnosticData((float)35, 31, 103, testTemperature, testRespiratory, testHeart)); // Отправляются полные списки


                AxisMinX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5) / (int)60).Ticks - TimeSpan.FromHours(minModifier).Ticks; // Симуляция изменения времени для ускоренной версии графика, перекрывает
                XAxesTemperature[0].MinValue = AxisMinX;
                XAxesTemperature[1].MinValue = AxisMinX;
                XAxesRespiratory[0].MinValue = AxisMinX;
                XAxesRespiratory[1].MinValue = AxisMinX;
                XAxesHeart[0].MinValue = AxisMinX;
                XAxesHeart[1].MinValue = AxisMinX;

                AxisMaxX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5) / (int)60).Ticks + TimeSpan.FromHours(1).Ticks; // базовую реализацию
                XAxesTemperature[0].MaxValue = AxisMaxX;
                XAxesTemperature[1].MaxValue = AxisMaxX;
                XAxesRespiratory[0].MaxValue = AxisMaxX;
                XAxesRespiratory[1].MaxValue = AxisMaxX;
                XAxesHeart[0].MaxValue = AxisMaxX;
                XAxesHeart[1].MaxValue = AxisMaxX;

                await Task.Delay(ms);

                testTemperature.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 1), 32));
                testRespiratory.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 1), 35));
                testHeart.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 1), 117));

                Button_Click_Refresh(this, new RoutedEventArgs(), new DiagnosticData((float)37, 38, 104, testTemperature, testRespiratory, testHeart));

                AxisMinX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5 + 1) / (int)60).Ticks - TimeSpan.FromHours(minModifier).Ticks;
                XAxesTemperature[0].MinValue = AxisMinX;
                XAxesTemperature[1].MinValue = AxisMinX;
                XAxesRespiratory[0].MinValue = AxisMinX;
                XAxesRespiratory[1].MinValue = AxisMinX;
                XAxesHeart[0].MinValue = AxisMinX;
                XAxesHeart[1].MinValue = AxisMinX;

                AxisMaxX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5 + 1) / (int)60).Ticks + TimeSpan.FromHours(1).Ticks;
                XAxesTemperature[0].MaxValue = AxisMaxX;
                XAxesTemperature[1].MaxValue = AxisMaxX;
                XAxesRespiratory[0].MaxValue = AxisMaxX;
                XAxesRespiratory[1].MaxValue = AxisMaxX;
                XAxesHeart[0].MaxValue = AxisMaxX;
                XAxesHeart[1].MaxValue = AxisMaxX;

                await Task.Delay(ms);

                testTemperature.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 2), 33));
                testRespiratory.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 2), 35));
                testHeart.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5+ 2), 140));

                Button_Click_Refresh(this, new RoutedEventArgs(), new DiagnosticData((float)39, 35, 105, testTemperature, testRespiratory, testHeart));

                AxisMinX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5 + 2) / (int)60).Ticks - TimeSpan.FromHours(minModifier).Ticks;
                XAxesTemperature[0].MinValue = AxisMinX;
                XAxesTemperature[1].MinValue = AxisMinX;
                XAxesRespiratory[0].MinValue = AxisMinX;
                XAxesRespiratory[1].MinValue = AxisMinX;
                XAxesHeart[0].MinValue = AxisMinX;
                XAxesHeart[1].MinValue = AxisMinX;

                AxisMaxX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5 + 2) / (int)60).Ticks + TimeSpan.FromHours(1).Ticks;
                XAxesTemperature[0].MaxValue = AxisMaxX;
                XAxesTemperature[1].MaxValue = AxisMaxX;
                XAxesRespiratory[0].MaxValue = AxisMaxX;
                XAxesRespiratory[1].MaxValue = AxisMaxX;
                XAxesHeart[0].MaxValue = AxisMaxX;
                XAxesHeart[1].MaxValue = AxisMaxX;

                await Task.Delay(ms);

                testTemperature.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 3), 38));
                testRespiratory.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 3), 35));
                testHeart.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 3), 93));

                Button_Click_Refresh(this, new RoutedEventArgs(), new DiagnosticData((float)31, 35, 106, testTemperature, testRespiratory, testHeart));

                AxisMinX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5 + 3) / (int)60).Ticks - TimeSpan.FromHours(minModifier).Ticks;
                XAxesTemperature[0].MinValue = AxisMinX;
                XAxesTemperature[1].MinValue = AxisMinX;
                XAxesRespiratory[0].MinValue = AxisMinX;
                XAxesRespiratory[1].MinValue = AxisMinX;
                XAxesHeart[0].MinValue = AxisMinX;
                XAxesHeart[1].MinValue = AxisMinX;

                AxisMaxX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5 + 3) / (int)60).Ticks + TimeSpan.FromHours(1).Ticks;
                XAxesTemperature[0].MaxValue = AxisMaxX;
                XAxesTemperature[1].MaxValue = AxisMaxX;
                XAxesRespiratory[0].MaxValue = AxisMaxX;
                XAxesRespiratory[1].MaxValue = AxisMaxX;
                XAxesHeart[0].MaxValue = AxisMaxX;
                XAxesHeart[1].MaxValue = AxisMaxX;

                await Task.Delay(ms);

                testTemperature.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 4), 36));
                testRespiratory.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 4), 40));
                testHeart.Add(new MeasureModel(now.Date + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(i * 5 + 4), 115));

                Button_Click_Refresh(this, new RoutedEventArgs(), new DiagnosticData((float)30, 36, 107, testTemperature, testRespiratory, testHeart));

                AxisMinX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5 + 4) / (int)60).Ticks - TimeSpan.FromHours(minModifier).Ticks;
                XAxesTemperature[0].MinValue = AxisMinX;
                XAxesTemperature[1].MinValue = AxisMinX;
                XAxesRespiratory[0].MinValue = AxisMinX;
                XAxesRespiratory[1].MinValue = AxisMinX;
                XAxesHeart[0].MinValue = AxisMinX;
                XAxesHeart[1].MinValue = AxisMinX;

                AxisMaxX = now.Date.Ticks + TimeSpan.FromHours(12).Ticks + TimeSpan.FromHours((i * 5 + 4) / (int)60).Ticks + TimeSpan.FromHours(1).Ticks;
                XAxesTemperature[0].MaxValue = AxisMaxX;
                XAxesTemperature[1].MaxValue = AxisMaxX;
                XAxesRespiratory[0].MaxValue = AxisMaxX;
                XAxesRespiratory[1].MaxValue = AxisMaxX;
                XAxesHeart[0].MaxValue = AxisMaxX;
                XAxesHeart[1].MaxValue = AxisMaxX;

                await Task.Delay(ms);
            }                           
        }

        private void Axis_RangeChanged(LiveCharts.Events.RangeChangedEventArgs eventArgs)
        {
            if (!minModifierToggled)
            {
                minModifierToggled = true;
                minModifier = 3; // Уменьшаем с 24 часов до 4
                XAxesTemperature[0].Separator.Step = TimeSpan.FromHours(1).Ticks;
                XAxesRespiratory[0].Separator.Step = TimeSpan.FromHours(1).Ticks;
                XAxesHeart[0].Separator.Step = TimeSpan.FromHours(1).Ticks;
            }
            else
            {
                minModifierToggled = false;
                minModifier = 23;
                XAxesTemperature[0].Separator.Step = TimeSpan.FromHours(6).Ticks;
                XAxesRespiratory[0].Separator.Step = TimeSpan.FromHours(6).Ticks;
                XAxesHeart[0].Separator.Step = TimeSpan.FromHours(6).Ticks;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HealthState npw = new HealthState(_pet);
            npw.ShowDialog();
            //Заглушка для редактирования
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));                
            }
        }
    }
}
