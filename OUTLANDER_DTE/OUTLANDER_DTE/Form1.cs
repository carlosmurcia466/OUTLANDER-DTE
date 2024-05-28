using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OUTLANDER_DTE
{
    public partial class Form1 : Form
    {

        DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["data"].ToString());
        string db1, user1, pwd1, server1;
        string db2, user2, pwd2, server2;
        String tabla, tipoDoc, docEntry, Serie, docNum, numero, impreso, seimprime, fecha;
        string codigoGeneracion;

        List<Identificacion> identificacion = new List<Identificacion>();
        List<detalle_relacionado> dt_re = new List<detalle_relacionado>();
        List<Emisor> emisor = new List<Emisor>();
        List<Receptor> receptor = new List<Receptor>();
        List<detalle> detalles = new List<detalle>();
        List<detalle_extension> detalle_extension = new List<detalle_extension>();
        List<Resumen> resumen = new List<Resumen>();
        List<detalle_apendice> detalle_apendice = new List<detalle_apendice>();
        List<Documento> documento = new List<Documento>();
        List<Motivo> motivo = new List<Motivo>();



        List<string[]> detalle_factura = new List<string[]>();
        public Form1()
        {
            InitializeComponent();
        }

        private void tiempo_Tick(object sender, EventArgs e)
        {

           
            tiempo.Enabled = false;

            foreach (var i in di.GetFiles())
            {
                Encoding encoding = Encoding.UTF7;
                StreamReader r = new StreamReader(i.FullName, encoding);
                string jsonString = r.ReadToEnd();

                var m = JsonConvert.DeserializeObject<List<principal>>(jsonString);

                foreach (var cx in m)
                {

                    server1 = cx.conexiones[0].servidor.ToString();
                    user1 = cx.conexiones[0].usuario.ToString();
                    pwd1 = cx.conexiones[0].pwd.ToString();
                    db1 = cx.conexiones[0].basededatos.ToString();

                    server2 = cx.conexiones[1].servidor.ToString();
                    user2 = cx.conexiones[1].usuario.ToString();
                    pwd2 = cx.conexiones[1].pwd.ToString();
                    db2 = cx.conexiones[1].basededatos.ToString();

                }

                Data();
            }


            tiempo.Enabled = true;
        }

        public void Data()
        {
            //-------------------------INICIO CREACION JSON----------------------------//
            //APARTADO PARA IDENTIFICACION

            try
            {
                SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
                SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);
           
            //TABLA RICOH PRINT

            cxdb1.Open();
            string queryDocumentos = "Select*From RICOH_PRINT where impreso='0' and seimprime='1' and tipoDoc in('FA','CF','NCL','NR','SJ','EX','NCI','NB') order by id desc";
            SqlCommand cmdDocumentos = new SqlCommand(queryDocumentos, cxdb1);

            using (SqlDataReader reader = cmdDocumentos.ExecuteReader())
            {
                if (reader.Read())
                {

                    tabla = Convert.ToString(reader["tabla"]);
                    tipoDoc = Convert.ToString(reader["tipoDoc"]);
                    docEntry = Convert.ToString(reader["docEntry"]);
                    Serie = Convert.ToString(reader["Serie"]);
                    docNum = Convert.ToString(reader["docNum"]);
                    impreso = Convert.ToString(reader["impreso"]);
                    seimprime = Convert.ToString(reader["seimprime"]);
                    fecha = Convert.ToString(reader["fecha"]);



                }
            }

            object result = cmdDocumentos.ExecuteScalar();
            cxdb1.Close();


                txtmensaje.Text = docEntry+" "+ tipoDoc;


                if (result != null)
            {
               



                if (tipoDoc.ToString().Trim() == "FA")
                {
                    FA(docEntry);
                        txtmensaje.Text = docEntry;
                }
                else if (tipoDoc.ToString().Trim() == "CF")
                {
                    CF(docEntry);
                }
                else if (tipoDoc.ToString().Trim() == "NCL")
                {
                    NCL(docEntry);
                }
                else if (tipoDoc.ToString().Trim() == "NR")
                {
                        txtmensaje.Text = docEntry + "NR"+"1";
                        NR(docEntry);
                    
                }
                else if (tipoDoc.ToString().Trim() == "SJ")
                {
                    SJ(docEntry);
                }
                else if (tipoDoc.ToString().Trim() == "EX")
                {
                    EX(docEntry);
                }
                else if (tipoDoc.ToString().Trim() == "NCI")
                {
                    NCI(docEntry);
                }
                else if (tipoDoc.ToString().Trim() == "NB")
                {
                    NB(docEntry);
                }


            }

            }
            catch(Exception ex)
            {


               
            }

        }
        public void FA(string docEntry)
        {

            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);


            Identificacion iden = new Identificacion();

            cxdb1.Open();


            string queryIden = "exec GENERAR_DTE_FE " + docEntry;
            SqlDataAdapter adapterIden = new SqlDataAdapter(queryIden, cxdb1);
            DataSet dataIden = new DataSet();
            adapterIden.Fill(dataIden, "Identificacion");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtIden = dataIden.Tables[0];

            for (int i = 0; i < dtIden.Rows.Count; i++)
            {
                iden.version = Convert.ToInt32(dtIden.Rows[i]["version"]);
                iden.ambiente = Convert.ToString(dtIden.Rows[i]["ambiente"]).Trim();
                iden.tipoDte = Convert.ToString(dtIden.Rows[i]["tipoDte"]).Trim();
                iden.numeroControl = Convert.ToString(dtIden.Rows[i]["numeroControl"]).Trim();
                iden.codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                codigoGeneracion= Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();

                iden.tipoModelo = Convert.ToInt32(dtIden.Rows[i]["tipoModelo"]);
                iden.tipoOperacion = Convert.ToInt32(dtIden.Rows[i]["tipoOperacion"]);
                iden.tipoContingencia = null;
                iden.motivoContin = null;
                iden.fecEmi = Convert.ToString(dtIden.Rows[i]["fecEmi"]).Trim();
                iden.horEmi = Convert.ToString(dtIden.Rows[i]["horEmi"]).Trim();
                iden.tipoMoneda = Convert.ToString(dtIden.Rows[i]["tipoMoneda"]).Trim();

            }
                cxdb1.Close();


            identificacion.Add(iden);

            txtmensaje.Text = "identificacion  " + docEntry;

            detalle_relacionado detalle_re = new detalle_relacionado();
            List<DocumentoRelacionado> documentorelacionado = new List<DocumentoRelacionado>();
            // documentorelacionado.Add(addDocumentosRelacionados("",2,"",""));

            if (documentorelacionado.Count <= 0)
            {
                detalle_re.documentoRelacionado = null;
            }
            else
            {
                detalle_re.documentoRelacionado = documentorelacionado;
            }


            dt_re.Add(detalle_re);





            //APARTADO PARA EMISOR
            Emisor emi = new Emisor();
            cxdb1.Open();

            string query = "exec GENERAR_DTE_FE " + docEntry;
            SqlDataAdapter adapter = new SqlDataAdapter(query, cxdb1);
            DataSet dataEmisor = new DataSet();
            adapter.Fill(dataEmisor, "Emisor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtEmisor = dataEmisor.Tables[1];


            List<direccion> direccion_emisor = new List<direccion>();

            for (int i = 0; i < dtEmisor.Rows.Count; i++)
            {

                emi.nit = Convert.ToString(dtEmisor.Rows[i]["nit"].ToString().Trim());
                emi.nrc = Convert.ToString(dtEmisor.Rows[i]["nrc"].ToString().Trim());
                emi.nombre = Convert.ToString(dtEmisor.Rows[i]["nombre"].ToString().Trim());
                emi.codActividad = Convert.ToString(dtEmisor.Rows[i]["codActividad"].ToString().Trim());
                emi.descActividad = Convert.ToString(dtEmisor.Rows[i]["descActividad"].ToString().Trim());
                emi.nombreComercial = Convert.ToString(dtEmisor.Rows[i]["nombreComercial"].ToString().Trim());
                emi.tipoEstablecimiento = Convert.ToString(dtEmisor.Rows[i]["tipoEstablecimiento"].ToString().Trim());
                direccion_emisor.Add(adddireccionemisor(Convert.ToString(dtEmisor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.municipo"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.complemento"].ToString().Trim())));
                if (Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    emi.telefono = Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString()).Trim();
                }
                else
                {
                    emi.telefono = "25252800";
                }


                if (Convert.ToString(dtEmisor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    emi.correo = Convert.ToString(dtEmisor.Rows[i]["correo"].ToString()).Trim();
                }
                else
                {
                    emi.correo = "contabilidad@motocity.com.sv";
                }
                emi.codEstableMH = Convert.ToString(dtEmisor.Rows[i]["codEstableMH"].ToString().Trim());
                emi.codEstable = Convert.ToString(dtEmisor.Rows[i]["codEstable"].ToString().Trim());
                emi.codPuntoVentaMH = Convert.ToString(dtEmisor.Rows[i]["codPuntoVentaMH"].ToString().Trim());
                emi.codPuntoVenta = Convert.ToString(dtEmisor.Rows[i]["codPuntoVenta"].ToString().Trim());

            }


            emi.direccion = direccion_emisor;

            emisor.Add(emi);

            txtmensaje.Text = "emisor " + docEntry;

            cxdb1.Close();


            //APARTADO PARA RECEPTOR

            Receptor rec = new Receptor();
            List<direccion> direccion_receptor = new List<direccion>();

            cxdb1.Open();

            string queryReceptor = "exec GENERAR_DTE_FE " + docEntry;
            SqlDataAdapter adapterReceptor = new SqlDataAdapter(queryReceptor, cxdb1);
            DataSet dataReceptor = new DataSet();
            adapterReceptor.Fill(dataReceptor, "Receptor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtReceptor = dataReceptor.Tables[2];

            for (int i = 0; i < dtReceptor.Rows.Count; i++)
            {

  
                if (Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim()) != "")
                {
                    rec.tipoDocumento = Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim());
                }
                else
                {
                    rec.tipoDocumento = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim()) != "")
                {
                    rec.numDocumento = Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim());
                }
                else
                {
                    rec.numDocumento = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim()) != "")
                {
                    rec.nit = Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim());
                }
                else
                {
                    rec.nit = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim()) != "")
                {
                    rec.nrc = Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim());
                }
                else
                {
                    rec.nrc = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim()) != "")
                {
                    rec.nombre = Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim());
                }
                else
                {
                    rec.nombre = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim()) != "")
                {
                    rec.codActividad = Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim());
                }
                else
                {
                    rec.codActividad = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim()) != "")
                {
                    rec.descActividad = Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim());
                }
                else
                {
                    rec.descActividad = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()) != "")
                {
                    direccion_receptor.Add(adddireccionreceptor(Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.municipio"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.complemento"].ToString().Trim())));
                    rec.direccion = direccion_receptor;
                }
                else
                {
                    rec.direccion = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim())!=""){
                    rec.telefono = Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim());
                }
                else
                {
                    rec.telefono = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    rec.correo = Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim());
                }
                else
                {
                    rec.correo = null;
                }

              

            }


          

            receptor.Add(rec);

            txtmensaje.Text = "receptor  " + docEntry;
            cxdb1.Close();


            //APARTADO CUERPO DE DOCUMENTO

            detalle detalle = new detalle();
            List<CuerpoDocumento> cuerpo_documents = new List<CuerpoDocumento>();
            //   List<string> lista = Regex.Split("20", @"\s+").ToList();

            cxdb1.Open();

            string queryCuerpoDocumento = "exec GENERAR_DTE_FE " + docEntry;
            SqlDataAdapter adapterCuerpoDocumento = new SqlDataAdapter(queryCuerpoDocumento, cxdb1);
            DataSet dataCuerpoDocumento = new DataSet();
            adapterCuerpoDocumento.Fill(dataCuerpoDocumento, "CuerpoDocumento");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtCuerpoDocumento = dataCuerpoDocumento.Tables[3];


            for (int i = 0; i < dtCuerpoDocumento.Rows.Count; i++)
            {
                int numItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["numItem"].ToString());
                int tipoItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["tipoItem"].ToString());
                int uniMedida = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["uniMedida"].ToString());
                double cantidad = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["cantidad"].ToString());
                double precioUni = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["precioUni"].ToString());
                double montoDescu = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["montoDescu"].ToString());
                double ventaNoSuj = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaNoSuj"].ToString());
                double ventaExenta = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaExenta"].ToString());
                double ventaGravada = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaGravada"].ToString());
                double psv = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["psv"].ToString());
                double noGravado = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["noGravado"].ToString());
                double ivaItem = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ivaItem"].ToString());
                string descripcion = Convert.ToString(dtCuerpoDocumento.Rows[i]["descripcion"].ToString());
                string codigo = Convert.ToString(dtCuerpoDocumento.Rows[i]["codigo"].ToString());
                double compra = 0.0;
                cuerpo_documents.Add(adddetalle(numItem, tipoItem, null, codigo, null, descripcion.Replace("\\n", "\n"), cantidad, uniMedida, precioUni, montoDescu, ventaNoSuj, ventaExenta, ventaGravada, null, psv, noGravado, ivaItem, compra));
            }
            detalle.cuerpoDocumento = cuerpo_documents;
            detalles.Add(detalle);

            txtmensaje.Text = "cuerpo de documento  " + docEntry;

            cxdb1.Close();


            //APARTADO RESUMEN

            Resumen res = new Resumen();
            List<Tributo> tributos = new List<Tributo>();
            cxdb1.Open();

            string queryResumen = "exec GENERAR_DTE_FE " + docEntry;
            SqlDataAdapter adapterResumen = new SqlDataAdapter(queryResumen, cxdb1);
            DataSet dataResumen = new DataSet();
            adapterResumen.Fill(dataResumen, "Resumen");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtResumen = dataResumen.Tables[4];


            for (int i = 0; i < dtResumen.Rows.Count; i++)
            {
                res.totalNoSuj= Convert.ToDouble(dtResumen.Rows[i]["totalNoSuj"].ToString());
                res.totalExenta = Convert.ToDouble(dtResumen.Rows[i]["totalExenta"].ToString());
                res.totalGravada = Convert.ToDouble(dtResumen.Rows[i]["totalGravada"].ToString());
                res.subTotalVentas = Convert.ToDouble(dtResumen.Rows[i]["subTotalVentas"].ToString());
                res.descuNoSuj = Convert.ToDouble(dtResumen.Rows[i]["descuNoSuj"].ToString());
                res.descuExenta = Convert.ToDouble(dtResumen.Rows[i]["descuExenta"].ToString());
                res.descuGravada = Convert.ToDouble(dtResumen.Rows[i]["descuGravada"].ToString());

                txtmensaje.Text = dtResumen.Rows[i]["porcentajeDescuento"].ToString() + docEntry;

                if (dtResumen.Rows[i]["porcentajeDescuento"].ToString()=="")
                {
                    res.porcentajeDescuento = 0.0;
                }
                else
                {
                    res.porcentajeDescuento = Convert.ToDouble(dtResumen.Rows[i]["porcentajeDescuento"].ToString());
                }
               
                res.totalDescu = Convert.ToDouble(dtResumen.Rows[i]["totalDescu"].ToString());
                res.totalIva = Convert.ToDouble(dtResumen.Rows[i]["totalIva"].ToString());
                res.tributos = null;
                res.subTotal = Convert.ToDouble(dtResumen.Rows[i]["subTotal"].ToString());
                res.ivaPerci1 = 0.00;
                res.ivaRete1 = Convert.ToDouble(dtResumen.Rows[i]["ivaRete1"].ToString());
                res.reteRenta = Convert.ToDouble(dtResumen.Rows[i]["reteRenta"].ToString());
                res.montoTotalOperacion = Convert.ToDouble(dtResumen.Rows[i]["montoTotalOperacion"].ToString());
                res.totalNoGravado = Convert.ToDouble(dtResumen.Rows[i]["totalNoGravado"].ToString());
                res.totalPagar = Convert.ToDouble(dtResumen.Rows[i]["totalPagar"].ToString());
                res.totalLetras = Convert.ToString(dtResumen.Rows[i]["totalLetras"].ToString());
                res.saldoFavor = Convert.ToDouble(dtResumen.Rows[i]["saldoFavor"].ToString());
                res.condicionOperacion = Convert.ToInt32(dtResumen.Rows[i]["condicionOperacion"].ToString());
                res.numPagoElectronico = null;

            }



            resumen.Add(res);

            txtmensaje.Text = "resumen  " + docEntry;
            cxdb1.Close();
            //APARTADO DETALLE EXTENSION

            detalle_extension de = new detalle_extension();
                List<Extension> exts = new List<Extension>();

            cxdb1.Open();

            string queryExtension = "exec GENERAR_DTE_FE " + docEntry;
            SqlDataAdapter adapterExtension = new SqlDataAdapter(queryExtension, cxdb1);
            DataSet dataExtension = new DataSet();
            adapterExtension.Fill(dataExtension, "Extension");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtExtension = dataExtension.Tables[5];


            for (int i = 0; i < dtExtension.Rows.Count; i++)
            {
                string nombEntrega = Convert.ToString(dtExtension.Rows[i]["nombEntrega"].ToString());
                string docuEntrega = Convert.ToString(dtExtension.Rows[i]["docuEntrega"].ToString());
                string codEmpleado = Convert.ToString(dtExtension.Rows[i]["codEmpleado"].ToString());
                string nombRecibe = Convert.ToString(dtExtension.Rows[i]["nombRecibe"].ToString());
                string docuRecibe = Convert.ToString(dtExtension.Rows[i]["docuRecibe"].ToString());
                string placaVehiculo = Convert.ToString(dtExtension.Rows[i]["placaVehiculo"].ToString());
                string observaciones = Convert.ToString(dtExtension.Rows[i]["observaciones"].ToString());
                if (nombRecibe.ToString().Trim() != "" && docuRecibe.ToString().Trim() != "" && nombEntrega.ToString().Trim() != "" && docuEntrega.ToString().Trim() != "")
                {
                    exts.Add(addExtension(nombEntrega, docuEntrega, nombRecibe, docuRecibe, observaciones, placaVehiculo));
                }
                else
                {
                   
                }
              
            }

            if (exts.Count <= 0)
                {
                    de.extension = null;
                }
                else
                {
                    de.extension = exts;
                }

                detalle_extension.Add(de);

            txtmensaje.Text = "extension  " + docEntry;

            cxdb1.Close();


            //APARTADO DETALLE_APENDICE

            detalle_apendice da = new detalle_apendice();
            List<Apendice> apend = new List<Apendice>();

            cxdb1.Open();

            string queryApendice = "exec GENERAR_DTE_FE " + docEntry;
            SqlDataAdapter adapterApendice = new SqlDataAdapter(queryApendice, cxdb1);
            DataSet dataApendice = new DataSet();
            adapterApendice.Fill(dataApendice, "Apendice");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtApendice = dataApendice.Tables[6];


            for (int i = 0; i < dtApendice.Rows.Count; i++)
            {
                string campo = Convert.ToString(dtApendice.Rows[i]["campo"].ToString());
                string etiqueta = Convert.ToString(dtApendice.Rows[i]["etiqueta"].ToString());
                string valor = Convert.ToString(dtApendice.Rows[i]["valor"].ToString());

                apend.Add(addApendice(campo, etiqueta, valor));
            }

            if (apend.Count <= 0)
            {
                da.apendice = null;
            }
            else
            {
                da.apendice = apend;
            }

            cxdb1.Close();
            detalle_apendice.Add(da);



            string str_identificacion = JsonConvert.SerializeObject(iden, Newtonsoft.Json.Formatting.Indented);
                string str_emisor = JsonConvert.SerializeObject(emi, Newtonsoft.Json.Formatting.Indented);
                string str_receptor = JsonConvert.SerializeObject(rec, Newtonsoft.Json.Formatting.Indented);
                string str_cuerpodoc = JsonConvert.SerializeObject(detalles, Newtonsoft.Json.Formatting.Indented);
                string cadenaRecortada1 = str_cuerpodoc.Remove(str_cuerpodoc.Length - 5);
                string cadenaRecortada2 = cadenaRecortada1.Remove(0, 6);
                string str_resumen = JsonConvert.SerializeObject(resumen, Newtonsoft.Json.Formatting.Indented);
                string cadenaRecortada1_resumen = str_resumen.Remove(str_resumen.Length - 1);
                string cadenaRecortada2_resumen = cadenaRecortada1_resumen.Remove(0, 1);

                string str_relacionado = JsonConvert.SerializeObject(dt_re, Newtonsoft.Json.Formatting.Indented);
                string cadenaRecortada1_relacionado = str_relacionado.Remove(str_relacionado.Length - 5);
                string cadenaRecortada2_relacionado = cadenaRecortada1_relacionado.Remove(0, 6);


                string str_extension = JsonConvert.SerializeObject(detalle_extension, Newtonsoft.Json.Formatting.Indented);
                string cadenaRecortada1_extension = str_extension.Remove(str_extension.Length - 5);
                string cadenaRecortada2_extension = cadenaRecortada1_extension.Remove(0, 6);


                string str_apendice = JsonConvert.SerializeObject(detalle_apendice, Newtonsoft.Json.Formatting.Indented);
                string cadenaRecortada1_apendice = str_apendice.Remove(str_apendice.Length - 5);
                string cadenaRecortada2_apendice = cadenaRecortada1_apendice.Remove(0, 6);







                string var_identificacion = "\"identificacion\":" + str_identificacion;
                string var_emisor = "\"emisor\":" + str_emisor;
                string var_receptor = "\"receptor\":" + str_receptor;
                string var_otrosDocumentos = "\"otrosDocumentos\":" + "null";
                string var_ventaTercero = "\"ventaTercero\":" + "null";
                string var_cuerpo = cadenaRecortada2.Trim();
                string var_resumen = "\"resumen\":" + cadenaRecortada2_resumen.Trim();
                string var_extension = cadenaRecortada2_extension.Trim();
                string var_apendice = cadenaRecortada2_apendice.Trim();

                string json_completo = "{\n" + var_identificacion + ",\n" + cadenaRecortada2_relacionado.Trim() + ",\n" + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_receptor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_otrosDocumentos + ",\n" + var_ventaTercero + ",\n" + var_cuerpo + ",\n" + var_resumen + ",\n" + var_extension.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_apendice + "\n}";

            byte[] bytesUtf8 = Encoding.UTF8.GetBytes(json_completo);

   
            string textoDecodificado = Encoding.UTF8.GetString(bytesUtf8);



            System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", textoDecodificado.Trim());

            cxdb1.Open();

            string updateQuery = "UPDATE RICOH_PRINT SET impreso = @impreso  WHERE docEntry = @Condicion";

            using (SqlCommand command = new SqlCommand(updateQuery, cxdb1))
            {

                command.Parameters.AddWithValue("@impreso", 1);
                command.Parameters.AddWithValue("@condicion", docEntry);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("Registros actualizados: " + rowsAffected);
            }

            cxdb1.Close();


                identificacion.Clear();
                dt_re.Clear();
                emisor.Clear();
                receptor.Clear();
                detalles.Clear();
                detalle_extension.Clear();
                resumen.Clear();
                detalle_factura.Clear();
                detalle_apendice.Clear();



            
            
            


        }
        public void CF(string docEntry)
        {

            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);


            Identificacion iden = new Identificacion();

            cxdb1.Open();


            string queryIden = "exec GENERAR_DTE_CCFE " + docEntry;
            SqlDataAdapter adapterIden = new SqlDataAdapter(queryIden, cxdb1);
            DataSet dataIden = new DataSet();
            adapterIden.Fill(dataIden, "Identificacion");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtIden = dataIden.Tables[0];

            for (int i = 0; i < dtIden.Rows.Count; i++)
            {
                iden.version = Convert.ToInt32(dtIden.Rows[i]["version"]);
                iden.ambiente = Convert.ToString(dtIden.Rows[i]["ambiente"]).Trim();
                iden.tipoDte = Convert.ToString(dtIden.Rows[i]["tipoDte"]).Trim();
                iden.numeroControl = Convert.ToString(dtIden.Rows[i]["numeroControl"]).Trim();
                iden.codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();

                iden.tipoModelo = Convert.ToInt32(dtIden.Rows[i]["tipoModelo"]);
                iden.tipoOperacion = Convert.ToInt32(dtIden.Rows[i]["tipoOperacion"]);
                iden.tipoContingencia = null;
                iden.motivoContin = null;
                iden.fecEmi = Convert.ToString(dtIden.Rows[i]["fecEmi"]).Trim();
                iden.horEmi = Convert.ToString(dtIden.Rows[i]["horEmi"]).Trim();
                iden.tipoMoneda = Convert.ToString(dtIden.Rows[i]["tipoMoneda"]).Trim();

            }
            cxdb1.Close();


            identificacion.Add(iden);


            detalle_relacionado detalle_re = new detalle_relacionado();
            List<DocumentoRelacionado> documentorelacionado = new List<DocumentoRelacionado>();
            // documentorelacionado.Add(addDocumentosRelacionados("",2,"",""));

            if (documentorelacionado.Count <= 0)
            {
                detalle_re.documentoRelacionado = null;
            }
            else
            {
                detalle_re.documentoRelacionado = documentorelacionado;
            }


            dt_re.Add(detalle_re);





            //APARTADO PARA EMISOR
            Emisor emi = new Emisor();
            cxdb1.Open();

            string query = "exec GENERAR_DTE_CCFE " + docEntry;
            SqlDataAdapter adapter = new SqlDataAdapter(query, cxdb1);
            DataSet dataEmisor = new DataSet();
            adapter.Fill(dataEmisor, "Emisor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtEmisor = dataEmisor.Tables[1];


            List<direccion> direccion_emisor = new List<direccion>();

            for (int i = 0; i < dtEmisor.Rows.Count; i++)
            {

                emi.nit = Convert.ToString(dtEmisor.Rows[i]["nit"].ToString().Trim());
                emi.nrc = Convert.ToString(dtEmisor.Rows[i]["nrc"].ToString().Trim());
                emi.nombre = Convert.ToString(dtEmisor.Rows[i]["nombre"].ToString().Trim());
                emi.codActividad = Convert.ToString(dtEmisor.Rows[i]["codActividad"].ToString().Trim());
                emi.descActividad = Convert.ToString(dtEmisor.Rows[i]["descActividad"].ToString().Trim());
                emi.nombreComercial = Convert.ToString(dtEmisor.Rows[i]["nombreComercial"].ToString().Trim());
                emi.tipoEstablecimiento = Convert.ToString(dtEmisor.Rows[i]["tipoEstablecimiento"].ToString().Trim());
                direccion_emisor.Add(adddireccionemisor(Convert.ToString(dtEmisor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.municipo"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.complemento"].ToString().Trim())));
                if (Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    emi.telefono = Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString()).Trim();
                }
                else
                {
                    emi.telefono = "25252800";
                }


                if (Convert.ToString(dtEmisor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    emi.correo = Convert.ToString(dtEmisor.Rows[i]["correo"].ToString()).Trim();
                }
                else
                {
                    emi.correo = "contabilidad@motocity.com.sv";
                }
                emi.codEstableMH = Convert.ToString(dtEmisor.Rows[i]["codEstableMH"].ToString().Trim());
                emi.codEstable = Convert.ToString(dtEmisor.Rows[i]["codEstable"].ToString().Trim());
                emi.codPuntoVentaMH = Convert.ToString(dtEmisor.Rows[i]["codPuntoVentaMH"].ToString().Trim());
                emi.codPuntoVenta = Convert.ToString(dtEmisor.Rows[i]["codPuntoVenta"].ToString().Trim());

            }


            emi.direccion = direccion_emisor;

            emisor.Add(emi);

            cxdb1.Close();


            //APARTADO PARA RECEPTOR

            Receptor rec = new Receptor();
            List<direccion> direccion_receptor = new List<direccion>();

            cxdb1.Open();

            string queryReceptor = "exec GENERAR_DTE_CCFE " + docEntry;
            SqlDataAdapter adapterReceptor = new SqlDataAdapter(queryReceptor, cxdb1);
            DataSet dataReceptor = new DataSet();
            adapterReceptor.Fill(dataReceptor, "Receptor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtReceptor = dataReceptor.Tables[2];

            for (int i = 0; i < dtReceptor.Rows.Count; i++)
            {


                if (Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim()) != "")
                {
                    rec.tipoDocumento = Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim());
                }
                else
                {
                    rec.tipoDocumento = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim()) != "")
                {
                    rec.numDocumento = Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim());
                }
                else
                {
                    rec.numDocumento = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim()) != "")
                {
                    rec.nit = Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim());
                }
                else
                {
                    rec.nit = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim()) != "")
                {
                    rec.nrc = Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim());
                }
                else
                {
                    rec.nrc = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim()) != "")
                {
                    rec.nombre = Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim());
                }
                else
                {
                    rec.nombre = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim()) != "")
                {
                    rec.codActividad = Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim());
                }
                else
                {
                    rec.codActividad = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim()) != "")
                {
                    rec.descActividad = Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim());
                }
                else
                {
                    rec.descActividad = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["nombreComercial"].ToString().Trim()) != "")
                {
                    rec.nombreComercial = Convert.ToString(dtReceptor.Rows[i]["nombreComercial"].ToString().Trim());
                }
                else
                {
                    rec.nombreComercial = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()) != "")
                {
                    direccion_receptor.Add(adddireccionreceptor(Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.municipio"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.complemento"].ToString().Trim())));
                    rec.direccion = direccion_receptor;
                }
                else
                {
                    rec.direccion = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    rec.telefono = Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim());
                }
                else
                {
                    rec.telefono = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    rec.correo = Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim());
                }
                else
                {
                    rec.correo = null;
                }



            }




            receptor.Add(rec);

            cxdb1.Close();


            //APARTADO CUERPO DE DOCUMENTO

            detalle detalle = new detalle();
            List<CuerpoDocumento> cuerpo_documents = new List<CuerpoDocumento>();
               List<string> lista = Regex.Split("20", @"\s+").ToList();

            cxdb1.Open();

            string queryCuerpoDocumento = "exec GENERAR_DTE_CCFE " + docEntry;
            SqlDataAdapter adapterCuerpoDocumento = new SqlDataAdapter(queryCuerpoDocumento, cxdb1);
            DataSet dataCuerpoDocumento = new DataSet();
            adapterCuerpoDocumento.Fill(dataCuerpoDocumento, "CuerpoDocumento");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtCuerpoDocumento = dataCuerpoDocumento.Tables[3];


            for (int i = 0; i < dtCuerpoDocumento.Rows.Count; i++)
            {
                int numItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["numItem"].ToString());
                int tipoItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["tipoItem"].ToString());
                int uniMedida = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["uniMedida"].ToString());
                double cantidad = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["cantidad"].ToString());
                double precioUni = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["precioUni"].ToString());
                double montoDescu = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["montoDescu"].ToString());
                double ventaNoSuj = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaNoSuj"].ToString());
                double ventaExenta = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaExenta"].ToString());
                double ventaGravada = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaGravada"].ToString());
                double psv = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["psv"].ToString());
                double noGravado = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["noGravado"].ToString());
                double ivaItem = 0.00;
                string descripcion = Convert.ToString(dtCuerpoDocumento.Rows[i]["descripcion"].ToString());
                string codigo = Convert.ToString(dtCuerpoDocumento.Rows[i]["codigo"].ToString());
                string tributo = Convert.ToString(dtCuerpoDocumento.Rows[i]["tributos"].ToString());
                double compra = 0.0;

                if (tributo.ToString().Trim() == "20")
                {
                    cuerpo_documents.Add(adddetalle(numItem, tipoItem, null, codigo, null, descripcion.Replace("\\n", "\n"), cantidad, uniMedida, precioUni, montoDescu, ventaNoSuj, ventaExenta, ventaGravada, lista, psv, noGravado, ivaItem, compra));

                }
                else
                {
                    cuerpo_documents.Add(adddetalle(numItem, tipoItem, null, codigo, null, descripcion.Replace("\\n", "\n"), cantidad, uniMedida, precioUni, montoDescu, ventaNoSuj, ventaExenta, ventaGravada, null, psv, noGravado, ivaItem,compra));

                }


            }
            detalle.cuerpoDocumento = cuerpo_documents;
            detalles.Add(detalle);

            cxdb1.Close();


            //APARTADO RESUMEN

            Resumen res = new Resumen();
            List<Tributo> tributos = new List<Tributo>();
            cxdb1.Open();

            string queryResumen = "exec GENERAR_DTE_CCFE " + docEntry;
            SqlDataAdapter adapterResumen = new SqlDataAdapter(queryResumen, cxdb1);
            DataSet dataResumen = new DataSet();
            adapterResumen.Fill(dataResumen, "Resumen");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtResumen = dataResumen.Tables[4];


            for (int i = 0; i < dtResumen.Rows.Count; i++)
            {
                res.totalNoSuj = Convert.ToDouble(dtResumen.Rows[i]["totalNoSuj"].ToString());
                res.totalExenta = Convert.ToDouble(dtResumen.Rows[i]["totalExenta"].ToString());
                res.totalGravada = Convert.ToDouble(dtResumen.Rows[i]["totalGravada"].ToString());
                res.subTotalVentas = Convert.ToDouble(dtResumen.Rows[i]["subTotalVentas"].ToString());
                res.descuNoSuj = Convert.ToDouble(dtResumen.Rows[i]["descuNoSuj"].ToString());
                res.descuExenta = Convert.ToDouble(dtResumen.Rows[i]["descuExenta"].ToString());
                res.descuGravada = Convert.ToDouble(dtResumen.Rows[i]["descuGravada"].ToString());
                res.porcentajeDescuento = Convert.ToDouble(dtResumen.Rows[i]["porcentajeDescuento"].ToString());
                res.totalDescu = Convert.ToDouble(dtResumen.Rows[i]["totalDescu"].ToString());
                res.totalIva = 0.00;
                tributos.Add(addtributos("20", "Impuesto al Valor Agregado 13%", Convert.ToDouble(dtResumen.Rows[i]["tributo.valor"])));
                res.subTotal = Convert.ToDouble(dtResumen.Rows[i]["subTotal"].ToString());
                res.ivaPerci1 = 0.00;
                res.ivaRete1 = Convert.ToDouble(dtResumen.Rows[i]["ivaRete1"].ToString());
                res.reteRenta = Convert.ToDouble(dtResumen.Rows[i]["reteRenta"].ToString());
                res.montoTotalOperacion = Convert.ToDouble(dtResumen.Rows[i]["montoTotalOperacion"].ToString());
                res.totalNoGravado = Convert.ToDouble(dtResumen.Rows[i]["totalNoGravado"].ToString());
                res.totalPagar = Convert.ToDouble(dtResumen.Rows[i]["totalPagar"].ToString());
                res.totalLetras = Convert.ToString(dtResumen.Rows[i]["totalLetras"].ToString());
                res.saldoFavor = Convert.ToDouble(dtResumen.Rows[i]["saldoFavor"].ToString());
                res.condicionOperacion = Convert.ToInt32(dtResumen.Rows[i]["condicionOperacion"].ToString());
                res.numPagoElectronico = null;
                
                res.tributos = tributos;

            }



            resumen.Add(res);
            cxdb1.Close();
            //APARTADO DETALLE EXTENSION

            detalle_extension de = new detalle_extension();
            List<Extension> exts = new List<Extension>();

            cxdb1.Open();

            string queryExtension = "exec GENERAR_DTE_CCFE " + docEntry;
            SqlDataAdapter adapterExtension = new SqlDataAdapter(queryExtension, cxdb1);
            DataSet dataExtension = new DataSet();
            adapterExtension.Fill(dataExtension, "Extension");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtExtension = dataExtension.Tables[5];


            for (int i = 0; i < dtExtension.Rows.Count; i++)
            {
                string nombEntrega = Convert.ToString(dtExtension.Rows[i]["nombEntrega"].ToString());
                string docuEntrega = Convert.ToString(dtExtension.Rows[i]["docuEntrega"].ToString());
                string codEmpleado = Convert.ToString(dtExtension.Rows[i]["codEmpleado"].ToString());
                string nombRecibe = Convert.ToString(dtExtension.Rows[i]["nombRecibe"].ToString());
                string docuRecibe = Convert.ToString(dtExtension.Rows[i]["docuRecibe"].ToString());
                string placaVehiculo = Convert.ToString(dtExtension.Rows[i]["placaVehiculo"].ToString());
                string observaciones = Convert.ToString(dtExtension.Rows[i]["observaciones"].ToString());
                if (nombRecibe.ToString().Trim() != "" && docuRecibe.ToString().Trim() != "" && nombEntrega.ToString().Trim() != "" && docuEntrega.ToString().Trim() != "")
                {
                    exts.Add(addExtension(nombEntrega, docuEntrega, nombRecibe, docuRecibe, observaciones, placaVehiculo));
                }
                else
                {

                }

            }

            if (exts.Count <= 0)
            {
                de.extension = null;
            }
            else
            {
                de.extension = exts;
            }

            detalle_extension.Add(de);

            cxdb1.Close();

            //APARTADO DETALLE_APENDICE

            detalle_apendice da = new detalle_apendice();
            List<Apendice> apend = new List<Apendice>();

            cxdb1.Open();

            string queryApendice = "exec GENERAR_DTE_CCFE " + docEntry;
            SqlDataAdapter adapterApendice = new SqlDataAdapter(queryApendice, cxdb1);
            DataSet dataApendice = new DataSet();
            adapterApendice.Fill(dataApendice, "Apendice");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtApendice = dataApendice.Tables[6];


            for (int i = 0; i < dtApendice.Rows.Count; i++)
            {
                string campo = Convert.ToString(dtApendice.Rows[i]["campo"].ToString());
                string etiqueta = Convert.ToString(dtApendice.Rows[i]["etiqueta"].ToString());
                string valor = Convert.ToString(dtApendice.Rows[i]["valor"].ToString());

                apend.Add(addApendice(campo, etiqueta, valor));
            }

            if (apend.Count <= 0)
            {
                da.apendice = null;
            }
            else
            {
                da.apendice = apend;
            }

            cxdb1.Close();
            detalle_apendice.Add(da);




            string str_identificacion = JsonConvert.SerializeObject(iden, Newtonsoft.Json.Formatting.Indented);
            string str_emisor = JsonConvert.SerializeObject(emi, Newtonsoft.Json.Formatting.Indented);
            string str_receptor = JsonConvert.SerializeObject(rec, Newtonsoft.Json.Formatting.Indented);
            string str_cuerpodoc = JsonConvert.SerializeObject(detalles, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1 = str_cuerpodoc.Remove(str_cuerpodoc.Length - 5);
            string cadenaRecortada2 = cadenaRecortada1.Remove(0, 6);
            string str_resumen = JsonConvert.SerializeObject(resumen, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_resumen = str_resumen.Remove(str_resumen.Length - 1);
            string cadenaRecortada2_resumen = cadenaRecortada1_resumen.Remove(0, 1);

            string str_relacionado = JsonConvert.SerializeObject(dt_re, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_relacionado = str_relacionado.Remove(str_relacionado.Length - 5);
            string cadenaRecortada2_relacionado = cadenaRecortada1_relacionado.Remove(0, 6);


            string str_extension = JsonConvert.SerializeObject(detalle_extension, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_extension = str_extension.Remove(str_extension.Length - 5);
            string cadenaRecortada2_extension = cadenaRecortada1_extension.Remove(0, 6);


            string str_apendice = JsonConvert.SerializeObject(detalle_apendice, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_apendice = str_apendice.Remove(str_apendice.Length - 5);
            string cadenaRecortada2_apendice = cadenaRecortada1_apendice.Remove(0, 6);







            string var_identificacion = "\"identificacion\":" + str_identificacion;
            string var_emisor = "\"emisor\":" + str_emisor;
            string var_receptor = "\"receptor\":" + str_receptor;
            string var_otrosDocumentos = "\"otrosDocumentos\":" + "null";
            string var_ventaTercero = "\"ventaTercero\":" + "null";
            string var_cuerpo = cadenaRecortada2.Trim();
            string var_resumen = "\"resumen\":" + cadenaRecortada2_resumen.Trim();
            string var_extension = cadenaRecortada2_extension.Trim();
            string var_apendice = cadenaRecortada2_apendice.Trim();

            string json_completo = "{\n" + var_identificacion + ",\n" + cadenaRecortada2_relacionado.Trim() + ",\n" + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_receptor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_otrosDocumentos + ",\n" + var_ventaTercero + ",\n" + var_cuerpo + ",\n" + var_resumen + ",\n" + var_extension.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_apendice + "\n}";

            byte[] bytesUtf8 = Encoding.UTF8.GetBytes(json_completo);


            string textoDecodificado = Encoding.UTF8.GetString(bytesUtf8);



            System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", textoDecodificado.Trim());

            cxdb1.Open();

            string updateQuery = "UPDATE RICOH_PRINT SET impreso = @impreso  WHERE docEntry = @Condicion";

            using (SqlCommand command = new SqlCommand(updateQuery, cxdb1))
            {

                command.Parameters.AddWithValue("@impreso", 1);
                command.Parameters.AddWithValue("@condicion", docEntry);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("Registros actualizados: " + rowsAffected);
            }

            cxdb1.Close();


            identificacion.Clear();
            dt_re.Clear();
            emisor.Clear();
            receptor.Clear();
            detalles.Clear();
            detalle_extension.Clear();
            resumen.Clear();
            detalle_factura.Clear();
            detalle_apendice.Clear();








        }

        public void NCL(string docEntry)
        {

            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);


            Identificacion iden = new Identificacion();

            cxdb1.Open();


            string queryIden = "exec GENERAR_DTE_NCE " + docEntry;
            SqlDataAdapter adapterIden = new SqlDataAdapter(queryIden, cxdb1);
            DataSet dataIden = new DataSet();
            adapterIden.Fill(dataIden, "Identificacion");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtIden = dataIden.Tables[0];

            for (int i = 0; i < dtIden.Rows.Count; i++)
            {
                iden.version = Convert.ToInt32(dtIden.Rows[i]["version"]);
                iden.ambiente = Convert.ToString(dtIden.Rows[i]["ambiente"]).Trim();
                iden.tipoDte = Convert.ToString(dtIden.Rows[i]["tipoDte"]).Trim();
                iden.numeroControl = Convert.ToString(dtIden.Rows[i]["numeroControl"]).Trim();
                iden.codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();

                iden.tipoModelo = Convert.ToInt32(dtIden.Rows[i]["tipoModelo"]);
                iden.tipoOperacion = Convert.ToInt32(dtIden.Rows[i]["tipoOperacion"]);
                iden.tipoContingencia = null;
                iden.motivoContin = null;
                iden.fecEmi = Convert.ToString(dtIden.Rows[i]["fecEmi"]).Trim();
                iden.horEmi = Convert.ToString(dtIden.Rows[i]["horEmi"]).Trim();
                iden.tipoMoneda = Convert.ToString(dtIden.Rows[i]["tipoMoneda"]).Trim();

            }
            cxdb1.Close();


            identificacion.Add(iden);


            detalle_relacionado detalle_re = new detalle_relacionado();
            List<DocumentoRelacionado> documentorelacionado = new List<DocumentoRelacionado>();
         

            cxdb1.Open();

            string queryDocumentoRelacionado = "exec GENERAR_DTE_NCE " + docEntry;
            SqlDataAdapter adapterDocumentoRelacionado = new SqlDataAdapter(queryDocumentoRelacionado, cxdb1);
            DataSet dataDocumentoRelacionado = new DataSet();
            adapterDocumentoRelacionado.Fill(dataDocumentoRelacionado, "Documento Relacionado");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtDocumentoRelacionado = dataDocumentoRelacionado.Tables[1];


            for (int i = 0; i < dtDocumentoRelacionado.Rows.Count; i++)
            {
                string tipoDocumento = Convert.ToString(dtDocumentoRelacionado.Rows[i]["tipoDocumento"].ToString().Trim());
                int tipoGeneracion = Convert.ToInt32(dtDocumentoRelacionado.Rows[i]["tipoGeneracion"].ToString().Trim());
                string numeroDocumento = Convert.ToString(dtDocumentoRelacionado.Rows[i]["numeroDocumento"].ToString().Trim());
                string fechaEmision = Convert.ToString(dtDocumentoRelacionado.Rows[i]["fechaEmision"].ToString().Trim());


                documentorelacionado.Add(addDocumentosRelacionados(tipoDocumento,tipoGeneracion,numeroDocumento,fechaEmision));
            }





            if (documentorelacionado.Count <= 0)
            {
                detalle_re.documentoRelacionado = null;
            }
            else
            {
                detalle_re.documentoRelacionado = documentorelacionado;
            }


            dt_re.Add(detalle_re);


            cxdb1.Close();




            //APARTADO PARA EMISOR
            Emisor emi = new Emisor();
            cxdb1.Open();

            string query = "exec GENERAR_DTE_NCE " + docEntry;
            SqlDataAdapter adapter = new SqlDataAdapter(query, cxdb1);
            DataSet dataEmisor = new DataSet();
            adapter.Fill(dataEmisor, "Emisor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtEmisor = dataEmisor.Tables[2];


            List<direccion> direccion_emisor = new List<direccion>();

            for (int i = 0; i < dtEmisor.Rows.Count; i++)
            {

                emi.nit = Convert.ToString(dtEmisor.Rows[i]["nit"].ToString().Trim());
                emi.nrc = Convert.ToString(dtEmisor.Rows[i]["nrc"].ToString().Trim());
                emi.nombre = Convert.ToString(dtEmisor.Rows[i]["nombre"].ToString().Trim());
                emi.codActividad = Convert.ToString(dtEmisor.Rows[i]["codActividad"].ToString().Trim());
                emi.descActividad = Convert.ToString(dtEmisor.Rows[i]["descActividad"].ToString().Trim());
                emi.nombreComercial = Convert.ToString(dtEmisor.Rows[i]["nombreComercial"].ToString().Trim());
                emi.tipoEstablecimiento = Convert.ToString(dtEmisor.Rows[i]["tipoEstablecimiento"].ToString().Trim());
                direccion_emisor.Add(adddireccionemisor(Convert.ToString(dtEmisor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.municipo"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.complemento"].ToString().Trim())));
                if (Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    emi.telefono = Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString()).Trim();
                }
                else
                {
                    emi.telefono = "25252800";
                }


                if (Convert.ToString(dtEmisor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    emi.correo = Convert.ToString(dtEmisor.Rows[i]["correo"].ToString()).Trim();
                }
                else
                {
                    emi.correo = "contabilidad@motocity.com.sv";
                }
                emi.codEstableMH = Convert.ToString(dtEmisor.Rows[i]["codEstableMH"].ToString().Trim());
                emi.codEstable = Convert.ToString(dtEmisor.Rows[i]["codEstable"].ToString().Trim());
                emi.codPuntoVentaMH = Convert.ToString(dtEmisor.Rows[i]["codPuntoVentaMH"].ToString().Trim());
                emi.codPuntoVenta = Convert.ToString(dtEmisor.Rows[i]["codPuntoVenta"].ToString().Trim());

            }


            emi.direccion = direccion_emisor;

            emisor.Add(emi);

            cxdb1.Close();


            //APARTADO PARA RECEPTOR

            Receptor rec = new Receptor();
            List<direccion> direccion_receptor = new List<direccion>();

            cxdb1.Open();

            string queryReceptor = "exec GENERAR_DTE_NCE " + docEntry;
            SqlDataAdapter adapterReceptor = new SqlDataAdapter(queryReceptor, cxdb1);
            DataSet dataReceptor = new DataSet();
            adapterReceptor.Fill(dataReceptor, "Receptor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtReceptor = dataReceptor.Tables[3];

            for (int i = 0; i < dtReceptor.Rows.Count; i++)
            {


                if (Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim()) != "")
                {
                    rec.tipoDocumento = Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim());
                }
                else
                {
                    rec.tipoDocumento = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim()) != "")
                {
                    rec.numDocumento = Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim());
                }
                else
                {
                    rec.numDocumento = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim()) != "")
                {
                    rec.nit = Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim());
                }
                else
                {
                    rec.nit = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim()) != "")
                {
                    rec.nrc = Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim());
                }
                else
                {
                    rec.nrc = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim()) != "")
                {
                    rec.nombre = Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim());
                }
                else
                {
                    rec.nombre = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim()) != "")
                {
                    rec.codActividad = Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim());
                }
                else
                {
                    rec.codActividad = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim()) != "")
                {
                    rec.descActividad = Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim());
                }
                else
                {
                    rec.descActividad = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["nombreComercial"].ToString().Trim()) != "")
                {
                    rec.nombreComercial = Convert.ToString(dtReceptor.Rows[i]["nombreComercial"].ToString().Trim());
                }
                else
                {
                    rec.nombreComercial = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()) != "")
                {
                    direccion_receptor.Add(adddireccionreceptor(Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.municipio"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.complemento"].ToString().Trim())));
                    rec.direccion = direccion_receptor;
                }
                else
                {
                    rec.direccion = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    rec.telefono = Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim());
                }
                else
                {
                    rec.telefono = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    rec.correo = Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim());
                }
                else
                {
                    rec.correo = null;
                }



            }




            receptor.Add(rec);

            cxdb1.Close();


            //APARTADO CUERPO DE DOCUMENTO

            detalle detalle = new detalle();
            List<CuerpoDocumento> cuerpo_documents = new List<CuerpoDocumento>();
            List<string> lista = Regex.Split("20", @"\s+").ToList();

            cxdb1.Open();

            string queryCuerpoDocumento = "exec GENERAR_DTE_NCE " + docEntry;
            SqlDataAdapter adapterCuerpoDocumento = new SqlDataAdapter(queryCuerpoDocumento, cxdb1);
            DataSet dataCuerpoDocumento = new DataSet();
            adapterCuerpoDocumento.Fill(dataCuerpoDocumento, "CuerpoDocumento");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtCuerpoDocumento = dataCuerpoDocumento.Tables[4];


            for (int i = 0; i < dtCuerpoDocumento.Rows.Count; i++)
            {
                int numItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["numItem"].ToString());
                int tipoItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["tipoItem"].ToString());
                int uniMedida = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["uniMedida"].ToString());
                double cantidad = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["cantidad"].ToString());
                double precioUni = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["precioUni"].ToString());
                double montoDescu = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["montoDescu"].ToString());
                double ventaNoSuj = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaNoSuj"].ToString());
                double ventaExenta = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaExenta"].ToString());
                double ventaGravada = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaGravada"].ToString());
                double psv = 0.0;
                double noGravado = 0.0;
                double ivaItem = 0.00;
                string descripcion = Convert.ToString(dtCuerpoDocumento.Rows[i]["descripcion"].ToString());
                string numeroDocumento = Convert.ToString(dtCuerpoDocumento.Rows[i]["numeroDocumento"].ToString());
                string codigo = Convert.ToString(dtCuerpoDocumento.Rows[i]["codigo"].ToString());
                double compra = 0.0;
                cuerpo_documents.Add(adddetalle(numItem, tipoItem, numeroDocumento, codigo, null, descripcion.Replace("\\n", "\n"), cantidad, uniMedida, precioUni, montoDescu, ventaNoSuj, ventaExenta, ventaGravada, lista, psv, noGravado, ivaItem,compra));
            }
            detalle.cuerpoDocumento = cuerpo_documents;
            detalles.Add(detalle);

            cxdb1.Close();


            //APARTADO RESUMEN

            Resumen res = new Resumen();
            List<Tributo> tributos = new List<Tributo>();
            cxdb1.Open();

            string queryResumen = "exec GENERAR_DTE_NCE " + docEntry;
            SqlDataAdapter adapterResumen = new SqlDataAdapter(queryResumen, cxdb1);
            DataSet dataResumen = new DataSet();
            adapterResumen.Fill(dataResumen, "Resumen");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtResumen = dataResumen.Tables[5];


            for (int i = 0; i < dtResumen.Rows.Count; i++)
            {
                res.totalNoSuj = Convert.ToDouble(dtResumen.Rows[i]["totalNoSuj"].ToString());
                res.totalExenta = Convert.ToDouble(dtResumen.Rows[i]["totalExenta"].ToString());
                res.totalGravada = Convert.ToDouble(dtResumen.Rows[i]["totalGravada"].ToString());
                res.subTotalVentas = Convert.ToDouble(dtResumen.Rows[i]["subTotalVentas"].ToString());
                res.descuNoSuj = Convert.ToDouble(dtResumen.Rows[i]["descuNoSuj"].ToString());
                res.descuExenta = Convert.ToDouble(dtResumen.Rows[i]["descuExenta"].ToString());
                res.descuGravada = Convert.ToDouble(dtResumen.Rows[i]["descuGravada"].ToString());
                res.porcentajeDescuento = 0.0;
                res.totalDescu = Convert.ToDouble(dtResumen.Rows[i]["totalDescu"].ToString());
                res.totalIva = 0.00;
                tributos.Add(addtributos("20", "Impuesto al Valor Agregado 13%", Convert.ToDouble(dtResumen.Rows[i]["tributo.valor"])));
                res.subTotal = Convert.ToDouble(dtResumen.Rows[i]["subTotal"].ToString());
                res.ivaPerci1 = 0.00;
                res.ivaRete1 = Convert.ToDouble(dtResumen.Rows[i]["ivaRete1"].ToString());
                res.reteRenta = Convert.ToDouble(dtResumen.Rows[i]["reteRenta"].ToString());
                res.montoTotalOperacion = Convert.ToDouble(dtResumen.Rows[i]["montoTotalOperacion"].ToString());
                res.totalNoGravado = 0.00;
                res.totalPagar = 0.00;
                res.totalLetras = Convert.ToString(dtResumen.Rows[i]["totalLetras"].ToString());
                res.saldoFavor = 0.00;
                res.condicionOperacion = Convert.ToInt32(dtResumen.Rows[i]["condicionOperacion"].ToString());
                res.numPagoElectronico = null;

                res.tributos = tributos;

            }



            resumen.Add(res);
            cxdb1.Close();
            //APARTADO DETALLE EXTENSION

            detalle_extension de = new detalle_extension();
            List<Extension> exts = new List<Extension>();

            cxdb1.Open();

            string queryExtension = "exec GENERAR_DTE_NCE " + docEntry;
            SqlDataAdapter adapterExtension = new SqlDataAdapter(queryExtension, cxdb1);
            DataSet dataExtension = new DataSet();
            adapterExtension.Fill(dataExtension, "Extension");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtExtension = dataExtension.Tables[6];


            for (int i = 0; i < dtExtension.Rows.Count; i++)
            {
                string nombEntrega = Convert.ToString(dtExtension.Rows[i]["nombEntrega"].ToString());
                string docuEntrega = Convert.ToString(dtExtension.Rows[i]["docuEntrega"].ToString());
                string codEmpleado = Convert.ToString(dtExtension.Rows[i]["codEmpleado"].ToString());
                string nombRecibe = Convert.ToString(dtExtension.Rows[i]["nombRecibe"].ToString());
                string docuRecibe = Convert.ToString(dtExtension.Rows[i]["docuRecibe"].ToString());
                string placaVehiculo = Convert.ToString(dtExtension.Rows[i]["placaVehiculo"].ToString());
                string observaciones = Convert.ToString(dtExtension.Rows[i]["observaciones"].ToString());
                if (nombRecibe.ToString().Trim() != "" && docuRecibe.ToString().Trim() != "" && nombEntrega.ToString().Trim() != "" && docuEntrega.ToString().Trim() != "")
                {
                    exts.Add(addExtension(nombEntrega, docuEntrega, nombRecibe, docuRecibe, observaciones, placaVehiculo));
                }
                else
                {

                }

            }

            if (exts.Count <= 0)
            {
                de.extension = null;
            }
            else
            {
                de.extension = exts;
            }

            detalle_extension.Add(de);

            cxdb1.Close();


            //APARTADO DETALLE_APENDICE

            detalle_apendice da = new detalle_apendice();
            List<Apendice> apend = new List<Apendice>();

            cxdb1.Open();

            string queryApendice = "exec GENERAR_DTE_NCE " + docEntry;
            SqlDataAdapter adapterApendice = new SqlDataAdapter(queryApendice, cxdb1);
            DataSet dataApendice = new DataSet();
            adapterApendice.Fill(dataApendice, "Apendice");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtApendice = dataApendice.Tables[7];


            for (int i = 0; i < dtApendice.Rows.Count; i++)
            {
                string campo = Convert.ToString(dtApendice.Rows[i]["campo"].ToString());
                string etiqueta = Convert.ToString(dtApendice.Rows[i]["etiqueta"].ToString());
                string valor = Convert.ToString(dtApendice.Rows[i]["valor"].ToString());

                apend.Add(addApendice(campo, etiqueta, valor));
            }

            if (apend.Count <= 0)
            {
                da.apendice = null;
            }
            else
            {
                da.apendice = apend;
            }

            cxdb1.Close();
            detalle_apendice.Add(da);



            string str_identificacion = JsonConvert.SerializeObject(iden, Newtonsoft.Json.Formatting.Indented);
            string str_emisor = JsonConvert.SerializeObject(emi, Newtonsoft.Json.Formatting.Indented);
            string str_receptor = JsonConvert.SerializeObject(rec, Newtonsoft.Json.Formatting.Indented);
            string str_cuerpodoc = JsonConvert.SerializeObject(detalles, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1 = str_cuerpodoc.Remove(str_cuerpodoc.Length - 5);
            string cadenaRecortada2 = cadenaRecortada1.Remove(0, 6);
            string str_resumen = JsonConvert.SerializeObject(resumen, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_resumen = str_resumen.Remove(str_resumen.Length - 1);
            string cadenaRecortada2_resumen = cadenaRecortada1_resumen.Remove(0, 1);

            string str_relacionado = JsonConvert.SerializeObject(dt_re, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_relacionado = str_relacionado.Remove(str_relacionado.Length - 5);
            string cadenaRecortada2_relacionado = cadenaRecortada1_relacionado.Remove(0, 6);


            string str_extension = JsonConvert.SerializeObject(detalle_extension, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_extension = str_extension.Remove(str_extension.Length - 5);
            string cadenaRecortada2_extension = cadenaRecortada1_extension.Remove(0, 6);


            string str_apendice = JsonConvert.SerializeObject(detalle_apendice, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_apendice = str_apendice.Remove(str_apendice.Length - 5);
            string cadenaRecortada2_apendice = cadenaRecortada1_apendice.Remove(0, 6);







            string var_identificacion = "\"identificacion\":" + str_identificacion;
            string var_emisor = "\"emisor\":" + str_emisor;
            string var_receptor = "\"receptor\":" + str_receptor;
            string var_otrosDocumentos = "\"otrosDocumentos\":" + "null";
            string var_ventaTercero = "\"ventaTercero\":" + "null";
            string var_cuerpo = cadenaRecortada2.Trim();
            string var_resumen = "\"resumen\":" + cadenaRecortada2_resumen.Trim();
            string var_extension = cadenaRecortada2_extension.Trim();
            string var_apendice = cadenaRecortada2_apendice.Trim();

            string json_completo = "{\n" + var_identificacion + ",\n" + cadenaRecortada2_relacionado.Trim() + ",\n" + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_receptor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_otrosDocumentos + ",\n" + var_ventaTercero + ",\n" + var_cuerpo + ",\n" + var_resumen + ",\n" + var_extension.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_apendice + "\n}";

            byte[] bytesUtf8 = Encoding.UTF8.GetBytes(json_completo);


            string textoDecodificado = Encoding.UTF8.GetString(bytesUtf8);



            System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", textoDecodificado.Trim());

            cxdb1.Open();

            string updateQuery = "UPDATE RICOH_PRINT SET impreso = @impreso  WHERE docEntry = @Condicion";

            using (SqlCommand command = new SqlCommand(updateQuery, cxdb1))
            {

                command.Parameters.AddWithValue("@impreso", 1);
                command.Parameters.AddWithValue("@condicion", docEntry);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("Registros actualizados: " + rowsAffected);
            }

            cxdb1.Close();


            identificacion.Clear();
            dt_re.Clear();
            emisor.Clear();
            receptor.Clear();
            detalles.Clear();
            detalle_extension.Clear();
            resumen.Clear();
            detalle_factura.Clear();
            detalle_apendice.Clear();








        }

        
        public void NR(string docEntry)
        {
           

            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);


            Identificacion iden = new Identificacion();

            cxdb1.Open();


            string queryIden = "exec GENERAR_DTE_NRE " + docEntry;
            SqlDataAdapter adapterIden = new SqlDataAdapter(queryIden, cxdb1);
            DataSet dataIden = new DataSet();
            adapterIden.Fill(dataIden, "Identificacion");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtIden = dataIden.Tables[0];

            for (int i = 0; i < dtIden.Rows.Count; i++)
            {
                iden.version = Convert.ToInt32(dtIden.Rows[i]["version"]);
                iden.ambiente = Convert.ToString(dtIden.Rows[i]["ambiente"]).Trim();
                iden.tipoDte = Convert.ToString(dtIden.Rows[i]["tipoDte"]).Trim();
                iden.numeroControl = Convert.ToString(dtIden.Rows[i]["numeroControl"]).Trim();
                iden.codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();

                iden.tipoModelo = Convert.ToInt32(dtIden.Rows[i]["tipoModelo"]);
                iden.tipoOperacion = Convert.ToInt32(dtIden.Rows[i]["tipoOperacion"]);
                iden.tipoContingencia = null;
                iden.motivoContin = null;
                iden.fecEmi = Convert.ToString(dtIden.Rows[i]["fecEmi"]).Trim();
                iden.horEmi = Convert.ToString(dtIden.Rows[i]["horEmi"]).Trim();
                iden.tipoMoneda = Convert.ToString(dtIden.Rows[i]["tipoMoneda"]).Trim();

            }
            cxdb1.Close();


            identificacion.Add(iden);


            detalle_relacionado detalle_re = new detalle_relacionado();
            List<DocumentoRelacionado> documentorelacionado = new List<DocumentoRelacionado>();
            // documentorelacionado.Add(addDocumentosRelacionados("",2,"",""));

            if (documentorelacionado.Count <= 0)
            {
                detalle_re.documentoRelacionado = null;
            }
            else
            {
                detalle_re.documentoRelacionado = documentorelacionado;
            }


            dt_re.Add(detalle_re);





            //APARTADO PARA EMISOR
            Emisor emi = new Emisor();
            cxdb1.Open();

            string query = "exec GENERAR_DTE_NRE " + docEntry;
            SqlDataAdapter adapter = new SqlDataAdapter(query, cxdb1);
            DataSet dataEmisor = new DataSet();
            adapter.Fill(dataEmisor, "Emisor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtEmisor = dataEmisor.Tables[1];


            List<direccion> direccion_emisor = new List<direccion>();

            for (int i = 0; i < dtEmisor.Rows.Count; i++)
            {

                emi.nit = Convert.ToString(dtEmisor.Rows[i]["nit"].ToString().Trim());
                emi.nrc = Convert.ToString(dtEmisor.Rows[i]["nrc"].ToString().Trim());
                emi.nombre = Convert.ToString(dtEmisor.Rows[i]["nombre"].ToString().Trim());
                emi.codActividad = Convert.ToString(dtEmisor.Rows[i]["codActividad"].ToString().Trim());
                emi.descActividad = Convert.ToString(dtEmisor.Rows[i]["descActividad"].ToString().Trim());
                emi.nombreComercial = Convert.ToString(dtEmisor.Rows[i]["nombreComercial"].ToString().Trim());
                emi.tipoEstablecimiento = Convert.ToString(dtEmisor.Rows[i]["tipoEstablecimiento"].ToString().Trim());
                direccion_emisor.Add(adddireccionemisor(Convert.ToString(dtEmisor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.municipo"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.complemento"].ToString().Trim())));
                if (Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    emi.telefono = Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString()).Trim();
                }
                else
                {
                    emi.telefono = "25252800";
                }


                if (Convert.ToString(dtEmisor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    emi.correo = Convert.ToString(dtEmisor.Rows[i]["correo"].ToString()).Trim();
                }
                else
                {
                    emi.correo = "contabilidad@motocity.com.sv";
                }
                emi.codEstableMH = Convert.ToString(dtEmisor.Rows[i]["codEstableMH"].ToString().Trim());
                emi.codEstable = Convert.ToString(dtEmisor.Rows[i]["codEstable"].ToString().Trim());
                emi.codPuntoVentaMH = Convert.ToString(dtEmisor.Rows[i]["codPuntoVentaMH"].ToString().Trim());
                emi.codPuntoVenta = Convert.ToString(dtEmisor.Rows[i]["codPuntoVenta"].ToString().Trim());

            }


            emi.direccion = direccion_emisor;

            emisor.Add(emi);

            cxdb1.Close();


            //APARTADO PARA RECEPTOR

            Receptor rec = new Receptor();
            List<direccion> direccion_receptor = new List<direccion>();

            cxdb1.Open();

            string queryReceptor = "exec GENERAR_DTE_NRE " + docEntry;
            SqlDataAdapter adapterReceptor = new SqlDataAdapter(queryReceptor, cxdb1);
            DataSet dataReceptor = new DataSet();
            adapterReceptor.Fill(dataReceptor, "Receptor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtReceptor = dataReceptor.Tables[2];

            for (int i = 0; i < dtReceptor.Rows.Count; i++)
            {


                if (Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim()) != "")
                {
                    rec.tipoDocumento = Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim());
                }
                else
                {
                    rec.tipoDocumento = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim()) != "")
                {
                    rec.numDocumento = Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim());
                }
                else
                {
                    rec.numDocumento = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim()) != "")
                {
                    rec.nit = Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim());
                }
                else
                {
                    rec.nit = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim()) != "")
                {
                    rec.nrc = Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim());
                }
                else
                {
                    rec.nrc = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim()) != "")
                {
                    rec.nombre = Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim());
                }
                else
                {
                    rec.nombre = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim()) != "")
                {
                    rec.codActividad = Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim());
                }
                else
                {
                    rec.codActividad = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim()) != "")
                {
                    rec.descActividad = Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim());
                }
                else
                {
                    rec.descActividad = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["bienTitulo"].ToString().Trim()) != "")
                {
                    rec.bienTitulo = Convert.ToString(dtReceptor.Rows[i]["bienTitulo"].ToString().Trim());
                }
                else
                {
                    rec.bienTitulo = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()) != "")
                {
                    direccion_receptor.Add(adddireccionreceptor(Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.municipio"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.complemento"].ToString().Trim())));
                    rec.direccion = direccion_receptor;
                }
                else
                {
                    rec.direccion = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    rec.telefono = Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim());
                }
                else
                {
                    rec.telefono = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    rec.correo = Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim());
                }
                else
                {
                    rec.correo = null;
                }



            }


            txtmensaje.Text = docEntry + "NR" + "2";

            receptor.Add(rec);

            cxdb1.Close();


            //APARTADO CUERPO DE DOCUMENTO

            detalle detalle = new detalle();
            List<CuerpoDocumento> cuerpo_documents = new List<CuerpoDocumento>();
              List<string> lista = Regex.Split("20", @"\s+").ToList();

            cxdb1.Open();

            string queryCuerpoDocumento = "exec GENERAR_DTE_NRE " + docEntry;
            SqlDataAdapter adapterCuerpoDocumento = new SqlDataAdapter(queryCuerpoDocumento, cxdb1);
            DataSet dataCuerpoDocumento = new DataSet();
            adapterCuerpoDocumento.Fill(dataCuerpoDocumento, "CuerpoDocumento");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtCuerpoDocumento = dataCuerpoDocumento.Tables[3];


            for (int i = 0; i < dtCuerpoDocumento.Rows.Count; i++)
            {
                int numItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["numItem"].ToString());
                int tipoItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["tipoItem"].ToString());
                int uniMedida = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["uniMedida"].ToString());
                double cantidad = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["cantidad"].ToString());
                double precioUni = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["precioUni"].ToString());
                double montoDescu = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["montoDescu"].ToString());
                double ventaNoSuj = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaNoSuj"].ToString());
                double ventaExenta = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaExenta"].ToString());
                double ventaGravada = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaGravada"].ToString());
                double psv = 0.00;
                double noGravado = 0.00;
                double ivaItem = 0.00;
                string descripcion = Convert.ToString(dtCuerpoDocumento.Rows[i]["descripcion"].ToString());
                string codigo = Convert.ToString(dtCuerpoDocumento.Rows[i]["codigo"].ToString());
                double compra = 0.0;
                cuerpo_documents.Add(adddetalle(numItem, tipoItem, null, codigo, null, descripcion.Replace("\\n", "\n"), cantidad, uniMedida, precioUni, montoDescu, ventaNoSuj, ventaExenta, ventaGravada, lista, psv, noGravado, ivaItem,compra));
            }
            detalle.cuerpoDocumento = cuerpo_documents;
            detalles.Add(detalle);

            cxdb1.Close();
            txtmensaje.Text = docEntry + "NR" + "3";

            //APARTADO RESUMEN

            Resumen res = new Resumen();
            List<Tributo> tributos = new List<Tributo>();
            cxdb1.Open();

            string queryResumen = "exec GENERAR_DTE_NRE " + docEntry;
            SqlDataAdapter adapterResumen = new SqlDataAdapter(queryResumen, cxdb1);
            DataSet dataResumen = new DataSet();
            adapterResumen.Fill(dataResumen, "Resumen");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtResumen = dataResumen.Tables[4];


            for (int i = 0; i < dtResumen.Rows.Count; i++)
            {
                res.totalNoSuj = Convert.ToDouble(dtResumen.Rows[i]["totalNoSuj"].ToString());
                res.totalExenta = Convert.ToDouble(dtResumen.Rows[i]["totalExenta"].ToString());
                res.totalGravada = Convert.ToDouble(dtResumen.Rows[i]["totalGravada"].ToString());
                res.subTotalVentas = Convert.ToDouble(dtResumen.Rows[i]["subTotalVentas"].ToString());
                res.descuNoSuj = Convert.ToDouble(dtResumen.Rows[i]["descuNoSuj"].ToString());
                res.descuExenta = Convert.ToDouble(dtResumen.Rows[i]["descuExenta"].ToString());
                res.descuGravada = Convert.ToDouble(dtResumen.Rows[i]["descuGravada"].ToString());
                res.porcentajeDescuento = Convert.ToDouble(dtResumen.Rows[i]["porcentajeDescuento"].ToString());
                res.totalDescu = Convert.ToDouble(dtResumen.Rows[i]["totalDescu"].ToString());
                res.totalIva = 0.00;
                tributos.Add(addtributos("20", "Impuesto al Valor Agregado 13%", Convert.ToDouble(dtResumen.Rows[i]["tributo.valor"])));

                res.tributos = tributos;

                res.subTotal = Convert.ToDouble(dtResumen.Rows[i]["subTotal"].ToString());
                res.ivaPerci1 = 0.00;
                res.ivaRete1 = 0.00;
                res.reteRenta = 0.00;
                res.montoTotalOperacion = Convert.ToDouble(dtResumen.Rows[i]["montoTotalOperacion"].ToString());
                res.totalNoGravado = 0.00;
                res.totalPagar = 0.00;
                res.totalLetras = Convert.ToString(dtResumen.Rows[i]["totalLetras"].ToString());
                res.saldoFavor = 0.00;
                res.condicionOperacion = 1;
                res.numPagoElectronico = null;

            }

            txtmensaje.Text = docEntry + "NR" + "4";

            resumen.Add(res);
            cxdb1.Close();
            //APARTADO DETALLE EXTENSION

            detalle_extension de = new detalle_extension();
            List<Extension> exts = new List<Extension>();

            cxdb1.Open();

            string queryExtension = "exec GENERAR_DTE_NRE " + docEntry;
            SqlDataAdapter adapterExtension = new SqlDataAdapter(queryExtension, cxdb1);
            DataSet dataExtension = new DataSet();
            adapterExtension.Fill(dataExtension, "Extension");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtExtension = dataExtension.Tables[5];


            for (int i = 0; i < dtExtension.Rows.Count; i++)
            {
                string nombEntrega = Convert.ToString(dtExtension.Rows[i]["nombEntrega"].ToString());
                string docuEntrega = Convert.ToString(dtExtension.Rows[i]["docuEntrega"].ToString());
                string codEmpleado = Convert.ToString(dtExtension.Rows[i]["codEmpleado"].ToString());
                string nombRecibe = Convert.ToString(dtExtension.Rows[i]["nombRecibe"].ToString());
                string docuRecibe = Convert.ToString(dtExtension.Rows[i]["docuRecibe"].ToString());
                string placaVehiculo = Convert.ToString(dtExtension.Rows[i]["placaVehiculo"].ToString());
                string observaciones = Convert.ToString(dtExtension.Rows[i]["observaciones"].ToString());
                if (nombRecibe.ToString().Trim() != "" && docuRecibe.ToString().Trim() != "" && nombEntrega.ToString().Trim() != "" && docuEntrega.ToString().Trim() != "")
                {
                    exts.Add(addExtension(nombEntrega, docuEntrega, nombRecibe, docuRecibe, observaciones, placaVehiculo));
                }
                else
                {

                }

            }

            if (exts.Count <= 0)
            {
                de.extension = null;
            }
            else
            {
                de.extension = exts;
            }

            txtmensaje.Text = docEntry + "NR" + "5";
            detalle_extension.Add(de);

            cxdb1.Close();


            //APARTADO DETALLE_APENDICE

            detalle_apendice da = new detalle_apendice();
            List<Apendice> apend = new List<Apendice>();

            cxdb1.Open();

            string queryApendice = "exec GENERAR_DTE_NRE " + docEntry;
            SqlDataAdapter adapterApendice = new SqlDataAdapter(queryApendice, cxdb1);
            DataSet dataApendice = new DataSet();
            adapterApendice.Fill(dataApendice, "Apendice");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtApendice = dataApendice.Tables[6];


            for (int i = 0; i < dtApendice.Rows.Count; i++)
            {
                string campo = Convert.ToString(dtApendice.Rows[i]["campo"].ToString());
                string etiqueta = Convert.ToString(dtApendice.Rows[i]["etiqueta"].ToString());
                string valor = Convert.ToString(dtApendice.Rows[i]["valor"].ToString());

                apend.Add(addApendice(campo, etiqueta, valor));
            }

            if (apend.Count <= 0)
            {
                da.apendice = null;
            }
            else
            {
                da.apendice = apend;
            }

            cxdb1.Close();
            txtmensaje.Text = docEntry + "NR" + "6";
            detalle_apendice.Add(da);


            string str_identificacion = JsonConvert.SerializeObject(iden, Newtonsoft.Json.Formatting.Indented);
            string str_emisor = JsonConvert.SerializeObject(emi, Newtonsoft.Json.Formatting.Indented);
            string str_receptor = JsonConvert.SerializeObject(rec, Newtonsoft.Json.Formatting.Indented);
            string str_cuerpodoc = JsonConvert.SerializeObject(detalles, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1 = str_cuerpodoc.Remove(str_cuerpodoc.Length - 5);
            string cadenaRecortada2 = cadenaRecortada1.Remove(0, 6);
            string str_resumen = JsonConvert.SerializeObject(resumen, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_resumen = str_resumen.Remove(str_resumen.Length - 1);
            string cadenaRecortada2_resumen = cadenaRecortada1_resumen.Remove(0, 1);

            string str_relacionado = JsonConvert.SerializeObject(dt_re, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_relacionado = str_relacionado.Remove(str_relacionado.Length - 5);
            string cadenaRecortada2_relacionado = cadenaRecortada1_relacionado.Remove(0, 6);


            string str_extension = JsonConvert.SerializeObject(detalle_extension, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_extension = str_extension.Remove(str_extension.Length - 5);
            string cadenaRecortada2_extension = cadenaRecortada1_extension.Remove(0, 6);


            string str_apendice = JsonConvert.SerializeObject(detalle_apendice, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_apendice = str_apendice.Remove(str_apendice.Length - 5);
            string cadenaRecortada2_apendice = cadenaRecortada1_apendice.Remove(0, 6);







            string var_identificacion = "\"identificacion\":" + str_identificacion;
            string var_emisor = "\"emisor\":" + str_emisor;
            string var_receptor = "\"receptor\":" + str_receptor;
            string var_otrosDocumentos = "\"otrosDocumentos\":" + "null";
            string var_ventaTercero = "\"ventaTercero\":" + "null";
            string var_cuerpo = cadenaRecortada2.Trim();
            string var_resumen = "\"resumen\":" + cadenaRecortada2_resumen.Trim();
            string var_extension = cadenaRecortada2_extension.Trim();
            string var_apendice = cadenaRecortada2_apendice.Trim();

            string json_completo = "{\n" + var_identificacion + ",\n" + cadenaRecortada2_relacionado.Trim() + ",\n" + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_receptor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_otrosDocumentos + ",\n" + var_ventaTercero + ",\n" + var_cuerpo + ",\n" + var_resumen + ",\n" + var_extension.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_apendice + "\n}";

            byte[] bytesUtf8 = Encoding.UTF8.GetBytes(json_completo);


            string textoDecodificado = Encoding.UTF8.GetString(bytesUtf8);



            System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", textoDecodificado.Trim());

            cxdb1.Open();

            string updateQuery = "UPDATE RICOH_PRINT SET impreso = @impreso  WHERE docEntry = @Condicion";

            using (SqlCommand command = new SqlCommand(updateQuery, cxdb1))
            {

                command.Parameters.AddWithValue("@impreso", 1);
                command.Parameters.AddWithValue("@condicion", docEntry);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("Registros actualizados: " + rowsAffected);
            }

            cxdb1.Close();


            identificacion.Clear();
            dt_re.Clear();
            emisor.Clear();
            receptor.Clear();
            detalles.Clear();
            detalle_extension.Clear();
            resumen.Clear();
            detalle_factura.Clear();
            detalle_apendice.Clear();







        }


        public void SJ(string docEntry)
        {

            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);


            Identificacion iden = new Identificacion();

            cxdb1.Open();


            string queryIden = "exec GENERAR_DTE_FSEE " + docEntry;
            SqlDataAdapter adapterIden = new SqlDataAdapter(queryIden, cxdb1);
            DataSet dataIden = new DataSet();
            adapterIden.Fill(dataIden, "Identificacion");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtIden = dataIden.Tables[0];

            for (int i = 0; i < dtIden.Rows.Count; i++)
            {
                iden.version = Convert.ToInt32(dtIden.Rows[i]["version"]);
                iden.ambiente = Convert.ToString(dtIden.Rows[i]["ambiente"]).Trim();
                iden.tipoDte = Convert.ToString(dtIden.Rows[i]["tipoDte"]).Trim();
                iden.numeroControl = Convert.ToString(dtIden.Rows[i]["numeroControl"]).Trim();
                iden.codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();

                iden.tipoModelo = Convert.ToInt32(dtIden.Rows[i]["tipoModelo"]);
                iden.tipoOperacion = Convert.ToInt32(dtIden.Rows[i]["tipoOperacion"]);
                iden.tipoContingencia = null;
                iden.motivoContin = null;
                iden.fecEmi = Convert.ToString(dtIden.Rows[i]["fecEmi"]).Trim();
                iden.horEmi = Convert.ToString(dtIden.Rows[i]["horEmi"]).Trim();
                iden.tipoMoneda = Convert.ToString(dtIden.Rows[i]["tipoMoneda"]).Trim();

            }
            cxdb1.Close();


            identificacion.Add(iden);


            detalle_relacionado detalle_re = new detalle_relacionado();
            List<DocumentoRelacionado> documentorelacionado = new List<DocumentoRelacionado>();
            // documentorelacionado.Add(addDocumentosRelacionados("",2,"",""));

            if (documentorelacionado.Count <= 0)
            {
                detalle_re.documentoRelacionado = null;
            }
            else
            {
                detalle_re.documentoRelacionado = documentorelacionado;
            }


            dt_re.Add(detalle_re);





            //APARTADO PARA EMISOR
            Emisor emi = new Emisor();
            cxdb1.Open();

            string query = "exec GENERAR_DTE_FSEE " + docEntry;
            SqlDataAdapter adapter = new SqlDataAdapter(query, cxdb1);
            DataSet dataEmisor = new DataSet();
            adapter.Fill(dataEmisor, "Emisor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtEmisor = dataEmisor.Tables[1];


            List<direccion> direccion_emisor = new List<direccion>();

            for (int i = 0; i < dtEmisor.Rows.Count; i++)
            {

                emi.nit = Convert.ToString(dtEmisor.Rows[i]["nit"].ToString().Trim());
                emi.nrc = Convert.ToString(dtEmisor.Rows[i]["nrc"].ToString().Trim());
                emi.nombre = Convert.ToString(dtEmisor.Rows[i]["nombre"].ToString().Trim());
                emi.codActividad = Convert.ToString(dtEmisor.Rows[i]["codActividad"].ToString().Trim());
                emi.descActividad = Convert.ToString(dtEmisor.Rows[i]["descActividad"].ToString().Trim());
                emi.nombreComercial = Convert.ToString(dtEmisor.Rows[i]["nombreComercial"].ToString().Trim());
                emi.tipoEstablecimiento = Convert.ToString(dtEmisor.Rows[i]["tipoEstablecimiento"].ToString().Trim());
                direccion_emisor.Add(adddireccionemisor(Convert.ToString(dtEmisor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.municipo"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.complemento"].ToString().Trim())));
                if (Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    emi.telefono = Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString()).Trim();
                }
                else
                {
                    emi.telefono = "25252800";
                }


                if (Convert.ToString(dtEmisor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    emi.correo = Convert.ToString(dtEmisor.Rows[i]["correo"].ToString()).Trim();
                }
                else
                {
                    emi.correo = "contabilidad@motocity.com.sv";
                }
                emi.codEstableMH = Convert.ToString(dtEmisor.Rows[i]["codEstableMH"].ToString().Trim());
                emi.codEstable = Convert.ToString(dtEmisor.Rows[i]["codEstable"].ToString().Trim());
                emi.codPuntoVentaMH = Convert.ToString(dtEmisor.Rows[i]["codPuntoVentaMH"].ToString().Trim());
                emi.codPuntoVenta = Convert.ToString(dtEmisor.Rows[i]["codPuntoVenta"].ToString().Trim());

            }


            emi.direccion = direccion_emisor;

            emisor.Add(emi);

            cxdb1.Close();


            //APARTADO PARA RECEPTOR

            Receptor rec = new Receptor();
            List<direccion> direccion_receptor = new List<direccion>();

            cxdb1.Open();

            string queryReceptor = "exec GENERAR_DTE_FSEE " + docEntry;
            SqlDataAdapter adapterReceptor = new SqlDataAdapter(queryReceptor, cxdb1);
            DataSet dataReceptor = new DataSet();
            adapterReceptor.Fill(dataReceptor, "Receptor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtReceptor = dataReceptor.Tables[2];

            for (int i = 0; i < dtReceptor.Rows.Count; i++)
            {


                if (Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim()) != "")
                {
                    rec.tipoDocumento = Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim());
                }
                else
                {
                    rec.tipoDocumento = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim()) != "")
                {
                    rec.numDocumento = Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim());
                }
                else
                {
                    rec.numDocumento = null;
                }

              



                if (Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim()) != "")
                {
                    rec.nombre = Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim());
                }
                else
                {
                    rec.nombre = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim()) != "")
                {
                    rec.codActividad = Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim());
                }
                else
                {
                    rec.codActividad = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim()) != "")
                {
                    rec.descActividad = Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim());
                }
                else
                {
                    rec.descActividad = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()) != "")
                {
                    direccion_receptor.Add(adddireccionreceptor(Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.municipio"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.complemento"].ToString().Trim())));
                    rec.direccion = direccion_receptor;
                }
                else
                {
                    rec.direccion = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    rec.telefono = Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim());
                }
                else
                {
                    rec.telefono = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    rec.correo = Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim());
                }
                else
                {
                    rec.correo = null;
                }



            }




            receptor.Add(rec);

            cxdb1.Close();


            //APARTADO CUERPO DE DOCUMENTO

            detalle detalle = new detalle();
            List<CuerpoDocumento> cuerpo_documents = new List<CuerpoDocumento>();
            //   List<string> lista = Regex.Split("20", @"\s+").ToList();

            cxdb1.Open();

            string queryCuerpoDocumento = "exec GENERAR_DTE_FSEE " + docEntry;
            SqlDataAdapter adapterCuerpoDocumento = new SqlDataAdapter(queryCuerpoDocumento, cxdb1);
            DataSet dataCuerpoDocumento = new DataSet();
            adapterCuerpoDocumento.Fill(dataCuerpoDocumento, "CuerpoDocumento");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtCuerpoDocumento = dataCuerpoDocumento.Tables[3];


            for (int i = 0; i < dtCuerpoDocumento.Rows.Count; i++)
            {
                int numItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["numItem"].ToString());
                int tipoItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["tipoItem"].ToString());
                int uniMedida = 99;
                double cantidad = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["cantidad"].ToString());
                double precioUni = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["precioUni"].ToString());
                double montoDescu = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["montoDescu"].ToString());
                double ventaNoSuj = 0.0;
                double ventaExenta = 0.0;
                double ventaGravada = 0.0;
                double psv = 0.0;
                double noGravado = 0.0;
                double ivaItem = 0.0;
                string descripcion = Convert.ToString(dtCuerpoDocumento.Rows[i]["descripcion"].ToString());
                string codigo = Convert.ToString(dtCuerpoDocumento.Rows[i]["codigo"].ToString());
                double compra = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["compra"].ToString());

                cuerpo_documents.Add(adddetalle(numItem, tipoItem, null, codigo, null, descripcion.Replace("\\n", "\n"), cantidad, uniMedida, precioUni, montoDescu, ventaNoSuj, ventaExenta, ventaGravada, null, psv, noGravado, ivaItem,compra));
            }
            detalle.cuerpoDocumento = cuerpo_documents;
            detalles.Add(detalle);

            cxdb1.Close();
      
   
            //APARTADO RESUMEN

            Resumen res = new Resumen();
            List<Tributo> tributos = new List<Tributo>();
            cxdb1.Open();

            string queryResumen = "exec GENERAR_DTE_FSEE " + docEntry;
            SqlDataAdapter adapterResumen = new SqlDataAdapter(queryResumen, cxdb1);
            DataSet dataResumen = new DataSet();
            adapterResumen.Fill(dataResumen, "Resumen");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtResumen = dataResumen.Tables[4];


            for (int i = 0; i < dtResumen.Rows.Count; i++)
            {
                res.totalCompra = Convert.ToDouble(dtResumen.Rows[i]["totalCompra"].ToString());
                res.descu = Convert.ToDouble(dtResumen.Rows[i]["descu"].ToString());
                res.totalNoSuj = 0.00;
                res.totalExenta = 0.00;
                res.totalGravada = 0.00;
                res.subTotalVentas = 0.00;
                res.descuNoSuj = 0.00;
                res.descuExenta = 0.00;
                res.descuGravada = 0.00;
                res.porcentajeDescuento = 0.00;
                res.totalDescu = 0.00;
                res.totalIva = 0.00;
                res.tributos = null;
                res.subTotal = Convert.ToDouble(dtResumen.Rows[i]["subTotal"].ToString());
                res.ivaPerci1 = 0.00;
                res.ivaRete1 = Convert.ToDouble(dtResumen.Rows[i]["ivaRete1"].ToString());
                res.reteRenta = Convert.ToDouble(dtResumen.Rows[i]["reteRenta"].ToString());
                res.montoTotalOperacion = 0.00;
                res.totalNoGravado = 0.00;
                res.totalPagar = Convert.ToDouble(dtResumen.Rows[i]["totalPagar"].ToString());
                res.totalLetras = Convert.ToString(dtResumen.Rows[i]["totalLetras"].ToString());
                res.saldoFavor = 0.00;
                res.condicionOperacion = Convert.ToInt32(dtResumen.Rows[i]["condicionOperacion"].ToString());
                res.numPagoElectronico = null;

            }



            resumen.Add(res);
            cxdb1.Close();
            //APARTADO DETALLE EXTENSION

            detalle_extension de = new detalle_extension();
            List<Extension> exts = new List<Extension>();

            de.extension = null;

            detalle_extension.Add(de);


            //APARTADO DETALLE_APENDICE

            detalle_apendice da = new detalle_apendice();
            List<Apendice> apend = new List<Apendice>();

            cxdb1.Open();

            string queryApendice = "exec GENERAR_DTE_FSEE " + docEntry;
            SqlDataAdapter adapterApendice = new SqlDataAdapter(queryApendice, cxdb1);
            DataSet dataApendice = new DataSet();
            adapterApendice.Fill(dataApendice, "Apendice");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtApendice = dataApendice.Tables[5];


            for (int i = 0; i < dtApendice.Rows.Count; i++)
            {
                string campo = Convert.ToString(dtApendice.Rows[i]["campo"].ToString());
                string etiqueta = Convert.ToString(dtApendice.Rows[i]["etiqueta"].ToString());
                string valor = Convert.ToString(dtApendice.Rows[i]["valor"].ToString());

                apend.Add(addApendice(campo, etiqueta, valor));
            }

            if (apend.Count <= 0)
            {
                da.apendice = null;
            }
            else
            {
                da.apendice = apend;
            }

            cxdb1.Close();
            detalle_apendice.Add(da);




            string str_identificacion = JsonConvert.SerializeObject(iden, Newtonsoft.Json.Formatting.Indented);
            string str_emisor = JsonConvert.SerializeObject(emi, Newtonsoft.Json.Formatting.Indented);
            string str_receptor = JsonConvert.SerializeObject(rec, Newtonsoft.Json.Formatting.Indented);
            string str_cuerpodoc = JsonConvert.SerializeObject(detalles, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1 = str_cuerpodoc.Remove(str_cuerpodoc.Length - 5);
            string cadenaRecortada2 = cadenaRecortada1.Remove(0, 6);
            string str_resumen = JsonConvert.SerializeObject(resumen, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_resumen = str_resumen.Remove(str_resumen.Length - 1);
            string cadenaRecortada2_resumen = cadenaRecortada1_resumen.Remove(0, 1);

            string str_relacionado = JsonConvert.SerializeObject(dt_re, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_relacionado = str_relacionado.Remove(str_relacionado.Length - 5);
            string cadenaRecortada2_relacionado = cadenaRecortada1_relacionado.Remove(0, 6);


            string str_extension = JsonConvert.SerializeObject(detalle_extension, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_extension = str_extension.Remove(str_extension.Length - 5);
            string cadenaRecortada2_extension = cadenaRecortada1_extension.Remove(0, 6);


            string str_apendice = JsonConvert.SerializeObject(detalle_apendice, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_apendice = str_apendice.Remove(str_apendice.Length - 5);
            string cadenaRecortada2_apendice = cadenaRecortada1_apendice.Remove(0, 6);







            string var_identificacion = "\"identificacion\":" + str_identificacion;
            string var_emisor = "\"emisor\":" + str_emisor;
            string var_receptor = "\"sujetoExcluido\":" + str_receptor;
            string var_otrosDocumentos = "\"otrosDocumentos\":" + "null";
            string var_ventaTercero = "\"ventaTercero\":" + "null";
            string var_cuerpo = cadenaRecortada2.Trim();
            string var_resumen = "\"resumen\":" + cadenaRecortada2_resumen.Trim();
            string var_extension = cadenaRecortada2_extension.Trim();
            string var_apendice = cadenaRecortada2_apendice.Trim();

            string json_completo = "{\n" + var_identificacion + ",\n"  + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_receptor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_otrosDocumentos + ",\n" + var_ventaTercero + ",\n" + var_cuerpo + ",\n" + var_resumen + ",\n" + var_extension.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_apendice + "\n}";

            byte[] bytesUtf8 = Encoding.UTF8.GetBytes(json_completo);


            string textoDecodificado = Encoding.UTF8.GetString(bytesUtf8);



            System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", textoDecodificado.Trim());

            cxdb1.Open();

            string updateQuery = "UPDATE RICOH_PRINT SET impreso = @impreso  WHERE docEntry = @Condicion";

            using (SqlCommand command = new SqlCommand(updateQuery, cxdb1))
            {

                command.Parameters.AddWithValue("@impreso", 1);
                command.Parameters.AddWithValue("@condicion", docEntry);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("Registros actualizados: " + rowsAffected);
            }

            cxdb1.Close();


            identificacion.Clear();
            dt_re.Clear();
            emisor.Clear();
            receptor.Clear();
            detalles.Clear();
            detalle_extension.Clear();
            resumen.Clear();
            detalle_factura.Clear();
            detalle_apendice.Clear();






        }

        public void EX(string docEntry)
        {

            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);


            Identificacion iden = new Identificacion();

            cxdb1.Open();


            string queryIden = "exec GENERAR_DTE_FEXE " + docEntry;
            SqlDataAdapter adapterIden = new SqlDataAdapter(queryIden, cxdb1);
            DataSet dataIden = new DataSet();
            adapterIden.Fill(dataIden, "Identificacion");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtIden = dataIden.Tables[0];

            for (int i = 0; i < dtIden.Rows.Count; i++)
            {
                iden.version = Convert.ToInt32(dtIden.Rows[i]["version"]);
                iden.ambiente = Convert.ToString(dtIden.Rows[i]["ambiente"]).Trim();
                iden.tipoDte = Convert.ToString(dtIden.Rows[i]["tipoDte"]).Trim();
                iden.numeroControl = Convert.ToString(dtIden.Rows[i]["numeroControl"]).Trim();
                iden.codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();

                iden.tipoModelo = Convert.ToInt32(dtIden.Rows[i]["tipoModelo"]);
                iden.tipoOperacion = Convert.ToInt32(dtIden.Rows[i]["tipoOperacion"]);
                iden.tipoContingencia = null;
                iden.motivoContin = null;
                iden.fecEmi = Convert.ToString(dtIden.Rows[i]["fecEmi"]).Trim();
                iden.horEmi = Convert.ToString(dtIden.Rows[i]["horEmi"]).Trim();
                iden.tipoMoneda = Convert.ToString(dtIden.Rows[i]["tipoMoneda"]).Trim();

            }
            cxdb1.Close();


            identificacion.Add(iden);


            detalle_relacionado detalle_re = new detalle_relacionado();
            List<DocumentoRelacionado> documentorelacionado = new List<DocumentoRelacionado>();
            // documentorelacionado.Add(addDocumentosRelacionados("",2,"",""));

            if (documentorelacionado.Count <= 0)
            {
                detalle_re.documentoRelacionado = null;
            }
            else
            {
                detalle_re.documentoRelacionado = documentorelacionado;
            }


            dt_re.Add(detalle_re);





            //APARTADO PARA EMISOR
            Emisor emi = new Emisor();
            cxdb1.Open();

            string query = "exec GENERAR_DTE_FEXE " + docEntry;
            SqlDataAdapter adapter = new SqlDataAdapter(query, cxdb1);
            DataSet dataEmisor = new DataSet();
            adapter.Fill(dataEmisor, "Emisor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtEmisor = dataEmisor.Tables[1];


            List<direccion> direccion_emisor = new List<direccion>();

            for (int i = 0; i < dtEmisor.Rows.Count; i++)
            {

                emi.nit = Convert.ToString(dtEmisor.Rows[i]["nit"].ToString().Trim());
                emi.nrc = Convert.ToString(dtEmisor.Rows[i]["nrc"].ToString().Trim());
                emi.nombre = Convert.ToString(dtEmisor.Rows[i]["nombre"].ToString().Trim());
                emi.codActividad = Convert.ToString(dtEmisor.Rows[i]["codActividad"].ToString().Trim());
                emi.descActividad = Convert.ToString(dtEmisor.Rows[i]["descActividad"].ToString().Trim());
                emi.nombreComercial = Convert.ToString(dtEmisor.Rows[i]["nombreComercial"].ToString().Trim());
                emi.tipoEstablecimiento = Convert.ToString(dtEmisor.Rows[i]["tipoEstablecimiento"].ToString().Trim());
                direccion_emisor.Add(adddireccionemisor(Convert.ToString(dtEmisor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.municipo"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.complemento"].ToString().Trim())));
                if (Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    emi.telefono = Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString()).Trim();
                }
                else
                {
                    emi.telefono = "25252800";
                }


                if (Convert.ToString(dtEmisor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    emi.correo = Convert.ToString(dtEmisor.Rows[i]["correo"].ToString()).Trim();
                }
                else
                {
                    emi.correo = "contabilidad@motocity.com.sv";
                }
                emi.codEstableMH = Convert.ToString(dtEmisor.Rows[i]["codEstableMH"].ToString().Trim());
                emi.codEstable = Convert.ToString(dtEmisor.Rows[i]["codEstable"].ToString().Trim());
                emi.codPuntoVentaMH = Convert.ToString(dtEmisor.Rows[i]["codPuntoVentaMH"].ToString().Trim());
                emi.codPuntoVenta = Convert.ToString(dtEmisor.Rows[i]["codPuntoVenta"].ToString().Trim());
                emi.tipoItemExpor = Convert.ToInt32(dtEmisor.Rows[i]["tipoItemExpor"].ToString().Trim());
                emi.recintoFiscal = Convert.ToString(dtEmisor.Rows[i]["recintoFiscal"].ToString().Trim());
                emi.regimen = Convert.ToString(dtEmisor.Rows[i]["regimen"].ToString().Trim());

            }


            emi.direccion = direccion_emisor;

            emisor.Add(emi);

            cxdb1.Close();


            //APARTADO PARA RECEPTOR

            Receptor rec = new Receptor();
            List<direccion> direccion_receptor = new List<direccion>();

            cxdb1.Open();

            string queryReceptor = "exec GENERAR_DTE_FEXE " + docEntry;
            SqlDataAdapter adapterReceptor = new SqlDataAdapter(queryReceptor, cxdb1);
            DataSet dataReceptor = new DataSet();
            adapterReceptor.Fill(dataReceptor, "Receptor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtReceptor = dataReceptor.Tables[2];

            for (int i = 0; i < dtReceptor.Rows.Count; i++)
            {


                if (Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim()) != "")
                {
                    rec.tipoDocumento = Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim());
                }
                else
                {
                    rec.tipoDocumento = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim()) != "")
                {
                    rec.numDocumento = Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim());
                }
                else
                {
                    rec.numDocumento = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim()) != "")
                {
                    rec.nombre = Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim());
                }
                else
                {
                    rec.nombre = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim()) != "")
                {
                    rec.descActividad = Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim());
                }
                else
                {
                    rec.descActividad = null;
                }



                rec.direccion = null;
                

                if (Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    rec.correo = Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim());
                }
                else
                {
                    rec.correo = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["codPais"].ToString().Trim()) != "")
                {
                    rec.codPais = Convert.ToString(dtReceptor.Rows[i]["codPais"].ToString().Trim());
                }
                else
                {
                    rec.codPais = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["nombrePais"].ToString().Trim()) != "")
                {
                    rec.nombrePais = Convert.ToString(dtReceptor.Rows[i]["nombrePais"].ToString().Trim());
                }
                else
                {
                    rec.nombrePais = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["direccion.complemento"].ToString().Trim()) != "")
                {
                    rec.complemento = Convert.ToString(dtReceptor.Rows[i]["direccion.complemento"].ToString().Trim());
                }
                else
                {
                    rec.complemento = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["tipoPersona"].ToString().Trim()) != "")
                {
                    rec.tipoPersona = Convert.ToString(dtReceptor.Rows[i]["tipoPersona"].ToString().Trim());
                }
                else
                {
                    rec.tipoPersona = null;
                }

               
              

            }




            receptor.Add(rec);

            cxdb1.Close();


            //APARTADO CUERPO DE DOCUMENTO

            detalle detalle = new detalle();
            List<CuerpoDocumento> cuerpo_documents = new List<CuerpoDocumento>();
            //   List<string> lista = Regex.Split("20", @"\s+").ToList();

            cxdb1.Open();

            string queryCuerpoDocumento = "exec GENERAR_DTE_FEXE " + docEntry;
            SqlDataAdapter adapterCuerpoDocumento = new SqlDataAdapter(queryCuerpoDocumento, cxdb1);
            DataSet dataCuerpoDocumento = new DataSet();
            adapterCuerpoDocumento.Fill(dataCuerpoDocumento, "CuerpoDocumento");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtCuerpoDocumento = dataCuerpoDocumento.Tables[3];


            for (int i = 0; i < dtCuerpoDocumento.Rows.Count; i++)
            {
                int numItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["numItem"].ToString());
                int tipoItem = 0;
                int uniMedida = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["uniMedida"].ToString());
                double cantidad = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["cantidad"].ToString());
                double precioUni = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["precioUni"].ToString());
                double montoDescu = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["montoDescu"].ToString());
                double ventaNoSuj = 0;
                double ventaExenta = 0;
                double ventaGravada = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaGravada"].ToString());
                double psv = 0;
                double noGravado = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["noGravado"].ToString());
                double ivaItem = 0;
                string descripcion = Convert.ToString(dtCuerpoDocumento.Rows[i]["descripcion"].ToString());
                string codigo = Convert.ToString(dtCuerpoDocumento.Rows[i]["codigo"].ToString());
                double compra = 0.0;
                cuerpo_documents.Add(adddetalle(numItem, tipoItem, null, codigo, null, descripcion.Replace("\\n", "\n"), cantidad, uniMedida, precioUni, montoDescu, ventaNoSuj, ventaExenta, ventaGravada, null, psv, noGravado, ivaItem, compra));
            }
            detalle.cuerpoDocumento = cuerpo_documents;
            detalles.Add(detalle);

            cxdb1.Close();


            //APARTADO RESUMEN

            Resumen res = new Resumen();
            List<Tributo> tributos = new List<Tributo>();
            cxdb1.Open();

            string queryResumen = "exec GENERAR_DTE_FEXE " + docEntry;
            SqlDataAdapter adapterResumen = new SqlDataAdapter(queryResumen, cxdb1);
            DataSet dataResumen = new DataSet();
            adapterResumen.Fill(dataResumen, "Resumen");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtResumen = dataResumen.Tables[4];


            for (int i = 0; i < dtResumen.Rows.Count; i++)
            {
                res.totalNoSuj = 0;
                res.totalExenta = 0;
                res.totalGravada = Convert.ToDouble(dtResumen.Rows[i]["totalGravada"].ToString());
                res.subTotalVentas = 0;
                res.descuNoSuj = 0;
                res.descuExenta = 0;
                res.descuGravada = 0;
                res.porcentajeDescuento = Convert.ToDouble(dtResumen.Rows[i]["porcentajeDescuento"].ToString());
                res.totalDescu = Convert.ToDouble(dtResumen.Rows[i]["totalDescu"].ToString());
                res.totalIva = 0;
                res.tributos = null;
                res.subTotal = 0;
                res.ivaPerci1 = 0.00;
                res.ivaRete1 = 0;
                res.reteRenta = 0;
                res.montoTotalOperacion = Convert.ToDouble(dtResumen.Rows[i]["montoTotalOperacion"].ToString());
                res.totalNoGravado = Convert.ToDouble(dtResumen.Rows[i]["totalNoGravado"].ToString());
                res.totalPagar = Convert.ToDouble(dtResumen.Rows[i]["totalPagar"].ToString());
                res.totalLetras = Convert.ToString(dtResumen.Rows[i]["totalLetras"].ToString());
                res.saldoFavor = 0;
                res.condicionOperacion = Convert.ToInt32(dtResumen.Rows[i]["condicionOperacion"].ToString());
                res.numPagoElectronico = null;
                res.codIncoterms = Convert.ToString(dtResumen.Rows[i]["codIncoterms"].ToString());
                res.descIncoterms = Convert.ToString(dtResumen.Rows[i]["descIncoterms"].ToString());
                res.observaciones = null;
                res.flete = Convert.ToDouble(dtResumen.Rows[i]["flete"].ToString());
                res.seguro = Convert.ToDouble(dtResumen.Rows[i]["seguro"].ToString());





             
            }



            resumen.Add(res);
          
            detalle_extension de = new detalle_extension();
            List<Extension> exts = new List<Extension>();
               
           

            if (exts.Count <= 0)
            {
                de.extension = null;
            }
            else
            {
                de.extension = exts;
            }

            detalle_extension.Add(de);

            cxdb1.Close();




            //APARTADO DETALLE_APENDICE

            detalle_apendice da = new detalle_apendice();
            List<Apendice> apend = new List<Apendice>();

            cxdb1.Open();

            string queryApendice = "exec GENERAR_DTE_FEXE " + docEntry;
            SqlDataAdapter adapterApendice = new SqlDataAdapter(queryApendice, cxdb1);
            DataSet dataApendice = new DataSet();
            adapterApendice.Fill(dataApendice, "Apendice");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtApendice = dataApendice.Tables[5];


            for (int i = 0; i < dtApendice.Rows.Count; i++)
            {
                string campo = Convert.ToString(dtApendice.Rows[i]["campo"].ToString());
                string etiqueta = Convert.ToString(dtApendice.Rows[i]["etiqueta"].ToString());
                string valor = Convert.ToString(dtApendice.Rows[i]["valor"].ToString());

                apend.Add(addApendice(campo, etiqueta, valor));
            }

            if (apend.Count <= 0)
            {
                da.apendice = null;
            }
            else
            {
                da.apendice = apend;
            }

            cxdb1.Close();
            detalle_apendice.Add(da);




            string str_identificacion = JsonConvert.SerializeObject(iden, Newtonsoft.Json.Formatting.Indented);
            string str_emisor = JsonConvert.SerializeObject(emi, Newtonsoft.Json.Formatting.Indented);
            string str_receptor = JsonConvert.SerializeObject(rec, Newtonsoft.Json.Formatting.Indented);
            string str_cuerpodoc = JsonConvert.SerializeObject(detalles, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1 = str_cuerpodoc.Remove(str_cuerpodoc.Length - 5);
            string cadenaRecortada2 = cadenaRecortada1.Remove(0, 6);
            string str_resumen = JsonConvert.SerializeObject(resumen, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_resumen = str_resumen.Remove(str_resumen.Length - 1);
            string cadenaRecortada2_resumen = cadenaRecortada1_resumen.Remove(0, 1);

            string str_relacionado = JsonConvert.SerializeObject(dt_re, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_relacionado = str_relacionado.Remove(str_relacionado.Length - 5);
            string cadenaRecortada2_relacionado = cadenaRecortada1_relacionado.Remove(0, 6);


            string str_extension = JsonConvert.SerializeObject(detalle_extension, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_extension = str_extension.Remove(str_extension.Length - 5);
            string cadenaRecortada2_extension = cadenaRecortada1_extension.Remove(0, 6);


            string str_apendice = JsonConvert.SerializeObject(detalle_apendice, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_apendice = str_apendice.Remove(str_apendice.Length - 5);
            string cadenaRecortada2_apendice = cadenaRecortada1_apendice.Remove(0, 6);







            string var_identificacion = "\"identificacion\":" + str_identificacion;
            string var_emisor = "\"emisor\":" + str_emisor;
            string var_receptor = "\"receptor\":" + str_receptor;
            string var_otrosDocumentos = "\"otrosDocumentos\":" + "null";
            string var_ventaTercero = "\"ventaTercero\":" + "null";
            string var_cuerpo = cadenaRecortada2.Trim();
            string var_resumen = "\"resumen\":" + cadenaRecortada2_resumen.Trim();
            string var_extension = cadenaRecortada2_extension.Trim();
            string var_apendice = cadenaRecortada2_apendice.Trim();

            string json_completo = "{\n" + var_identificacion + ",\n" + cadenaRecortada2_relacionado.Trim() + ",\n" + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_receptor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_otrosDocumentos + ",\n" + var_ventaTercero + ",\n" + var_cuerpo + ",\n" + var_resumen + ",\n" + var_extension.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_apendice + "\n}";

            byte[] bytesUtf8 = Encoding.UTF8.GetBytes(json_completo);


            string textoDecodificado = Encoding.UTF8.GetString(bytesUtf8);



            System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", textoDecodificado.Trim());

            cxdb1.Open();

            string updateQuery = "UPDATE RICOH_PRINT SET impreso = @impreso  WHERE docEntry = @Condicion";

            using (SqlCommand command = new SqlCommand(updateQuery, cxdb1))
            {

                command.Parameters.AddWithValue("@impreso", 1);
                command.Parameters.AddWithValue("@condicion", docEntry);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("Registros actualizados: " + rowsAffected);
            }

            cxdb1.Close();


            identificacion.Clear();
            dt_re.Clear();
            emisor.Clear();
            receptor.Clear();
            detalles.Clear();
            detalle_extension.Clear();
            resumen.Clear();
            detalle_factura.Clear();
            detalle_apendice.Clear();







        }

        public void NCI(string docEntry)
        {
            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);

            //IDENTIFICACION------------------------------------

            Identificacion iden = new Identificacion();

            cxdb1.Open();


            string queryIden = "exec GENERAR_DTE_INVALID " + docEntry;
            SqlDataAdapter adapterIden = new SqlDataAdapter(queryIden, cxdb1);
            DataSet dataIden = new DataSet();
            adapterIden.Fill(dataIden, "Identificacion");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtIden = dataIden.Tables[0];

            for (int i = 0; i < dtIden.Rows.Count; i++)
            {
                iden.version = Convert.ToInt32(dtIden.Rows[i]["version"]);
                iden.ambiente = Convert.ToString(dtIden.Rows[i]["ambiente"]).Trim();
                iden.codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                iden.fecAnula = Convert.ToString(dtIden.Rows[i]["fecAnula"]).Trim();
                iden.horAnula = Convert.ToString(dtIden.Rows[i]["horAnula"]).Trim();


            }
            cxdb1.Close();
            identificacion.Add(iden);

            //EMISOR-------------------------------------------
            Emisor emi = new Emisor();
            cxdb1.Open();

            string query = "exec GENERAR_DTE_INVALID " + docEntry;
            SqlDataAdapter adapter = new SqlDataAdapter(query, cxdb1);
            DataSet dataEmisor = new DataSet();
            adapter.Fill(dataEmisor, "Emisor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtEmisor = dataEmisor.Tables[1];


            List<direccion> direccion_emisor = new List<direccion>();

            for (int i = 0; i < dtEmisor.Rows.Count; i++)
            {

                emi.nit = Convert.ToString(dtEmisor.Rows[i]["nit"].ToString().Trim());
                emi.nombre = Convert.ToString(dtEmisor.Rows[i]["nombre"].ToString().Trim());
                emi.nomEstablecimiento = "Sucursal / Agencia";
                emi.tipoEstablecimiento = Convert.ToString(dtEmisor.Rows[i]["tipoEstablecimiento"].ToString().Trim());
             
                if (Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    emi.telefono = Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString()).Trim();
                }
                else
                {
                    emi.telefono = "25252800";
                }


                if (Convert.ToString(dtEmisor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    emi.correo = Convert.ToString(dtEmisor.Rows[i]["correo"].ToString()).Trim();
                }
                else
                {
                    emi.correo = "contabilidad@motocity.com.sv";
                }
                emi.codEstableMH = Convert.ToString(dtEmisor.Rows[i]["codEstableMH"].ToString().Trim());
                emi.codEstable = Convert.ToString(dtEmisor.Rows[i]["codEstable"].ToString().Trim());
                emi.codPuntoVentaMH = Convert.ToString(dtEmisor.Rows[i]["codPuntoVentaMH"].ToString().Trim());
                emi.codPuntoVenta = Convert.ToString(dtEmisor.Rows[i]["codPuntoVenta"].ToString().Trim());
              
            }
            cxdb1.Close();


            emisor.Add(emi);


            //DOCUMENTO-------------------------------------------
            Documento docu = new Documento();
            cxdb1.Open();

            string queryDocumento = "exec GENERAR_DTE_INVALID " + docEntry;
            SqlDataAdapter adapterDocumento = new SqlDataAdapter(queryDocumento, cxdb1);
            DataSet dataDocumento = new DataSet();
            adapterDocumento.Fill(dataDocumento, "Documento");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtDocumento = dataDocumento.Tables[2];

            for (int i = 0; i < dtDocumento.Rows.Count; i++)
            {
                docu.codigoGeneracion = Convert.ToString(dtDocumento.Rows[i]["codigoGeneracion"].ToString().Trim());
                docu.codigoGeneracionR = Convert.ToString(dtDocumento.Rows[i]["codigoGeneracionR"].ToString().Trim());
                docu.correo = Convert.ToString(dtDocumento.Rows[i]["correo"].ToString().Trim());
                docu.fecEmi = Convert.ToString(dtDocumento.Rows[i]["fecEmi"].ToString().Trim());
                docu.montoIva = Convert.ToDouble(dtDocumento.Rows[i]["montoIva"].ToString().Trim());
                docu.nombre = Convert.ToString(dtDocumento.Rows[i]["nombre"].ToString().Trim());
                docu.numDocumento = Convert.ToString(dtDocumento.Rows[i]["numDocumento"].ToString().Trim());
                docu.numeroControl = Convert.ToString(dtDocumento.Rows[i]["numeroControl"].ToString().Trim());
                docu.selloRecibido = Convert.ToString(dtDocumento.Rows[i]["selloRecibido"].ToString().Trim());
                docu.telefono = Convert.ToString(dtDocumento.Rows[i]["telefono"].ToString().Trim());
                docu.tipoDocumento = Convert.ToString(dtDocumento.Rows[i]["tipoDocumento"].ToString().Trim());
                docu.tipoDte = Convert.ToString(dtDocumento.Rows[i]["tipoDte"].ToString().Trim());
            }

            cxdb1.Close();

            documento.Add(docu);


            //MOTIVO-------------------------------------------

            Motivo moti = new Motivo();
            cxdb1.Open();

            string queryMotivo = "exec GENERAR_DTE_INVALID " + docEntry;
            SqlDataAdapter adapterMotivo = new SqlDataAdapter(queryMotivo, cxdb1);
            DataSet dataMotivo = new DataSet();
            adapterMotivo.Fill(dataMotivo, "Motivo");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtMotivo = dataMotivo.Tables[3];

            for (int i = 0; i < dtMotivo.Rows.Count; i++)
            {

                moti.motivoAnulacion = Convert.ToString(dtMotivo.Rows[i]["motivoAnulacion"].ToString().Trim());
                moti.nombreResponsable = Convert.ToString(dtMotivo.Rows[i]["nombreResponsable"].ToString().Trim());
                moti.nombreSolicita = Convert.ToString(dtMotivo.Rows[i]["nombreSolicita"].ToString().Trim());
                moti.numDocResponsable = Convert.ToString(dtMotivo.Rows[i]["numDocResponsable"].ToString().Trim());
                moti.numDocSolicita = Convert.ToString(dtMotivo.Rows[i]["numDocSolicita"].ToString().Trim());
                moti.tipDocResponsable = Convert.ToString(dtMotivo.Rows[i]["tipoDocResposable"].ToString().Trim());
                moti.tipDocSolicita = Convert.ToString(dtMotivo.Rows[i]["tipoDocSolicita"].ToString().Trim());
                moti.tipoAnulacion = Convert.ToString(dtMotivo.Rows[i]["tipoAnulacion"].ToString().Trim());

            }
            cxdb1.Close();
            motivo.Add(moti);


            string str_identificacion = JsonConvert.SerializeObject(iden, Formatting.Indented);
            string str_emisor = JsonConvert.SerializeObject(emi, Formatting.Indented);
            string str_documento = JsonConvert.SerializeObject(docu, Formatting.Indented);
            string str_motivo = JsonConvert.SerializeObject(moti, Formatting.Indented);
            string var_identificacion = "\"identificacion\":" + str_identificacion;
            string var_emisor = "\"emisor\":" + str_emisor;
            string var_documento = "\"documento\":" + str_documento;
            string var_motivo = "\"motivo\":" + str_motivo;


            string json_completo = "{\n" + var_identificacion + ",\n" + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_documento.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_motivo.Replace("[", "").Trim().Replace("]", "").Trim() + "\n}";
            byte[] bytesUtf8 = Encoding.UTF8.GetBytes(json_completo);
            string textoDecodificado = Encoding.UTF8.GetString(bytesUtf8);


            System.IO.File.WriteAllText("C:\\invalidacion\\" + codigoGeneracion + ".json", textoDecodificado.Trim());

            cxdb1.Open();

            string updateQuery = "UPDATE RICOH_PRINT SET impreso = @impreso  WHERE docEntry = @Condicion";

            using (SqlCommand command = new SqlCommand(updateQuery, cxdb1))
            {

                command.Parameters.AddWithValue("@impreso", 1);
                command.Parameters.AddWithValue("@condicion", docEntry);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("Registros actualizados: " + rowsAffected);
            }

            cxdb1.Close();
        }

        public void NB(string docEntry)
        {

            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);


            Identificacion iden = new Identificacion();

            cxdb1.Open();


            string queryIden = "exec GENERAR_DTE_NDE " + docEntry;
            SqlDataAdapter adapterIden = new SqlDataAdapter(queryIden, cxdb1);
            DataSet dataIden = new DataSet();
            adapterIden.Fill(dataIden, "Identificacion");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtIden = dataIden.Tables[0];

            for (int i = 0; i < dtIden.Rows.Count; i++)
            {
                iden.version = Convert.ToInt32(dtIden.Rows[i]["version"]);
                iden.ambiente = Convert.ToString(dtIden.Rows[i]["ambiente"]).Trim();
                iden.tipoDte = Convert.ToString(dtIden.Rows[i]["tipoDte"]).Trim();
                iden.numeroControl = Convert.ToString(dtIden.Rows[i]["numeroControl"]).Trim();
                iden.codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();
                codigoGeneracion = Convert.ToString(dtIden.Rows[i]["codigoGeneracion"]).ToUpper().Trim();

                iden.tipoModelo = Convert.ToInt32(dtIden.Rows[i]["tipoModelo"]);
                iden.tipoOperacion = Convert.ToInt32(dtIden.Rows[i]["tipoOperacion"]);
                iden.tipoContingencia = null;
                iden.motivoContin = null;
                iden.fecEmi = Convert.ToString(dtIden.Rows[i]["fecEmi"]).Trim();
                iden.horEmi = Convert.ToString(dtIden.Rows[i]["horEmi"]).Trim();
                iden.tipoMoneda = Convert.ToString(dtIden.Rows[i]["tipoMoneda"]).Trim();

            }
            cxdb1.Close();


            identificacion.Add(iden);


            detalle_relacionado detalle_re = new detalle_relacionado();
            List<DocumentoRelacionado> documentorelacionado = new List<DocumentoRelacionado>();


            cxdb1.Open();

            string queryDocumentoRelacionado = "exec GENERAR_DTE_NDE " + docEntry;
            SqlDataAdapter adapterDocumentoRelacionado = new SqlDataAdapter(queryDocumentoRelacionado, cxdb1);
            DataSet dataDocumentoRelacionado = new DataSet();
            adapterDocumentoRelacionado.Fill(dataDocumentoRelacionado, "Documento Relacionado");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtDocumentoRelacionado = dataDocumentoRelacionado.Tables[1];


            for (int i = 0; i < dtDocumentoRelacionado.Rows.Count; i++)
            {
                string tipoDocumento = Convert.ToString(dtDocumentoRelacionado.Rows[i]["tipoDocumento"].ToString().Trim());
                int tipoGeneracion = Convert.ToInt32(dtDocumentoRelacionado.Rows[i]["tipoGeneracion"].ToString().Trim());
                string numeroDocumento = Convert.ToString(dtDocumentoRelacionado.Rows[i]["numeroDocumento"].ToString().Trim());
                string fechaEmision = Convert.ToString(dtDocumentoRelacionado.Rows[i]["fechaEmision"].ToString().Trim());


                documentorelacionado.Add(addDocumentosRelacionados(tipoDocumento, tipoGeneracion, numeroDocumento, fechaEmision));
            }





            if (documentorelacionado.Count <= 0)
            {
                detalle_re.documentoRelacionado = null;
            }
            else
            {
                detalle_re.documentoRelacionado = documentorelacionado;
            }


            dt_re.Add(detalle_re);


            cxdb1.Close();




            //APARTADO PARA EMISOR
            Emisor emi = new Emisor();
            cxdb1.Open();

            string query = "exec GENERAR_DTE_NDE " + docEntry;
            SqlDataAdapter adapter = new SqlDataAdapter(query, cxdb1);
            DataSet dataEmisor = new DataSet();
            adapter.Fill(dataEmisor, "Emisor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtEmisor = dataEmisor.Tables[2];


            List<direccion> direccion_emisor = new List<direccion>();

            for (int i = 0; i < dtEmisor.Rows.Count; i++)
            {

                emi.nit = Convert.ToString(dtEmisor.Rows[i]["nit"].ToString().Trim());
                emi.nrc = Convert.ToString(dtEmisor.Rows[i]["nrc"].ToString().Trim());
                emi.nombre = Convert.ToString(dtEmisor.Rows[i]["nombre"].ToString().Trim());
                emi.codActividad = Convert.ToString(dtEmisor.Rows[i]["codActividad"].ToString().Trim());
                emi.descActividad = Convert.ToString(dtEmisor.Rows[i]["descActividad"].ToString().Trim());
                emi.nombreComercial = Convert.ToString(dtEmisor.Rows[i]["nombreComercial"].ToString().Trim());
                emi.tipoEstablecimiento = Convert.ToString(dtEmisor.Rows[i]["tipoEstablecimiento"].ToString().Trim());
                direccion_emisor.Add(adddireccionemisor(Convert.ToString(dtEmisor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.municipo"].ToString().Trim()), Convert.ToString(dtEmisor.Rows[i]["direccion.complemento"].ToString().Trim())));
                if (Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    emi.telefono = Convert.ToString(dtEmisor.Rows[i]["telefono"].ToString()).Trim();
                }
                else
                {
                    emi.telefono = "25252800";
                }


                if (Convert.ToString(dtEmisor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    emi.correo = Convert.ToString(dtEmisor.Rows[i]["correo"].ToString()).Trim();
                }
                else
                {
                    emi.correo = "contabilidad@motocity.com.sv";
                }
                emi.codEstableMH = Convert.ToString(dtEmisor.Rows[i]["codEstableMH"].ToString().Trim());
                emi.codEstable = Convert.ToString(dtEmisor.Rows[i]["codEstable"].ToString().Trim());
                emi.codPuntoVentaMH = Convert.ToString(dtEmisor.Rows[i]["codPuntoVentaMH"].ToString().Trim());
                emi.codPuntoVenta = Convert.ToString(dtEmisor.Rows[i]["codPuntoVenta"].ToString().Trim());

            }


            emi.direccion = direccion_emisor;

            emisor.Add(emi);

            cxdb1.Close();


            //APARTADO PARA RECEPTOR

            Receptor rec = new Receptor();
            List<direccion> direccion_receptor = new List<direccion>();

            cxdb1.Open();

            string queryReceptor = "exec GENERAR_DTE_NDE " + docEntry;
            SqlDataAdapter adapterReceptor = new SqlDataAdapter(queryReceptor, cxdb1);
            DataSet dataReceptor = new DataSet();
            adapterReceptor.Fill(dataReceptor, "Receptor");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtReceptor = dataReceptor.Tables[3];

            for (int i = 0; i < dtReceptor.Rows.Count; i++)
            {


                if (Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim()) != "")
                {
                    rec.tipoDocumento = Convert.ToString(dtReceptor.Rows[i]["tipoDocumento"].ToString().Trim());
                }
                else
                {
                    rec.tipoDocumento = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim()) != "")
                {
                    rec.numDocumento = Convert.ToString(dtReceptor.Rows[i]["numDocumento"].ToString().Trim());
                }
                else
                {
                    rec.numDocumento = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim()) != "")
                {
                    rec.nit = Convert.ToString(dtReceptor.Rows[i]["nit"].ToString().Trim());
                }
                else
                {
                    rec.nit = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim()) != "")
                {
                    rec.nrc = Convert.ToString(dtReceptor.Rows[i]["nrc"].ToString().Trim());
                }
                else
                {
                    rec.nrc = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim()) != "")
                {
                    rec.nombre = Convert.ToString(dtReceptor.Rows[i]["nombre"].ToString().Trim());
                }
                else
                {
                    rec.nombre = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim()) != "")
                {
                    rec.codActividad = Convert.ToString(dtReceptor.Rows[i]["codActividad"].ToString().Trim());
                }
                else
                {
                    rec.codActividad = null;
                }

                if (Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim()) != "")
                {
                    rec.descActividad = Convert.ToString(dtReceptor.Rows[i]["descActividad"].ToString().Trim());
                }
                else
                {
                    rec.descActividad = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["nombreComercial"].ToString().Trim()) != "")
                {
                    rec.nombreComercial = Convert.ToString(dtReceptor.Rows[i]["nombreComercial"].ToString().Trim());
                }
                else
                {
                    rec.nombreComercial = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()) != "")
                {
                    direccion_receptor.Add(adddireccionreceptor(Convert.ToString(dtReceptor.Rows[i]["direccion.departamento"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.municipio"].ToString().Trim()), Convert.ToString(dtReceptor.Rows[i]["direccion.complemento"].ToString().Trim())));
                    rec.direccion = direccion_receptor;
                }
                else
                {
                    rec.direccion = null;
                }



                if (Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim()) != "")
                {
                    rec.telefono = Convert.ToString(dtReceptor.Rows[i]["telefono"].ToString().Trim());
                }
                else
                {
                    rec.telefono = null;
                }


                if (Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim()) != "")
                {
                    rec.correo = Convert.ToString(dtReceptor.Rows[i]["correo"].ToString().Trim());
                }
                else
                {
                    rec.correo = null;
                }



            }




            receptor.Add(rec);

            cxdb1.Close();


            //APARTADO CUERPO DE DOCUMENTO

            detalle detalle = new detalle();
            List<CuerpoDocumento> cuerpo_documents = new List<CuerpoDocumento>();
            List<string> lista = Regex.Split("20", @"\s+").ToList();

            cxdb1.Open();

            string queryCuerpoDocumento = "exec GENERAR_DTE_NDE " + docEntry;
            SqlDataAdapter adapterCuerpoDocumento = new SqlDataAdapter(queryCuerpoDocumento, cxdb1);
            DataSet dataCuerpoDocumento = new DataSet();
            adapterCuerpoDocumento.Fill(dataCuerpoDocumento, "CuerpoDocumento");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtCuerpoDocumento = dataCuerpoDocumento.Tables[4];


            for (int i = 0; i < dtCuerpoDocumento.Rows.Count; i++)
            {
                int numItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["numItem"].ToString());
                int tipoItem = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["tipoItem"].ToString());
                int uniMedida = Convert.ToInt32(dtCuerpoDocumento.Rows[i]["uniMedida"].ToString());
                double cantidad = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["cantidad"].ToString());
                double precioUni = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["precioUni"].ToString());
                double montoDescu = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["montoDescu"].ToString());
                double ventaNoSuj = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaNoSuj"].ToString());
                double ventaExenta = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaExenta"].ToString());
                double ventaGravada = Convert.ToDouble(dtCuerpoDocumento.Rows[i]["ventaGravada"].ToString());
                double psv = 0.0;
                double noGravado = 0.0;
                double ivaItem = 0.00;
                string descripcion = Convert.ToString(dtCuerpoDocumento.Rows[i]["descripcion"].ToString());
                string numeroDocumento = Convert.ToString(dtCuerpoDocumento.Rows[i]["numeroDocumento"].ToString());
                string codigo = Convert.ToString(dtCuerpoDocumento.Rows[i]["codigo"].ToString());
                double compra = 0.0;
                cuerpo_documents.Add(adddetalle(numItem, tipoItem, numeroDocumento, codigo, null, descripcion.Replace("\\n", "\n"), cantidad, uniMedida, precioUni, montoDescu, ventaNoSuj, ventaExenta, ventaGravada, lista, psv, noGravado, ivaItem, compra));
            }
            detalle.cuerpoDocumento = cuerpo_documents;
            detalles.Add(detalle);

            cxdb1.Close();


            //APARTADO RESUMEN

            Resumen res = new Resumen();
            List<Tributo> tributos = new List<Tributo>();
            cxdb1.Open();

            string queryResumen = "exec GENERAR_DTE_NDE " + docEntry;
            SqlDataAdapter adapterResumen = new SqlDataAdapter(queryResumen, cxdb1);
            DataSet dataResumen = new DataSet();
            adapterResumen.Fill(dataResumen, "Resumen");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtResumen = dataResumen.Tables[5];


            for (int i = 0; i < dtResumen.Rows.Count; i++)
            {
                res.totalNoSuj = Convert.ToDouble(dtResumen.Rows[i]["totalNoSuj"].ToString());
                res.totalExenta = Convert.ToDouble(dtResumen.Rows[i]["totalExenta"].ToString());
                res.totalGravada = Convert.ToDouble(dtResumen.Rows[i]["totalGravada"].ToString());
                res.subTotalVentas = Convert.ToDouble(dtResumen.Rows[i]["subTotalVentas"].ToString());
                res.descuNoSuj = Convert.ToDouble(dtResumen.Rows[i]["descuNoSuj"].ToString());
                res.descuExenta = Convert.ToDouble(dtResumen.Rows[i]["descuExenta"].ToString());
                res.descuGravada = Convert.ToDouble(dtResumen.Rows[i]["descuGravada"].ToString());
                res.porcentajeDescuento = 0.0;
                res.totalDescu = Convert.ToDouble(dtResumen.Rows[i]["totalDescu"].ToString());
                res.totalIva = 0.00;
                tributos.Add(addtributos("20", "Impuesto al Valor Agregado 13%", Convert.ToDouble(dtResumen.Rows[i]["tributo.valor"])));
                res.subTotal = Convert.ToDouble(dtResumen.Rows[i]["subTotal"].ToString());
                res.ivaPerci1 = 0.00;
                res.ivaRete1 = Convert.ToDouble(dtResumen.Rows[i]["ivaRete1"].ToString());
                res.reteRenta = Convert.ToDouble(dtResumen.Rows[i]["reteRenta"].ToString());
                res.montoTotalOperacion = Convert.ToDouble(dtResumen.Rows[i]["montoTotalOperacion"].ToString());
                res.totalNoGravado = 0.00;
                res.totalPagar = 0.00;
                res.totalLetras = Convert.ToString(dtResumen.Rows[i]["totalLetras"].ToString());
                res.saldoFavor = 0.00;
                res.condicionOperacion = Convert.ToInt32(dtResumen.Rows[i]["condicionOperacion"].ToString());
                res.numPagoElectronico = null;

                res.tributos = tributos;

            }



            resumen.Add(res);
            cxdb1.Close();
            //APARTADO DETALLE EXTENSION

            detalle_extension de = new detalle_extension();
            List<Extension> exts = new List<Extension>();

            cxdb1.Open();

            string queryExtension = "exec GENERAR_DTE_NDE " + docEntry;
            SqlDataAdapter adapterExtension = new SqlDataAdapter(queryExtension, cxdb1);
            DataSet dataExtension = new DataSet();
            adapterExtension.Fill(dataExtension, "Extension");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtExtension = dataExtension.Tables[6];


            for (int i = 0; i < dtExtension.Rows.Count; i++)
            {
                string nombEntrega = Convert.ToString(dtExtension.Rows[i]["nombEntrega"].ToString());
                string docuEntrega = Convert.ToString(dtExtension.Rows[i]["docuEntrega"].ToString());
                string codEmpleado = Convert.ToString(dtExtension.Rows[i]["codEmpleado"].ToString());
                string nombRecibe = Convert.ToString(dtExtension.Rows[i]["nombRecibe"].ToString());
                string docuRecibe = Convert.ToString(dtExtension.Rows[i]["docuRecibe"].ToString());
                string placaVehiculo = Convert.ToString(dtExtension.Rows[i]["placaVehiculo"].ToString());
                string observaciones = Convert.ToString(dtExtension.Rows[i]["observaciones"].ToString());
                if (nombRecibe.ToString().Trim() != "" && docuRecibe.ToString().Trim() != "" && nombEntrega.ToString().Trim() != "" && docuEntrega.ToString().Trim() != "")
                {
                    exts.Add(addExtension(nombEntrega, docuEntrega, nombRecibe, docuRecibe, observaciones, placaVehiculo));
                }
                else
                {

                }

            }

            if (exts.Count <= 0)
            {
                de.extension = null;
            }
            else
            {
                de.extension = exts;
            }

            detalle_extension.Add(de);

            cxdb1.Close();
            //APARTADO DETALLE_APENDICE

            detalle_apendice da = new detalle_apendice();
            List<Apendice> apend = new List<Apendice>();

            cxdb1.Open();

            string queryApendice = "exec GENERAR_DTE_NDE " + docEntry;
            SqlDataAdapter adapterApendice = new SqlDataAdapter(queryApendice, cxdb1);
            DataSet dataApendice = new DataSet();
            adapterApendice.Fill(dataApendice, "Apendice");

            //Guardamos en DataTables las diferentes tablas que regresa el Procedimiento Almacenado
            DataTable dtApendice = dataApendice.Tables[7];


            for (int i = 0; i < dtApendice.Rows.Count; i++)
            {
                string campo= Convert.ToString(dtApendice.Rows[i]["campo"].ToString());
                string etiqueta= Convert.ToString(dtApendice.Rows[i]["etiqueta"].ToString());
                string valor= Convert.ToString(dtApendice.Rows[i]["valor"].ToString());

                apend.Add(addApendice(campo, etiqueta, valor));
            }

            if (apend.Count <= 0)
            {
                da.apendice = null;
            }
            else
            {
                da.apendice = apend;
            }

            cxdb1.Close();
            detalle_apendice.Add(da);




            string str_identificacion = JsonConvert.SerializeObject(iden, Newtonsoft.Json.Formatting.Indented);
            string str_emisor = JsonConvert.SerializeObject(emi, Newtonsoft.Json.Formatting.Indented);
            string str_receptor = JsonConvert.SerializeObject(rec, Newtonsoft.Json.Formatting.Indented);
            string str_cuerpodoc = JsonConvert.SerializeObject(detalles, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1 = str_cuerpodoc.Remove(str_cuerpodoc.Length - 5);
            string cadenaRecortada2 = cadenaRecortada1.Remove(0, 6);
            string str_resumen = JsonConvert.SerializeObject(resumen, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_resumen = str_resumen.Remove(str_resumen.Length - 1);
            string cadenaRecortada2_resumen = cadenaRecortada1_resumen.Remove(0, 1);

            string str_relacionado = JsonConvert.SerializeObject(dt_re, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_relacionado = str_relacionado.Remove(str_relacionado.Length - 5);
            string cadenaRecortada2_relacionado = cadenaRecortada1_relacionado.Remove(0, 6);


            string str_extension = JsonConvert.SerializeObject(detalle_extension, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_extension = str_extension.Remove(str_extension.Length - 5);
            string cadenaRecortada2_extension = cadenaRecortada1_extension.Remove(0, 6);


            string str_apendice = JsonConvert.SerializeObject(detalle_apendice, Newtonsoft.Json.Formatting.Indented);
            string cadenaRecortada1_apendice = str_apendice.Remove(str_apendice.Length - 5);
            string cadenaRecortada2_apendice = cadenaRecortada1_apendice.Remove(0, 6);







            string var_identificacion = "\"identificacion\":" + str_identificacion;
            string var_emisor = "\"emisor\":" + str_emisor;
            string var_receptor = "\"receptor\":" + str_receptor;
            string var_otrosDocumentos = "\"otrosDocumentos\":" + "null";
            string var_ventaTercero = "\"ventaTercero\":" + "null";
            string var_cuerpo = cadenaRecortada2.Trim();
            string var_resumen = "\"resumen\":" + cadenaRecortada2_resumen.Trim();
            string var_extension = cadenaRecortada2_extension.Trim();
            string var_apendice = cadenaRecortada2_apendice.Trim();

            string json_completo = "{\n" + var_identificacion + ",\n" + cadenaRecortada2_relacionado.Trim() + ",\n" + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_receptor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_otrosDocumentos + ",\n" + var_ventaTercero + ",\n" + var_cuerpo + ",\n" + var_resumen + ",\n" + var_extension.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_apendice + "\n}";

            byte[] bytesUtf8 = Encoding.UTF8.GetBytes(json_completo);


            string textoDecodificado = Encoding.UTF8.GetString(bytesUtf8);



            System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", textoDecodificado.Trim());

            cxdb1.Open();

            string updateQuery = "UPDATE RICOH_PRINT SET impreso = @impreso  WHERE docEntry = @Condicion";

            using (SqlCommand command = new SqlCommand(updateQuery, cxdb1))
            {

                command.Parameters.AddWithValue("@impreso", 1);
                command.Parameters.AddWithValue("@condicion", docEntry);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("Registros actualizados: " + rowsAffected);
            }

            cxdb1.Close();


            identificacion.Clear();
            dt_re.Clear();
            emisor.Clear();
            receptor.Clear();
            detalles.Clear();
            detalle_extension.Clear();
            resumen.Clear();
            detalle_factura.Clear();
            detalle_apendice.Clear();
        }





        private direccion adddireccionemisor(string departamento, string municipio, string complemento)
        {
            direccion direccion = new direccion();
            direccion.departamento = departamento.ToString().Trim();
            direccion.municipio = municipio.ToString().Trim();
            direccion.complemento = complemento.ToString().Trim();


            return direccion;
        }

        private direccion adddireccionreceptor(string departamento, string municipio, string complemento)
        {
            direccion direccion = new direccion();
            direccion.departamento = departamento.ToString().Trim();
            direccion.municipio = municipio.ToString().Trim();
            direccion.complemento = complemento.ToString().Trim();


            return direccion;
        }

        private CuerpoDocumento adddetalle(int numItem, int tipoItem, string numeroDocumento, string codigo, string codTributo, string descripcion, double cantidad, int uniMedida, double precioUni, double montoDescu, double ventaNoSuj, double ventaExenta, double ventaGravada, List<string> tributos, double psv, double noGravado, double ivaItem,double compra)
        {
            CuerpoDocumento detalle = new CuerpoDocumento();

            detalle.numItem = numItem;
            detalle.tipoItem = tipoItem;
            detalle.numeroDocumento = numeroDocumento;
            detalle.codigo = codigo;
            detalle.codTributo = codTributo;
            detalle.descripcion = descripcion;
            detalle.cantidad = cantidad;
            detalle.uniMedida = uniMedida;
            detalle.precioUni = precioUni;
            detalle.tributos = tributos;
            detalle.ventaGravada = ventaGravada;
            detalle.montoDescu = montoDescu;
            detalle.ivaItem = ivaItem;
            detalle.noGravado = noGravado;
            detalle.ventaExenta = ventaExenta;
            detalle.ventaNoSuj = ventaNoSuj;
            detalle.compra = compra;

            return detalle;
        }

        private Tributo addtributos(string codigo, string descripcion, double valor)
        {
            Tributo tr = new Tributo();
            tr.codigo = codigo.ToString().Trim();
            tr.descripcion = descripcion.ToString().Trim();
            tr.valor = valor;

            return tr;
        }

        private DocumentoRelacionado addDocumentosRelacionados(string tipoDocumento, int tipoGeneracion, string numeroDocumento, string fechaEmision)
        {
            DocumentoRelacionado dr = new DocumentoRelacionado();
            dr.tipoDocumento = tipoDocumento.ToString().Trim();
            dr.tipoGeneracion = tipoGeneracion;
            dr.numeroDocumento = numeroDocumento.ToString().Trim();
            dr.fechaEmision = fechaEmision.ToString().Trim();
            return dr;
        }

        private Extension addExtension(string nombEntrega, string docuEntrega, string nombRecibe, string docuRecibe, string observaciones, string placaVehiculo)
        {
            Extension extension = new Extension();
            extension.nombEntrega = nombEntrega;
            extension.docuEntrega = docuEntrega;
            extension.docuRecibe = docuRecibe;
            extension.nombRecibe = nombRecibe;
            extension.observaciones = observaciones;
            extension.placaVehiculo = placaVehiculo;


            return extension;
        }

        private Apendice addApendice(string campo, string etiqueta, string valor)
        {
            Apendice apendice = new Apendice();

            apendice.campo = campo;
            apendice.etiqueta = etiqueta;
            apendice.valor = valor;


            return apendice;
        }

    }
}
