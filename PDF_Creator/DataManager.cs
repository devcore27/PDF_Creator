using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Creator
{
    class DataManager
    {
        private static string KLASSE_SETTINGS_KEY = "klasse_saved";

        public delegate void OnKlasseChangedListener(Klasse klasse);

        private static DataManager instance;
        private Klasse klasse = null;

        public event OnKlasseChangedListener Changed;

        private DataManager() { }

        protected virtual void OnChanged(Klasse klasse)
        {
            Changed?.Invoke(klasse);
        }

        public Klasse Klasse
        {
            get
            {
                return klasse;
            }

            set
            {
                klasse = value;
                OnChanged(klasse);
            }
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

        public bool isEmpty() { return klasse == null;  }

        public void saveSettings()
        {
            String klasseAsString = null;
            if (!isEmpty())
            {
                klasseAsString = klasse.Name + '\t' + klasse.Leiter;
                foreach (string student in klasse.Students)
                    klasseAsString += '\t' + student;
            }

            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[KLASSE_SETTINGS_KEY] = klasseAsString;
        }

        public void loadSettings()
        {
            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;
            String klasseAsString = (String) localSettings.Values[KLASSE_SETTINGS_KEY];

            if (klasseAsString != null)
            {
                string[] data = klasseAsString.Split('\t');
                klasse = new Klasse(data[0], data[1]);

                for (int i = 2; i < data.Length; ++i)
                {
                    klasse.addStudent(data[i]);
                }
            }
        }
    }
}
