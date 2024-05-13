using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VDC_WPF_T.Model
{
    public class DiagnosticData : INotifyPropertyChanged, ICloneable
    {
        public DiagnosticData(float vt, int vrr, int vhr, List<MeasureModel> tl, List<MeasureModel> rrl, List<MeasureModel> hrl)
        {
            visitTemperature = vt;
            visitRespiratoryRate = vrr;
            visitHeartRate = vhr;
            temperatureList = tl;
            respiratoryRateList = rrl;
            heartRateList = hrl;
        }

        public object Clone()
        {
            return new DiagnosticData(VisitTemperature, VisitRespiratoryRate, VisitHeartRate, new List<MeasureModel>(TemperatureList), new List<MeasureModel>(RespiratoryRateList), new List<MeasureModel>(HeartRateList));
        }

        float visitTemperature;
        int visitRespiratoryRate;
        int visitHeartRate;
        List<MeasureModel> temperatureList;
        List<MeasureModel> respiratoryRateList;
        List<MeasureModel> heartRateList;

        public float VisitTemperature { get { return visitTemperature; } set { visitTemperature = value; OnPropertyChanged("VisitTemperature"); }  }
        public int VisitRespiratoryRate { get { return visitRespiratoryRate; } set { visitRespiratoryRate = value; OnPropertyChanged("VisitRespiratoryRate"); } }
        public int VisitHeartRate { get { return visitHeartRate; } set { visitHeartRate = value; OnPropertyChanged("VisitHeartRate"); } }
        public List<MeasureModel> TemperatureList { get { return temperatureList; } set { temperatureList = value; OnPropertyChanged("TemperatureList"); } }
        public List<MeasureModel> RespiratoryRateList { get { return respiratoryRateList; } set { respiratoryRateList = value; OnPropertyChanged("RespiratoryRateList"); } }
        public List<MeasureModel> HeartRateList { get { return heartRateList; } set { heartRateList = value; OnPropertyChanged("HeartRateList"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }

        public MeasureModel(DateTime dateTime, double value)
        {
            DateTime = dateTime;
            Value = value;
        }
    }
}
