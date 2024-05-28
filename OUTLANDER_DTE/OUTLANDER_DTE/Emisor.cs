using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{
    class Emisor
    {
        public string nit { get; set; }
        public string nrc { get; set; }
        public string nombre { get; set; }
        public string codActividad { get; set; }
        public string descActividad { get; set; }
        public string nombreComercial { get; set; }
        public string tipoEstablecimiento { get; set; }
        public List<direccion> direccion { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string codEstableMH { get; set; }
        public string codEstable { get; set; }
        public string codPuntoVentaMH { get; set; }
        public string codPuntoVenta { get; set; }
        public int tipoItemExpor { get; set; }
        public string recintoFiscal { get; set; }
        public string regimen { get; set; }
        public string nomEstablecimiento { get; set; }
        public Emisor()
        {

        }

    }
}
public class direccion
{
    public string departamento { get; set; }
    public string municipio { get; set; }
    public string complemento { get; set; }
}

