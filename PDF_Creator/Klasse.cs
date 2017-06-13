using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Creator
{
    public class Klasse
    {
        private string name;
        private string leiter;
        private List<string> students = new List<string>();

        public Klasse(string name, string leiter)
        {
            this.name = name;
            this.leiter = leiter;
        }

        public Klasse (params string[] list)
        {
            name = list[0];
            leiter = list[1];
            for (int i = 2; i != list.Length; ++i)
            {
                addStudent(list[i]);
            }
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
