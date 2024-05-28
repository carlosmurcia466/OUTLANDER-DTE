using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{
    public class Conexion
    {
    }

    public class Conexiones
    {
        public string servidor { get; set; }
        public string basededatos { get; set; }
        public string usuario { get; set; }
        public string pwd { get; set; }
    }

    public class principal
    {
        public List<Conexiones> conexiones { get; set; }
    }

}
