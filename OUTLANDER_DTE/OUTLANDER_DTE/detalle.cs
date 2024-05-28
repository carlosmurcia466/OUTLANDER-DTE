using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{
    class detalle
    {
        public List<CuerpoDocumento> cuerpoDocumento { get; set; }


    }
    public class CuerpoDocumento
    {
        public int numItem { get; set; }
        public int tipoItem { get; set; }
        public object numeroDocumento { get; set; }
        public string codigo { get; set; }
        public object codTributo { get; set; }
        public string descripcion { get; set; }
        public double cantidad { get; set; }
        public int uniMedida { get; set; }
        public double precioUni { get; set; }
        public double montoDescu { get; set; }
        public double ventaNoSuj { get; set; }
        public double ventaExenta { get; set; }
        public double ventaGravada { get; set; }
        public List<string> tributos { get; set; }
        public double psv { get; set; }
        public double noGravado { get; set; }
        public double ivaItem { get; set; }
        public double compra { get; set; }
    }
}
