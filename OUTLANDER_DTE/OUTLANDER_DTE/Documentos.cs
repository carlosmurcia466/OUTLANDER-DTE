using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{
    public class Documento
    {
        public string codigoGeneracion { get; set; }
        public object codigoGeneracionR { get; set; }
        public string correo { get; set; }
        public string fecEmi { get; set; }
        public double montoIva { get; set; }
        public string nombre { get; set; }
        public string numDocumento { get; set; }
        public string numeroControl { get; set; }
        public string selloRecibido { get; set; }
        public string telefono { get; set; }
        public string tipoDocumento { get; set; }
        public string tipoDte { get; set; }
        public Documento()
        {

        }
    }
}
