using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{
    public class Receptor
    {

        public string tipoDocumento { get; set; }
        public string numDocumento { get; set; }
        public string nit { get; set; }
        public string nrc { get; set; }
        public string nombre { get; set; }
        public string codActividad { get; set; }
        public string descActividad { get; set; }
        public string bienTitulo { get; set; }
        public object nombreComercial { get; set; }
        public List<direccion> direccion { get; set; }
        public object telefono { get; set; }
        public string correo { get; set; }
        public string codPais { get; set; }
        public string nombrePais { get; set; }
        public string complemento { get; set; }
        public string tipoPersona { get; set; }



        public Receptor()
        {

        }
    }
}
