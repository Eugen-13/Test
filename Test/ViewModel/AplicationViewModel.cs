using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts;
using Microsoft.Win32;
using Test.Commands;
using System.Xml.Serialization;
using LINQtoCSV;
using Test.Models;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace Test
{
    internal class AplicationViewModel : INotifyPropertyChanged
    {

        private SeriesCollection _senderChart;
        public SeriesCollection SenderChart {
            get { return _senderChart; }
            set
            { 
                _senderChart = value;
                OnPropertyChanged("SenderChart");
            }
        }
        public List<string> ItemsFile { get; set; }

        private string _selectedFileItem;
        public string SelectedFileItem
        {
            get { return _selectedFileItem; }
            set
            {
                _selectedFileItem = value;
                OnPropertyChanged("SelectedFileItem");
            }
        }
        
        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ??
                  (_exportCommand = new RelayCommand(obj =>
                  {
                      WriteInFile();
                  }));
            }
        }

        private RelayCommand _exportCommand2;
        public RelayCommand ExportCommand2
        {
            get
            {
                return _exportCommand2 ??
                  (_exportCommand2 = new RelayCommand(obj =>
                  {
                      List<WalkerModel> tempWalkers = SerializeFrom.serializeWalkersFromJSON_ChooseFiles();
                      if (tempWalkers != null)
                      {
                          foreach (var item in tempWalkers)
                          {
                              if(Walkers.FindIndex(p=>p.User == item.User) != -1)
                              {
                                  Walkers[Walkers.FindIndex(p => p.User == item.User)].Days.AddRange(item.Days);
                              }                                  
                              else
                                  Walkers.Add(item);
                          }
                          InitGraph();
                      }
                  }));
            }
        }
        private ChartValues<double> _stepCounts;
        public ChartValues<double> StepCounts
        {
            get { return _stepCounts; }
            set
            {
                _stepCounts = value;
                OnPropertyChanged("StepCounts");
            }
        }

        private WalkerModel _selectedWalker;
        public WalkerModel SelectedWalker
        {
            get { return _selectedWalker; }
            set
            {
                _selectedWalker = value;
                OnPropertyChanged("SelectedWalker");
            }
        }
      
        public event PropertyChangedEventHandler PropertyChanged;
        public List<WalkerModel> Walkers { get; set; }
        public object CsvSerializer { get; private set; }

        public AplicationViewModel()
        {
            ItemsFile = new List<string>() { "XML", "JSON", "CSV" };
            this.Walkers = SerializeFrom.serializeWalkersFromJSON();
            _stepCounts = new ChartValues<double>();
            SelectedWalker = Walkers.First();
            SelectedFileItem = ItemsFile.First();
            InitGraph();
        }
        private void InitGraph()
        {
            _stepCounts.Clear();
            List<int> list = new List<int>();    
            for (int i = 0; i < Walkers[Walkers.FindIndex(p=> p.User == SelectedWalker.User)].Days.Count; i++)
            {
                list.Add((i+1));
                _stepCounts.Add(Walkers[Walkers.FindIndex(p => p.User == SelectedWalker.User)].Days[i].Steps);
            }
            var doubleMapperWithMonthColors = new LiveCharts.Configurations.CartesianMapper<double>()
            .X((value, index) => index + 1)
            .Y((value) => value)
           .Fill((value, index) => {
               if (SelectedWalker.HighestResult == value)
                   return new LinearGradientBrush(Color.FromArgb(255, 127, 255, 0), Colors.Red, 100);
               else if (SelectedWalker.LowestResult == value)
                   return new LinearGradientBrush(Color.FromArgb(255, 127, 255, 0), Color.FromArgb(255, 115, 13, 115),45);
               else
                   return new LinearGradientBrush(Color.FromArgb(255, 127, 255, 0), Colors.Blue, 100);
            });

            Charting.For<double>(doubleMapperWithMonthColors, SeriesOrientation.Horizontal);
            _senderChart = new SeriesCollection();
            var columnSeries = new ColumnSeries() { Values = new ChartValues<double>(), Title = "Steps-Days" };
            foreach (var val in _stepCounts)
            {
                columnSeries.Values.Add(val);
            }
            this._senderChart.Add(columnSeries);
            OnPropertyChanged(nameof(SenderChart));
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
                if (prop == "SelectedWalker")
                    InitGraph();
            }

        }
        private void WriteInFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if(SelectedFileItem == "XML")
                saveFileDialog1.Filter = "XML (*.xml)|*.xml";
            else if(SelectedFileItem == "JSON")
                saveFileDialog1.Filter = "JSON (*.json)|*.json";
            else
                saveFileDialog1.Filter = "CSV (*.csv)|*.csv";

            if (saveFileDialog1.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile()))
                {
                    switch (SelectedFileItem)
                    {
                        case "XML":
                            (new XmlSerializer(typeof(WalkerModel))).Serialize(sw, SelectedWalker);
                            break;
                        case "JSON":
                            sw.Write(JsonConvert.SerializeObject(SelectedWalker));
                            break;
                        case "CSV":
                            var csvFileDescription = new CsvFileDescription
                            {
                                FirstLineHasColumnNames = true,
                                SeparatorChar = ','
                            };
                            var csvContext = new CsvContext();
                            csvContext.Write<Day>(SelectedWalker.Days, sw, csvFileDescription);
                            break;
                    }
                }
            }
        }
    }
}
