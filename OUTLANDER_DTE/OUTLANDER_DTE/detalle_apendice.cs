using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{
    class detalle_apendice
    {
        public List<Apendice> apendice { get; set; }
    }
    public class Apendice
    {
        public string campo { get; set; }
        public string etiqueta { get; set; }
        public string valor { get; set; }
    }
}
