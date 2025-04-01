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
      // Establece cookie para usar conexión local (EF Core)
      Response.Cookies.Append("ConnectionPreference", "Local");
      ViewData["Modo"] = "EF Core"; // ← esto indica el modo
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

[HttpPost]
public async Task<IActionResult> InsertarMasivo(bool auditoriaIndividual = false)
{
    var empleados = new List<Empleado>
    {
        new Empleado { Nombre = "Juan Pérez", Puesto = "Desarrollador", FechaContratacion = DateTime.Parse("2022-01-15"), Salario = 48000 },
        new Empleado { Nombre = "Ana López", Puesto = "Diseñadora UX", FechaContratacion = DateTime.Parse("2021-08-01"), Salario = 52000 },
        new Empleado { Nombre = "Carlos García", Puesto = "Administrador de Sistemas", FechaContratacion = DateTime.Parse("2020-05-10"), Salario = 61000 },
        new Empleado { Nombre = "María Fernández", Puesto = "Scrum Master", FechaContratacion = DateTime.Parse("2019-11-22"), Salario = 70000 },
        new Empleado { Nombre = "Luis Torres", Puesto = "Tester QA", FechaContratacion = DateTime.Parse("2023-03-07"), Salario = 43000 },
        new Empleado { Nombre = "Carmen Díaz", Puesto = "Gerente de Proyecto", FechaContratacion = DateTime.Parse("2018-04-30"), Salario = 85000 },
        new Empleado { Nombre = "Roberto Sánchez", Puesto = "Backend Developer", FechaContratacion = DateTime.Parse("2021-06-18"), Salario = 56000 },
        new Empleado { Nombre = "Lucía Gómez", Puesto = "Frontend Developer", FechaContratacion = DateTime.Parse("2020-12-11"), Salario = 54000 },
        new Empleado { Nombre = "Alberto Ruiz", Puesto = "DevOps Engineer", FechaContratacion = DateTime.Parse("2022-09-23"), Salario = 62000 },
        new Empleado { Nombre = "Natalia Vargas", Puesto = "Data Analyst", FechaContratacion = DateTime.Parse("2021-10-15"), Salario = 58000 },
        new Empleado { Nombre = "Fernando Morales", Puesto = "Data Scientist", FechaContratacion = DateTime.Parse("2020-03-08"), Salario = 75000 },
        new Empleado { Nombre = "Javier Soto", Puesto = "Product Owner", FechaContratacion = DateTime.Parse("2019-07-01"), Salario = 72000 },
        new Empleado { Nombre = "Laura Castillo", Puesto = "Diseñadora Gráfica", FechaContratacion = DateTime.Parse("2023-01-12"), Salario = 47000 },
        new Empleado { Nombre = "Sofía Romero", Puesto = "Community Manager", FechaContratacion = DateTime.Parse("2022-11-02"), Salario = 42000 },
        new Empleado { Nombre = "Diego Herrera", Puesto = "Tester QA", FechaContratacion = DateTime.Parse("2021-05-05"), Salario = 44000 },
        new Empleado { Nombre = "Marta Peña", Puesto = "Desarrolladora Full Stack", FechaContratacion = DateTime.Parse("2020-10-20"), Salario = 60000 },
        new Empleado { Nombre = "Iván Castro", Puesto = "Técnico Soporte", FechaContratacion = DateTime.Parse("2021-02-17"), Salario = 38000 },
        new Empleado { Nombre = "Valeria Salinas", Puesto = "Especialista en Seguridad", FechaContratacion = DateTime.Parse("2019-09-03"), Salario = 68000 },
        new Empleado { Nombre = "Patricia Reyes", Puesto = "Scrum Master", FechaContratacion = DateTime.Parse("2023-03-20"), Salario = 71000 },
        new Empleado { Nombre = "Héctor Flores", Puesto = "Ingeniero de Pruebas", FechaContratacion = DateTime.Parse("2022-06-14"), Salario = 45000 },
        new Empleado { Nombre = "Gabriela Mejía", Puesto = "Ingeniera de Datos", FechaContratacion = DateTime.Parse("2020-08-28"), Salario = 63000 },
        new Empleado { Nombre = "Tomás Rivas", Puesto = "Diseñador UI", FechaContratacion = DateTime.Parse("2021-07-04"), Salario = 50000 },
        new Empleado { Nombre = "Esteban León", Puesto = "Líder Técnico", FechaContratacion = DateTime.Parse("2018-12-12"), Salario = 82000 },
        new Empleado { Nombre = "Daniela Pineda", Puesto = "Desarrolladora Móvil", FechaContratacion = DateTime.Parse("2022-04-10"), Salario = 56000 },
        new Empleado { Nombre = "Francisco Solís", Puesto = "Administrador de Redes", FechaContratacion = DateTime.Parse("2020-01-30"), Salario = 59000 },
        new Empleado { Nombre = "Marcela Cortés", Puesto = "Analista de Negocios", FechaContratacion = DateTime.Parse("2021-09-19"), Salario = 64000 },
        new Empleado { Nombre = "Joaquín Molina", Puesto = "Ingeniero QA", FechaContratacion = DateTime.Parse("2020-06-25"), Salario = 46000 },
        new Empleado { Nombre = "Nicolás Duarte", Puesto = "Arquitecto de Software", FechaContratacion = DateTime.Parse("2019-05-13"), Salario = 90000 },
        new Empleado { Nombre = "Andrea Lozano", Puesto = "Especialista SEO", FechaContratacion = DateTime.Parse("2023-02-05"), Salario = 48000 },
        new Empleado { Nombre = "Emilio Aguirre", Puesto = "Desarrollador Backend", FechaContratacion = DateTime.Parse("2021-03-15"), Salario = 57000 },
        new Empleado { Nombre = "Julia Esquivel", Puesto = "Gerente de Tecnología", FechaContratacion = DateTime.Parse("2017-08-08"), Salario = 98000 },
        new Empleado { Nombre = "Bruno Cordero", Puesto = "Tester Automatizado", FechaContratacion = DateTime.Parse("2022-07-21"), Salario = 49000 },
        new Empleado { Nombre = "Isabel Tapia", Puesto = "Consultora TI", FechaContratacion = DateTime.Parse("2020-04-04"), Salario = 66000 },
        new Empleado { Nombre = "Manuel Duarte", Puesto = "Ingeniero de Infraestructura", FechaContratacion = DateTime.Parse("2021-11-11"), Salario = 61000 },
        new Empleado { Nombre = "Ximena Ríos", Puesto = "Desarrolladora Web", FechaContratacion = DateTime.Parse("2023-01-01"), Salario = 52000 },
        new Empleado { Nombre = "Ricardo Palma", Puesto = "Administrador Base de Datos", FechaContratacion = DateTime.Parse("2019-10-10"), Salario = 69000 },
        new Empleado { Nombre = "Victoria Cárdenas", Puesto = "Líder de QA", FechaContratacion = DateTime.Parse("2020-09-09"), Salario = 71000 },
        new Empleado { Nombre = "Álvaro Pinto", Puesto = "Machine Learning Engineer", FechaContratacion = DateTime.Parse("2021-12-12"), Salario = 74000 },
        new Empleado { Nombre = "Paula Zamora", Puesto = "Analista Funcional", FechaContratacion = DateTime.Parse("2022-02-02"), Salario = 60000 },
        new Empleado { Nombre = "Camilo Medina", Puesto = "Desarrollador Frontend", FechaContratacion = DateTime.Parse("2020-05-05"), Salario = 55000 },
    };

    if (auditoriaIndividual)
    {
        foreach (var emp in empleados)
        {
            var sw = Stopwatch.StartNew();
            _context.Empleados.Add(emp);
            await _context.SaveChangesAsync();
            sw.Stop();

            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Insert",
                TipoAcceso = "EF Core",
                Entidad = "Empleado",
                IdAfectado = emp.Id,
                TiempoMs = sw.ElapsedMilliseconds,
                Ruta = "/Empleados/InsertarMasivo",
                Metodo = "POST",
                Observacion = "Inserción individual desde InsertarMasivo"
            });
        }
    }
    else
    {
        var sw = Stopwatch.StartNew();
        _context.Empleados.AddRange(empleados);
        await _context.SaveChangesAsync();
        sw.Stop();

        AuditoriaStore.Registros.Add(new AuditoriaRegistro
        {
            FechaHora = DateTime.Now,
            Accion = "Insert",
            TipoAcceso = "EF Core",
            Entidad = "Empleado",
            IdAfectado = null,
            TiempoMs = sw.ElapsedMilliseconds,
            Ruta = "/Empleados/InsertarMasivo",
            Metodo = "POST",
            Observacion = "Inserción de 40 empleados con datos predefinidos"
        });
    }

    return RedirectToAction(nameof(Index));
}


