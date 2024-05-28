using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{
    public class Resumen
    {
        public double totalCompra { get; set; }
        public double descu { get; set; }
        public double totalNoSuj { get; set; }
        public double totalExenta { get; set; }
        public double totalGravada { get; set; }
        public double subTotalVentas { get; set; }
        public double descuNoSuj { get; set; }
        public double descuExenta { get; set; }
        public double descuGravada { get; set; }
        public double porcentajeDescuento { get; set; }
        public double totalDescu { get; set; }
        public List<Tributo> tributos { get; set; }
        public double subTotal { get; set; }
        public double ivaPerci1 { get; set; }
        public double ivaRete1 { get; set; }
        public double reteRenta { get; set; }
        public double montoTotalOperacion { get; set; }
        public double totalNoGravado { get; set; }
        public double totalPagar { get; set; }
        public string totalLetras { get; set; }
        public double totalIva { get; set; }
        public double saldoFavor { get; set; }
        public int condicionOperacion { get; set; }
        public object pagos { get; set; }
        public object numPagoElectronico { get; set; }

        public string codIncoterms { get; set; }
        public string descIncoterms { get; set; }
        public string observaciones { get; set; }
        public double flete { get; set; }
        public double seguro { get; set; }
        
             
    }
}

public class Tributo
{
    public string codigo { get; set; }
    public string descripcion { get; set; }
    public double valor { get; set; }
}
