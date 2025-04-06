using System.Collections.Generic;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using FUNSAR.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models.ViewModels;
using FUNSAR.Utilidades;
using NPOI.SS.Util;
using System.Collections;
using NPOI.SS.Formula.Functions;
using System.Web;
using System.Reflection.Metadata.Ecma335;

namespace HerramientasComunes
{
    public class Excel
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        public Excel(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public double ProcesaExcel(string rutaExcel, CertificadoVM temp, string conection)
        {
            double cantCert = 0;

            try
            {
                OperacionCertificado parametros = new OperacionCertificado();
                int confirm = 0;
                int[] param;
                param = new int[3];
                IWorkbook MiExcel = null;
                FileStream fs = new FileStream(rutaExcel, FileMode.Open, FileAccess.Read);

                if (Path.GetExtension(rutaExcel) == ".xlsx")
                    MiExcel = new XSSFWorkbook(fs);
                else
                    MiExcel = new HSSFWorkbook(fs);


                ISheet hoja = MiExcel.GetSheetAt(0);

                if (hoja != null)
                {

                    int cantidadfilas = hoja.LastRowNum;

                    for (int i = 1; i <= cantidadfilas; i++)
                    {
                        IRow fila = hoja.GetRow(i);

                        if (fila != null)
                            confirm = parametros.DatosCertificado(fila.GetCell(2, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(2, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "", fila.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "", conection);
                            if (confirm == 0)
                            {
                                param = parametros.Parametros(fila.GetCell(2, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(2, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "", fila.GetCell(4, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(4, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "", fila.GetCell(6, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(6, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "", fila.GetCell(7, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(7, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "",conection);
                                temp.CertificadoTemp.Nombre = fila.GetCell(0, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(0, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "";
                                temp.CertificadoTemp.Apellido = fila.GetCell(1, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(1, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "";
                                temp.CertificadoTemp.DocumentoId = param[1];
                                temp.CertificadoTemp.Documento = fila.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "";
                                temp.CertificadoTemp.BrigadaId = param[0];
                                temp.CertificadoTemp.AnoProceso = fila.GetCell(5, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(5, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "";
                                temp.CertificadoTemp.SemestreId = param[2];
                                temp.CertificadoTemp.ProcesoId = param[3];
                                temp.CertificadoTemp.EstadoId = 5;  //fila.GetCell(7, MissingCellPolicy.RETURN_NULL_AND_BLANK) != null ? fila.GetCell(7, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString() : "";
                                temp.CertificadoTemp.FechaExpedicion = DateTime.Now.ToString("yyyy/MM/dd");
                                temp.CertificadoTemp.codCertificado = Guid.NewGuid().ToString();
                                temp.CertificadoTemp.Id = 0;
                                _contenedorTrabajo.CertificadoTemp.Add(temp.CertificadoTemp);
                                _contenedorTrabajo.save();
                            }
                    }
                    cantCert = cantidadfilas;
                }
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
           

            return cantCert;
        }

        public byte[] GenerarExcel(List<Dictionary<string, object>> tiporeporte,ReporteVM report, List<string> valores, List<string> encabezados)
        {
            //Define titulo reporte
            string titulo = string.Empty;
            switch (report.ReporteSeleccionado)
            {
                case "Pagos":
                    titulo = "Reporte Pagos Realizados";
                    break;
                case "Procesos Participantes":
                    titulo = "Reporte Colegio '' Programa SSE Periodo 2024";
                    break;
                default:
                    break;
            }

            //Definir Ancho
            var a = encabezados.Count() - 1;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Reporte Pagos");
            sheet.IsPrintGridlines = false;

            #region banner
            //Crear el título
            IRow titleRow = sheet.CreateRow(0);
            ICell titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue("Fundación de Busqueda y Rescate - FUNSAR");

            // Aplicar estilo de título con color de fondo
            ICellStyle titleStyle = workbook.CreateCellStyle();
            IFont titleFont = workbook.CreateFont();
            titleFont.FontHeightInPoints = 16;
            titleFont.Boldweight = (short)FontBoldWeight.Bold;
            titleFont.Color = IndexedColors.White.Index;
            titleStyle.SetFont(titleFont);
            titleStyle.Alignment = HorizontalAlignment.Center;
            titleStyle.VerticalAlignment = VerticalAlignment.Center;

            // Configurar color de fondo
            titleStyle.FillForegroundColor = IndexedColors.DarkGreen.Index; // Cambia el color a tu preferencia
            titleStyle.FillPattern = FillPattern.SolidForeground;

            titleCell.CellStyle = titleStyle;

            // Combinar celdas para el título
            sheet.AddMergedRegion(new CellRangeAddress(0, 2, 0, a));
            #endregion

            #region titulo

            //Crear el título de "Datos"
            IRow titleRow1 = sheet.CreateRow(4);
            ICell titleCell1 = titleRow1.CreateCell(0);
            titleCell1.SetCellValue("Datos Participante");

            // Aplicar estilo de título con color de fondo
            ICellStyle titleStyle1 = workbook.CreateCellStyle();
            titleStyle1.SetFont(titleFont);
            titleStyle1.Alignment = HorizontalAlignment.Center;
            titleStyle1.VerticalAlignment = VerticalAlignment.Center;

            // Configurar color de fondo
            titleStyle1.FillForegroundColor = IndexedColors.Grey40Percent.Index; // Cambia el color a tu preferencia
            titleStyle1.FillPattern = FillPattern.SolidForeground;

            titleCell1.CellStyle = titleStyle1;

            // Combinar celdas para el título
            sheet.AddMergedRegion(new CellRangeAddress(4, 5, 0, 3));

            #endregion

            #region titulo2

            //Crear el título del reporte
            ICell titleCell2 = titleRow1.CreateCell(4);
            titleCell2.SetCellValue(titulo);

            // Aplicar estilo de título con color de fondo
            ICellStyle titleStyle2 = workbook.CreateCellStyle();
            titleStyle2.SetFont(titleFont);
            titleStyle2.Alignment = HorizontalAlignment.Center;
            titleStyle2.VerticalAlignment = VerticalAlignment.Center;

            // Configurar color de fondo
            titleStyle2.FillForegroundColor = IndexedColors.Grey40Percent.Index; // Cambia el color a tu preferencia
            titleStyle2.FillPattern = FillPattern.SolidForeground;

            titleCell2.CellStyle = titleStyle2;

            // Combinar celdas para el título
            sheet.AddMergedRegion(new CellRangeAddress(4, 5, 4, a));

            #endregion

            #region espaciosentretitulos
            // Crear una fila
            IRow row1 = sheet.CreateRow(3);
            IRow row2 = sheet.CreateRow(6);
            row1.HeightInPoints = 5;
            row2.HeightInPoints = 5;

            #endregion
            //DATOS REPORTE

            // Crear estilo de encabezado
            ICellStyle headerStyle = workbook.CreateCellStyle();
            headerStyle.FillForegroundColor = IndexedColors.DarkGreen.Index; // Color de fondo
            headerStyle.FillPattern = FillPattern.SolidForeground;           // Patrón de relleno sólido

            // Configurar fuente del encabezado
            IFont headerFont = workbook.CreateFont();
            headerFont.FontHeightInPoints = 12; // Tamaño de la fuente
            headerFont.Color = IndexedColors.White.Index; // Color del texto
            headerFont.Boldweight = (short)FontBoldWeight.Bold; // Negrita
            headerStyle.SetFont(headerFont);

            // Crear fila de encabezados
            IRow headerRow = sheet.CreateRow(7); // Fila 8 (índice base cero)
            for (int i = 0; i < encabezados.Count; i++)
            {
                ICell cell = headerRow.CreateCell(i); // Crear celda
                cell.SetCellValue(encabezados[i]);    // Establecer el valor del encabezado
                cell.CellStyle = headerStyle;         // Aplicar el estilo a la celda
            }

            // Llenar filas de datos
            for (int i = 0; i < tiporeporte.Count; i++)
            {
                var dr = tiporeporte[i];
                IRow row = sheet.CreateRow(8 + i);

                for (int j = 0; j < valores.Count; j++)
                {
                    var valor = ObtenerValorPorRuta(dr, valores[j]);
                    row.CreateCell(j).SetCellValue(valor?.ToString() ?? "N/A");
                }
            }

            using var ms = new MemoryStream();
            workbook.Write(ms,true);
            return ms.ToArray();
        }

        private object ObtenerValorPorRuta(Dictionary<string,object> objeto, string ruta)
        {

            return objeto.ContainsKey(ruta) ? objeto[ruta] : string.Empty;

        }
    }
}