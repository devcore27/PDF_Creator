using System.Collections.Generic;
using System.Linq;

namespace PDF_Creator
{
    class DataManager
    {
        //private static string KLASSE_SETTINGS_KEY = "klasse_saved";

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

        /*public void saveSettings()
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
            String klasseAsString = (String)localSettings.Values[KLASSE_SETTINGS_KEY];

            if (klasseAsString != null)
            {
                Klasse = new Klasse(klasseAsString.Split('\t'));
            }
        }*/
    }
}
