// victorurrutia30-chatbotutec/Services/ReportService.cs
using System;
using System.Data;
using System.IO;
using OfficeOpenXml;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;


namespace ChatbotUTEC.Services
{
    public class ReportService
    {
        private readonly DatabaseHelper _db;

        public ReportService(DatabaseHelper db)
        {
            _db = db;
            // EPPlus: licencia no comercial
            ExcelPackage.License.SetNonCommercialPersonal("Tu Nombre Aquí");
        }

        // 1) Preguntas más frecuentes
        public byte[] GenerarExcelReporteFrecuentes()
            => GenerarExcelDesdeVista("vw_FrecuentesPreguntas", "Frecuentes");

        public byte[] GenerarPdfReporteFrecuentes()
            => GenerarPdfDesdeVista("vw_FrecuentesPreguntas", "Preguntas Más Frecuentes");

        // 2) Tiempo promedio de respuesta
        public byte[] GenerarExcelReporteTiempoRespuesta()
            => GenerarExcelDesdeVista("vw_TiempoRespuestaPromedio", "TiempoRespuesta");

        public byte[] GenerarPdfReporteTiempoRespuesta()
            => GenerarPdfDesdeVista("vw_TiempoRespuestaPromedio", "Tiempo Promedio de Respuesta");

        // 3) Distribución de intents
        public byte[] GenerarExcelReporteIntents()
            => GenerarExcelDesdeVista("vw_DistribucionIntents", "Intents");

        public byte[] GenerarPdfReporteIntents()
            => GenerarPdfDesdeVista("vw_DistribucionIntents", "Distribución de Intents");

        // 4) Entidades más usadas
        public byte[] GenerarExcelReporteEntidades()
            => GenerarExcelDesdeVista("vw_EntidadesMasUsadas", "Entidades");

        public byte[] GenerarPdfReporteEntidades()
            => GenerarPdfDesdeVista("vw_EntidadesMasUsadas", "Entidades Más Usadas");

        // 5) Interacciones por día
        public byte[] GenerarExcelReporteInteraccionesPorDia()
            => GenerarExcelDesdeVista("vw_InteraccionesPorDia", "InteraccionesPorDia");

        public byte[] GenerarPdfReporteInteraccionesPorDia()
            => GenerarPdfDesdeVista("vw_InteraccionesPorDia", "Interacciones por Día");

        // 6) Sesiones de usuario
        public byte[] GenerarExcelReporteSesionesUsuario()
            => GenerarExcelDesdeVista("vw_SesionesUsuario", "SesionesUsuario");

        public byte[] GenerarPdfReporteSesionesUsuario()
            => GenerarPdfDesdeVista("vw_SesionesUsuario", "Sesiones de Usuario");

        // 7) Promedio por intent
        public byte[] GenerarExcelReportePromedioPorIntent()
            => GenerarExcelDesdeVista("vw_PromedioPorIntent", "PromedioPorIntent");

        public byte[] GenerarPdfReportePromedioPorIntent()
            => GenerarPdfDesdeVista("vw_PromedioPorIntent", "Promedio por Intent");

        // 8) Interacciones por hora
        public byte[] GenerarExcelReporteInteraccionesPorHora()
            => GenerarExcelDesdeVista("vw_InteraccionesPorHora", "InteraccionesPorHora");

        public byte[] GenerarPdfReporteInteraccionesPorHora()
            => GenerarPdfDesdeVista("vw_InteraccionesPorHora", "Interacciones por Hora");

        // 9) Usuario más activo por día
        public byte[] GenerarExcelReporteUsuarioMasActivoPorDia()
            => GenerarExcelDesdeVista("vw_UsuarioMasActivoPorDia", "UsuarioMasActivoPorDia");

        public byte[] GenerarPdfReporteUsuarioMasActivoPorDia()
            => GenerarPdfDesdeVista("vw_UsuarioMasActivoPorDia", "Usuario Más Activo por Día");

        // 10) Usuarios activos
        public byte[] GenerarExcelReporteUsuariosActivos()
            => GenerarExcelDesdeVista("vw_UsuariosActivos", "UsuariosActivos");

        public byte[] GenerarPdfReporteUsuariosActivos()
            => GenerarPdfDesdeVista("vw_UsuariosActivos", "Usuarios Activos");

        // Helper para Excel
        private byte[] GenerarExcelDesdeVista(string vista, string sheetName)
        {
            DataTable table = _db.EjecutarConsultaATabla($"SELECT * FROM {vista}");
            using var pkg = new ExcelPackage();
            var ws = pkg.Workbook.Worksheets.Add(sheetName);
            ws.Cells["A1"].LoadFromDataTable(table, true);
            ws.Cells.AutoFitColumns();
            return pkg.GetAsByteArray();
        }

        // Helper para PDF
        private byte[] GenerarPdfDesdeVista(string vista, string titulo)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdfDoc = new PdfDocument(writer);
            var doc = new Document(pdfDoc);

            // Título
            doc.Add(new Paragraph($"Reporte: {titulo}")
                .SetFontSize(16)
                .SetMarginBottom(10)
            );

            // Tabla
            DataTable table = _db.EjecutarConsultaATabla($"SELECT * FROM {vista}");
            var pdfTable = new Table(table.Columns.Count);

            // Encabezados
            foreach (DataColumn col in table.Columns)
                pdfTable.AddHeaderCell(col.ColumnName);

            // Filas
            foreach (DataRow row in table.Rows)
                foreach (var cell in row.ItemArray)
                    pdfTable.AddCell(cell.ToString());

            doc.Add(pdfTable);
            doc.Close();
            return ms.ToArray();
        }
    }
}