[HttpPost]
public async Task<IActionResult> EliminarMasivo(bool auditoriaIndividual = false)
{
    // Lista de nombres usados en InsertarMasivo
    var nombres = new List<string>
    {
        "Juan Pérez", "Ana López", "Carlos García", "María Fernández", "Luis Torres", "Carmen Díaz", "Roberto Sánchez", "Lucía Gómez",
        "Alberto Ruiz", "Natalia Vargas", "Fernando Morales", "Javier Soto", "Laura Castillo", "Sofía Romero", "Diego Herrera", "Marta Peña",
        "Iván Castro", "Valeria Salinas", "Patricia Reyes", "Héctor Flores", "Gabriela Mejía", "Tomás Rivas", "Esteban León", "Daniela Pineda",
        "Francisco Solís", "Marcela Cortés", "Joaquín Molina", "Nicolás Duarte", "Andrea Lozano", "Emilio Aguirre", "Julia Esquivel", "Bruno Cordero",
        "Isabel Tapia", "Manuel Duarte", "Ximena Ríos", "Ricardo Palma", "Victoria Cárdenas", "Álvaro Pinto", "Paula Zamora", "Camilo Medina"
    };

    var empleados = _context.Empleados.Where(e => nombres.Contains(e.Nombre)).ToList();

    if (!empleados.Any())
        return RedirectToAction(nameof(Index));

    if (auditoriaIndividual)
    {
        foreach (var emp in empleados)
        {
            var sw = Stopwatch.StartNew();

            _context.Empleados.Remove(emp);
            await _context.SaveChangesAsync();

            sw.Stop();

            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Delete",
                TipoAcceso = "EF Core",
                Entidad = "Empleado",
                IdAfectado = emp.Id,
                TiempoMs = sw.ElapsedMilliseconds,
                Ruta = "/Empleados/EliminarMasivo",
                Metodo = "POST",
                Observacion = "Eliminación individual desde EliminarMasivo"
            });
        }
    }
    else
    {
        var sw = Stopwatch.StartNew();

        _context.Empleados.RemoveRange(empleados);
        await _context.SaveChangesAsync();

        sw.Stop();

        AuditoriaStore.Registros.Add(new AuditoriaRegistro
        {
            FechaHora = DateTime.Now,
            Accion = "Delete",
            TipoAcceso = "EF Core",
            Entidad = "Empleado",
            IdAfectado = null,
            TiempoMs = sw.ElapsedMilliseconds,
            Ruta = "/Empleados/EliminarMasivo",
            Metodo = "POST",
            Observacion = "Eliminación automática de empleados predefinidos"
        });
    }

    return RedirectToAction(nameof(Index));
}

