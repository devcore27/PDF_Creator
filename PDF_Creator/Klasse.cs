using System.Collections.Generic;
using System.Linq;

namespace PDF_Creator
{
    public class Klasse
    {
        public struct StudentName
        {
            public string firstname;
            public string lastname;
        }

        private string name;
        private string leiter;
        private List<StudentName> students = new List<StudentName>();
        
        public Klasse (string name, string leiter)
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

        public List<StudentName> Students
        {
            get
            {
                return students;
            }
        }

        public string StudentAt(int index)
        {
            return students[index].lastname + ", " + students[index].firstname;
        }

        public int StudentsCount()
        {
            return students.Count();
        }

        public void AddStudent(string firstname, string lastname)
        {
            StudentName studentName = new StudentName()
            {
                firstname = firstname,
                lastname = lastname
            };
            students.Add(studentName);
        }

    }
}
