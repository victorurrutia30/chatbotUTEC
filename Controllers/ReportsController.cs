// victorurrutia30-chatbotutec/Controllers/ReportsController.cs
using Microsoft.AspNetCore.Mvc;
using ChatbotUTEC.Services;

namespace EchoBot.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reports;

        public ReportsController(ReportService reports) => _reports = reports;

        // 1) Preguntas más frecuentes
        [HttpGet("frecuentes/excel")]
        public IActionResult GetFrecuentesExcel()
        {
            var bytes = _reports.GenerarExcelReporteFrecuentes();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Frecuentes.xlsx");
        }

        [HttpGet("frecuentes/pdf")]
        public IActionResult GetFrecuentesPdf()
        {
            var bytes = _reports.GenerarPdfReporteFrecuentes();
            return File(bytes, "application/pdf", "Frecuentes.pdf");
        }

        // 2) Tiempo promedio de respuesta
        [HttpGet("tiempo-respuesta/excel")]
        public IActionResult GetTiempoRespuestaExcel()
        {
            var bytes = _reports.GenerarExcelReporteTiempoRespuesta();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "TiempoRespuesta.xlsx");
        }

        [HttpGet("tiempo-respuesta/pdf")]
        public IActionResult GetTiempoRespuestaPdf()
        {
            var bytes = _reports.GenerarPdfReporteTiempoRespuesta();
            return File(bytes, "application/pdf", "TiempoRespuesta.pdf");
        }

        // 3) Distribución de intents
        [HttpGet("intents/excel")]
        public IActionResult GetIntentsExcel()
        {
            var bytes = _reports.GenerarExcelReporteIntents();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Intents.xlsx");
        }

        [HttpGet("intents/pdf")]
        public IActionResult GetIntentsPdf()
        {
            var bytes = _reports.GenerarPdfReporteIntents();
            return File(bytes, "application/pdf", "Intents.pdf");
        }

        // 4) Entidades más usadas
        [HttpGet("entidades/excel")]
        public IActionResult GetEntidadesExcel()
        {
            var bytes = _reports.GenerarExcelReporteEntidades();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Entidades.xlsx");
        }

        [HttpGet("entidades/pdf")]
        public IActionResult GetEntidadesPdf()
        {
            var bytes = _reports.GenerarPdfReporteEntidades();
            return File(bytes, "application/pdf", "Entidades.pdf");
        }

        // 5) Interacciones por día
        [HttpGet("interacciones-dia/excel")]
        public IActionResult GetInteraccionesDiaExcel()
        {
            var bytes = _reports.GenerarExcelReporteInteraccionesPorDia();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "InteraccionesDia.xlsx");
        }

        [HttpGet("interacciones-dia/pdf")]
        public IActionResult GetInteraccionesDiaPdf()
        {
            var bytes = _reports.GenerarPdfReporteInteraccionesPorDia();
            return File(bytes, "application/pdf", "InteraccionesDia.pdf");
        }

        // 6) Sesiones de usuario
        [HttpGet("sesiones-usuario/excel")]
        public IActionResult GetSesionesUsuarioExcel()
        {
            var bytes = _reports.GenerarExcelReporteSesionesUsuario();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "SesionesUsuario.xlsx");
        }

        [HttpGet("sesiones-usuario/pdf")]
        public IActionResult GetSesionesUsuarioPdf()
        {
            var bytes = _reports.GenerarPdfReporteSesionesUsuario();
            return File(bytes, "application/pdf", "SesionesUsuario.pdf");
        }

        // 7) Promedio por intent
        [HttpGet("promedio-intent/excel")]
        public IActionResult GetPromedioIntentExcel()
        {
            var bytes = _reports.GenerarExcelReportePromedioPorIntent();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "PromedioIntent.xlsx");
        }

        [HttpGet("promedio-intent/pdf")]
        public IActionResult GetPromedioIntentPdf()
        {
            var bytes = _reports.GenerarPdfReportePromedioPorIntent();
            return File(bytes, "application/pdf", "PromedioIntent.pdf");
        }

        // 8) Interacciones por hora
        [HttpGet("interacciones-hora/excel")]
        public IActionResult GetInteraccionesHoraExcel()
        {
            var bytes = _reports.GenerarExcelReporteInteraccionesPorHora();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "InteraccionesHora.xlsx");
        }

        [HttpGet("interacciones-hora/pdf")]
        public IActionResult GetInteraccionesHoraPdf()
        {
            var bytes = _reports.GenerarPdfReporteInteraccionesPorHora();
            return File(bytes, "application/pdf", "InteraccionesHora.pdf");
        }

        // 9) Usuario más activo por día
        [HttpGet("usuario-mas-activo/excel")]
        public IActionResult GetUsuarioMasActivoExcel()
        {
            var bytes = _reports.GenerarExcelReporteUsuarioMasActivoPorDia();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "UsuarioMasActivo.xlsx");
        }

        [HttpGet("usuario-mas-activo/pdf")]
        public IActionResult GetUsuarioMasActivoPdf()
        {
            var bytes = _reports.GenerarPdfReporteUsuarioMasActivoPorDia();
            return File(bytes, "application/pdf", "UsuarioMasActivo.pdf");
        }

        // 10) Usuarios activos
        [HttpGet("usuarios-activos/excel")]
        public IActionResult GetUsuariosActivosExcel()
        {
            var bytes = _reports.GenerarExcelReporteUsuariosActivos();
            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "UsuariosActivos.xlsx");
        }

        [HttpGet("usuarios-activos/pdf")]
        public IActionResult GetUsuariosActivosPdf()
        {
            var bytes = _reports.GenerarPdfReporteUsuariosActivos();
            return File(bytes, "application/pdf", "UsuariosActivos.pdf");
        }
    }
}
