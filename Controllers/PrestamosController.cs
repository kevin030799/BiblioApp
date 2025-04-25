using BiblioApp.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BiblioApp.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly PrestamoService _prestamoService;

        public PrestamosController(PrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        // GET: /Prestamos
        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();
            return View(prestamos);
        }

        // GET: /Prestamos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        // GET: /Prestamos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrestamosModel prestamo)
        {
            if (ModelState.IsValid)
            {
                await _prestamoService.CreatePrestamoAsync(prestamo);
                return RedirectToAction(nameof(Index));
            }
            return View(prestamo);
        }

        // GET: /Prestamos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        // POST: /Prestamos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrestamosModel prestamo)
        {
            if (id != prestamo.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _prestamoService.UpdatePrestamoAsync(id, prestamo);
                return RedirectToAction(nameof(Index));
            }
            return View(prestamo);
        }

        // GET: /Prestamos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        // POST: /Prestamos/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prestamoService.DeletePrestamoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToPdf()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();
            using (var stream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, stream).CloseStream = false;
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("Lista de Préstamos"));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                var table = new PdfPTable(7);
                table.AddCell("ID Usuario");
                table.AddCell("ID Libro");
                table.AddCell("Fecha Préstamo");
                table.AddCell("Fecha Esperada");
                table.AddCell("Fecha Real");
                table.AddCell("Estado");

                foreach (var prestamo in prestamos)
                {
                    table.AddCell(prestamo.IdUsuario.ToString());
                    table.AddCell(prestamo.IdLibro.ToString());
                    table.AddCell(prestamo.FechaPrestamo.ToShortDateString());
                    table.AddCell(prestamo.FechaDevolucionEsperada.ToShortDateString());
                    table.AddCell(prestamo.FechaDevolucionReal?.ToShortDateString() ?? "Pendiente");
                    table.AddCell(prestamo.Estado);
                }

                doc.Add(table);
                doc.Close();

                stream.Position = 0;
                return File(stream.ToArray(), "application/pdf", "Prestamos.pdf");
            }
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Prestamos");

                worksheet.Cells[1, 1].Value = "ID Usuario";
                worksheet.Cells[1, 2].Value = "ID Libro";
                worksheet.Cells[1, 3].Value = "Fecha Préstamo";
                worksheet.Cells[1, 4].Value = "Fecha Esperada";
                worksheet.Cells[1, 5].Value = "Fecha Real";
                worksheet.Cells[1, 6].Value = "Estado";

                int row = 2;
                foreach (var prestamo in prestamos)
                {
                    worksheet.Cells[row, 1].Value = prestamo.IdUsuario;
                    worksheet.Cells[row, 2].Value = prestamo.IdLibro;
                    worksheet.Cells[row, 3].Value = prestamo.FechaPrestamo.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = prestamo.FechaDevolucionEsperada.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 5].Value = prestamo.FechaDevolucionReal?.ToString("yyyy-MM-dd") ?? "Pendiente";
                    worksheet.Cells[row, 6].Value = prestamo.Estado;
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Prestamos.xlsx");
            }
        }
    }
}


