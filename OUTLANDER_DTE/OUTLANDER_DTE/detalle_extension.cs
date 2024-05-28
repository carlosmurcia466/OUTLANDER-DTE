using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{
    class detalle_extension
    {
        public List<Extension> extension { get; set; }
    }
    public class Extension
    {
        public string nombEntrega { get; set; }
        public string docuEntrega { get; set; }
        public string nombRecibe { get; set; }
        public string docuRecibe { get; set; }
        public string observaciones { get; set; }
        public string placaVehiculo { get; set; }
    }
}
