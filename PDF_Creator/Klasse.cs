using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Creator
{
    class Klasse
    {
        private string name;
        private string leiter;
        private List<string> students = new List<string>();

        public Klasse(string name, string leiter)
        {
            this.name = name;
            this.leiter = leiter;
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string Leiter
        {
            get
            {
                return leiter;
            }
            set
            {
                leiter = value;
            }
        }

        public List<string> Students {
            get
            {
                return students;
            }
        }

        public void addStudent(string name)
        {
            students.Add(name);
        }

    }
}
