using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EMPLOYEE_MANAGER.Models;
using EMPLOYEE_MANAGER.Services;
using System.Diagnostics;



namespace EMPLOYEE_MANAGER.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly AppDbContext _context;

        public EmpleadosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Empleados
    public async Task<IActionResult> Index()
    {
      var sw = Stopwatch.StartNew();
      var empleados = await _context.Empleados.ToListAsync();
      sw.Stop();
      AuditoriaStore.Registros.Add(new AuditoriaRegistro
        {
          FechaHora = DateTime.Now,
          Accion = "Select",
          TipoAcceso = "EF Core",
          Entidad = "Empleado",
          IdAfectado = null,
          TiempoMs = sw.ElapsedMilliseconds,
          Ruta = "/Empleados/Index",
          Metodo = "GET",
          Observacion = "Carga de lista de empleados"
        });

        return View(empleados);
    }


        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Puesto,FechaContratacion,Salario")] Empleado empleado)
    {
       if (ModelState.IsValid)
       {
          var sw = Stopwatch.StartNew();

          _context.Add(empleado);
          await _context.SaveChangesAsync();

          sw.Stop();

          AuditoriaStore.Registros.Add(new AuditoriaRegistro
           {
              FechaHora = DateTime.Now,
              Accion = "Insert",
              TipoAcceso = "EF Core",
              Entidad = "Empleado",
              IdAfectado = empleado.Id,
              TiempoMs = sw.ElapsedMilliseconds,
              Ruta = "/Empleados/Create",
              Metodo = "POST",
              Observacion = "Inserción de empleado desde formulario"
            });

            return RedirectToAction(nameof(Index));
        }
        return View(empleado);
    }


        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Puesto,FechaContratacion,Salario")] Empleado empleado)
{
    if (id != empleado.Id)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            _context.Update(empleado);
            await _context.SaveChangesAsync();

            sw.Stop();

            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Update",
                TipoAcceso = "EF Core",
                Entidad = "Empleado",
                IdAfectado = empleado.Id,
                TiempoMs = sw.ElapsedMilliseconds,
                Ruta = "/Empleados/Edit",
                Metodo = "POST",
                Observacion = "Edición de empleado"
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            sw.Stop(); // detener el reloj también en caso de excepción

            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Update-Failed",
                TipoAcceso = "EF Core",
                Entidad = "Empleado",
                IdAfectado = empleado.Id,
                TiempoMs = sw.ElapsedMilliseconds,
                Ruta = "/Empleados/Edit",
                Metodo = "POST",
                Observacion = "Error de concurrencia"
            });

            if (!EmpleadoExists(empleado.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToAction(nameof(Index));
    }

    return View(empleado);
}


        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleados/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var sw = Stopwatch.StartNew();

    var empleado = await _context.Empleados.FindAsync(id);
    if (empleado != null)
    {
        _context.Empleados.Remove(empleado);
    }

    await _context.SaveChangesAsync();

    sw.Stop();

    AuditoriaStore.Registros.Add(new AuditoriaRegistro
    {
        FechaHora = DateTime.Now,
        Accion = "Delete",
        TipoAcceso = "EF Core",
        Entidad = "Empleado",
        IdAfectado = id,
        TiempoMs = sw.ElapsedMilliseconds,
        Ruta = "/Empleados/Delete",
        Metodo = "POST",
        Observacion = "Eliminación de empleado"
    });

    return RedirectToAction(nameof(Index));
}


        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
