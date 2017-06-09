using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Creator
{
    class DataManager
    {
        public delegate void OnKlasseChangedListener(Klasse klasse);

        private static DataManager instance;
        private Klasse klasse = null;
        private event OnKlasseChangedListener Changed;

        private DataManager() { }

        protected virtual void OnChanged(Klasse klasse)
        {
            if (Changed != null)
                Changed(klasse);
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
    }
}