[HttpPost]
public async Task<IActionResult> EditarMasivo(bool auditoriaIndividual = false)
{
    var random = new Random();

    var nombres = new List<string>
    {
        "Juan Pérez", "Ana López", "Carlos García", "María Fernández", "Luis Torres", "Carmen Díaz", "Roberto Sánchez", "Lucía Gómez",
        "Alberto Ruiz", "Natalia Vargas", "Fernando Morales", "Javier Soto", "Laura Castillo", "Sofía Romero", "Diego Herrera", "Marta Peña",
        "Iván Castro", "Valeria Salinas", "Patricia Reyes", "Héctor Flores", "Gabriela Mejía", "Tomás Rivas", "Esteban León", "Daniela Pineda",
        "Francisco Solís", "Marcela Cortés", "Joaquín Molina", "Nicolás Duarte", "Andrea Lozano", "Emilio Aguirre", "Julia Esquivel", "Bruno Cordero",
        "Isabel Tapia", "Manuel Duarte", "Ximena Ríos", "Ricardo Palma", "Victoria Cárdenas", "Álvaro Pinto", "Paula Zamora", "Camilo Medina"
    };

    var puestosDisponibles = new List<string>
    {
        "Analista", "Arquitecto", "Consultor", "Líder Técnico", "Desarrollador", "Tester QA",
        "Scrum Master", "DevOps", "Gerente", "Soporte Técnico"
    };

    var empleados = _context.Empleados.Where(e => nombres.Contains(e.Nombre)).ToList();

    if (auditoriaIndividual)
    {
        foreach (var empleado in empleados)
        {
            var sw = Stopwatch.StartNew();

            empleado.Puesto = puestosDisponibles[random.Next(puestosDisponibles.Count)];
            empleado.Salario += random.Next(-3000, 5000);

            _context.Empleados.Update(empleado);
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
                Ruta = "/Empleados/EditarMasivo",
                Metodo = "POST",
                Observacion = "Edición individual desde EditarMasivo"
            });
        }
    }
    else
    {
        var sw = Stopwatch.StartNew();

        foreach (var empleado in empleados)
        {
            empleado.Puesto = puestosDisponibles[random.Next(puestosDisponibles.Count)];
            empleado.Salario += random.Next(-3000, 5000);
            _context.Empleados.Update(empleado);
        }

        await _context.SaveChangesAsync();

        sw.Stop();

        AuditoriaStore.Registros.Add(new AuditoriaRegistro
        {
            FechaHora = DateTime.Now,
            Accion = "Update",
            TipoAcceso = "EF Core",
            Entidad = "Empleado",
            IdAfectado = null,
            TiempoMs = sw.ElapsedMilliseconds,
            Ruta = "/Empleados/EditarMasivo",
            Metodo = "POST",
            Observacion = "Edición masiva aleatoria de empleados predefinidos"
        });
    }

    return RedirectToAction(nameof(Index));
}

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
