using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Test.Models
{
    static public class SerializeFrom
    {
        private static List<WalkerModel> serializeWalkers(string[] files)
        {
            List<SerializeClass> inits = new List<SerializeClass>();
            foreach (var file in files)
            {

                using (StreamReader r = new StreamReader(file))
                {
                    List<SerializeClass> items;
                    string json = r.ReadToEnd();
                    try
                    {
                        items = JsonConvert.DeserializeObject<List<SerializeClass>>(json);
                        inits.AddRange(items);
                    }
                    catch (Exception) { MessageBox.Show($"Некорректные данные в файле {file}, данные с этого файла не будут экспортированы."); }

                }
            }
            List<WalkerModel> Walkers = new List<WalkerModel>();
            foreach (var item in inits)
            {
                if (Walkers.FindIndex(p => p.User == item.User) == -1)
                    Walkers.Add(new WalkerModel(item.User, new Day(item.Rank, item.Status, item.Steps)));
                else
                    Walkers[Walkers.FindIndex(p => p.User == item.User)].addDay(new Day(item.Rank, item.Status, item.Steps));
            }
            Walkers.ForEach(p => p.Calculate());
            return Walkers;
        }

        public static List<WalkerModel> serializeWalkersFromJSON()
        {
            string[] files = Directory.GetFiles(@"Days\", "*.json");
            return serializeWalkers(files);
        }
        public static List<WalkerModel> serializeWalkersFromJSON_ChooseFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON (*.json)|*.json";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
                return serializeWalkers(openFileDialog.FileNames);
            return null;
        }
    }
}
