using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreDb.Domain.Model
{
    public class Student : Entity
    {
        public int Age { get; set; }
        public int Roll { get; set; }
        public string Name { get; set; }
        public int Class { get; set; }
        public string Section { get; set; }
    }
}
