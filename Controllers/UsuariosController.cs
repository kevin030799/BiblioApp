using BiblioApp.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BiblioApp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: /Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            return View(usuarios);
        }

        // GET: /Usuarios/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // GET: /Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuariosModel usuario)
        {
            if (ModelState.IsValid)
            {
                await _usuarioService.CreateUsuarioAsync(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: /Usuarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: /Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuariosModel usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _usuarioService.UpdateUsuarioAsync(id, usuario);
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // GET: /Usuarios/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: /Usuarios/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _usuarioService.DeleteUsuarioAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Usuarios/ExportToPdf
        public async Task<IActionResult> ExportToPdf()
        {
            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            using (var stream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, stream).CloseStream = false;
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("Lista de Usuarios"));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                var table = new PdfPTable(6);
                table.AddCell("Nombre");
                table.AddCell("Apellido");
                table.AddCell("Correo");
                table.AddCell("Teléfono");
                table.AddCell("Tipo de Usuario");
                table.AddCell("Clave");

                foreach (var usuario in usuarios)
                {
                    table.AddCell(usuario.Nombre);
                    table.AddCell(usuario.Apellido);
                    table.AddCell(usuario.Correo);
                    table.AddCell(usuario.Telefono);
                    table.AddCell(usuario.TipoUsuario);
                    table.AddCell(usuario.Clave);
                }

                doc.Add(table);
                doc.Close();

                stream.Position = 0;
                return File(stream.ToArray(), "application/pdf", "Usuarios.pdf");
            }
        }

        // GET: /Usuarios/ExportToExcel
        public async Task<IActionResult> ExportToExcel()
        {
            var usuarios = await _usuarioService.GetAllUsuariosAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Usuarios");

                // Encabezados
                worksheet.Cells[1, 1].Value = "Nombre";
                worksheet.Cells[1, 2].Value = "Apellido";
                worksheet.Cells[1, 3].Value = "Correo";
                worksheet.Cells[1, 4].Value = "Teléfono";
                worksheet.Cells[1, 5].Value = "Tipo de Usuario";
                worksheet.Cells[1, 6].Value = "Clave";

                int row = 2;
                foreach (var usuario in usuarios)
                {
                    worksheet.Cells[row, 1].Value = usuario.Nombre;
                    worksheet.Cells[row, 2].Value = usuario.Apellido;
                    worksheet.Cells[row, 3].Value = usuario.Correo;
                    worksheet.Cells[row, 4].Value = usuario.Telefono;
                    worksheet.Cells[row, 5].Value = usuario.TipoUsuario;
                    worksheet.Cells[row, 6].Value = usuario.Clave;
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Usuarios.xlsx");
            }
        }
    }
}
