using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUTLANDER_DTE
{

    class detalle_relacionado
    {
        public List<DocumentoRelacionado> documentoRelacionado { get; set; }
        Nullable<System.DateTime> _Date;
    }
    public class DocumentoRelacionado
    {
        public string tipoDocumento { get; set; }
        public int tipoGeneracion { get; set; }
        public string numeroDocumento { get; set; }
        public string fechaEmision { get; set; }
    }
}
