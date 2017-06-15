using System;
using System.Collections.Generic;
using System.Linq;

namespace PDF_Creator
{
    class DataManager
    {
        private static string KLASSEN_SETTINGS_KEY = "klassen_saved";

        public delegate void OnKlassenChangedListener(Klasse klasse);

        private static DataManager instance;
        private List<Klasse> klassen = new List<Klasse>();

        public event OnKlassenChangedListener Changed;

        private DataManager() { }

        protected virtual void OnChanged(Klasse klasse)
        {
            Changed?.Invoke(klasse);
        }

        public Klasse KlasseAt(int index)
        {
            if (index < 0 || index >= klassen.Count())
                return null;
            return klassen.ElementAt(index);
        }

        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataManager();
                }
                return instance;
            }
        }

        public bool IsEmpty() { return klassen.Count == 0;  }

        public void Clear()
        {
            klassen.Clear();
            Changed(null);
        }

        public int Count()
        {
            return klassen.Count();
        }

        public void AddKlasse(Klasse klasse)
        {
            klassen.Add(klasse);
            OnChanged(klasse);
        }

        public void SaveSettings()
        {
            string klassenAsString = "";
            if (!IsEmpty())
            {
                foreach (Klasse klasse in klassen)
                {
                    klassenAsString += klasse.Name + '\t' + klasse.Leiter;
                    foreach (Klasse.StudentName student in klasse.Students)
                    {
                        klassenAsString += '\t' + student.lastname + ',' + student.firstname;
                    }
                    klassenAsString += '\n';
                }
            }

            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[KLASSEN_SETTINGS_KEY] = klassenAsString;
        }

        public void LoadSettings()
        {
            if (!DataManager.Instance.IsEmpty())
                return;

            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            if (!localSettings.Values.ContainsKey(KLASSEN_SETTINGS_KEY))
                return;

            String klassenAsString = (String)localSettings.Values[KLASSEN_SETTINGS_KEY];
            
            if (klassenAsString != null && klassenAsString.Length > 0)
            {
                string[] klassen = klassenAsString.Split('\n');
                foreach (string klasseString in klassen)
                {
                    if (klasseString.Length == 0) continue;
                    string[] data = klasseString.Split('\t');
                    Klasse klasse = new Klasse(data[0], data[1]);
                    for (int i = 2; i < data.Length; ++i)
                    {
                        string[] names = data[i].Split(',');
                        klasse.AddStudent(names[1], names[0]);
                    }
                    AddKlasse(klasse);
                }
            }
        }
    }
}
