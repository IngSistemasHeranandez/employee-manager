using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using EMPLOYEE_MANAGER.Models;
using EMPLOYEE_MANAGER.Services;

namespace EMPLOYEE_MANAGER.Controllers
{
    public class EmpleadosSqlController : Controller
    {
        private readonly IConfiguration _config;

        public EmpleadosSqlController(IConfiguration config)
        {
            _config = config;
        }

        // GET: EmpleadosSql/Index
        public IActionResult Index()
        {
            // Establece cookie para usar conexión remota (SQL directa)
            Response.Cookies.Append("ConnectionPreference", "Remote");
            ViewData["Modo"] = "SQL Directa"; // ← este es el otro modo
            var empleados = new List<Empleado>();
            var sw = System.Diagnostics.Stopwatch.StartNew();

            using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
            {
                conn.Open();
                var query = "SELECT Id, Nombre, Puesto, FechaContratacion, Salario FROM Empleados";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        empleados.Add(new Empleado
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Puesto = reader.GetString(2),
                            FechaContratacion = reader.GetDateTime(3),
                            Salario = reader.GetDecimal(4)
                        });
                    }
                }
            }

            sw.Stop();
            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Select",
                TipoAcceso = "SQL Directa",
                Entidad = "Empleado",
                IdAfectado = null,
                TiempoMs = sw.ElapsedMilliseconds,
                Ruta = "/EmpleadosSql/Index",
                Metodo = "GET",
                Observacion = "Carga de lista de empleados desde SQL directa"
            });

            return View("Index", empleados); // Reutiliza la vista Index.cshtml
        }

        // GET: EmpleadosSql/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Crear (SQL Directa)";
            return View("Create"); // Reutiliza Create.cshtml
        }

        // POST: EmpleadosSql/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Nombre,Puesto,FechaContratacion,Salario")] Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Crear (SQL Directa)";
                return View("Create", empleado);
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();

            using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
            {
                conn.Open();
                var query = @"
                    INSERT INTO Empleados (Nombre, Puesto, FechaContratacion, Salario)
                    VALUES (@Nombre, @Puesto, @FechaContratacion, @Salario)";
                
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre);
                    cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);
                    cmd.Parameters.AddWithValue("@FechaContratacion", empleado.FechaContratacion);
                    cmd.Parameters.AddWithValue("@Salario", empleado.Salario);
                    cmd.ExecuteNonQuery();
                }
            }

            sw.Stop();

            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Insert",
                TipoAcceso = "SQL Directa",
                Entidad = "Empleado",
                IdAfectado = null,
                TiempoMs = sw.ElapsedMilliseconds,
                Ruta = "/EmpleadosSql/Create",
                Metodo = "POST",
                Observacion = "Inserción de empleado usando SQL directa"
            });

            return RedirectToAction(nameof(Index));
        }

        // GET: EmpleadosSql/Edit/5
public IActionResult Edit(int id)
{
    Empleado empleado = null;
    var sw = System.Diagnostics.Stopwatch.StartNew();

    using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
    {
        conn.Open();
        var query = "SELECT Id, Nombre, Puesto, FechaContratacion, Salario FROM Empleados WHERE Id = @Id";
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    empleado = new Empleado
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Puesto = reader.GetString(2),
                        FechaContratacion = reader.GetDateTime(3),
                        Salario = reader.GetDecimal(4)
                    };
                }
            }
        }
    }

    sw.Stop();

    if (empleado != null)
    {
        AuditoriaStore.Registros.Add(new AuditoriaRegistro
        {
            FechaHora = DateTime.Now,
            Accion = "Select",
            TipoAcceso = "SQL Directa",
            Entidad = "Empleado",
            IdAfectado = id,
            TiempoMs = sw.ElapsedMilliseconds,
            Ruta = "/EmpleadosSql/Edit",
            Metodo = "GET",
            Observacion = "Carga de empleado para edición desde SQL directa"
        });

        return View("Edit", empleado); // Usa la misma vista Edit.cshtml
    }

    return NotFound();
}

// POST: EmpleadosSql/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, [Bind("Id,Nombre,Puesto,FechaContratacion,Salario")] Empleado empleado)
{
    if (id != empleado.Id)
        return NotFound();

    if (!ModelState.IsValid)
        return View("Edit", empleado);

    var sw = System.Diagnostics.Stopwatch.StartNew();

    using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
    {
        conn.Open();
        var query = @"
            UPDATE Empleados
            SET Nombre = @Nombre,
                Puesto = @Puesto,
                FechaContratacion = @FechaContratacion,
                Salario = @Salario
            WHERE Id = @Id";
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre);
            cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);
            cmd.Parameters.AddWithValue("@FechaContratacion", empleado.FechaContratacion);
            cmd.Parameters.AddWithValue("@Salario", empleado.Salario);
            cmd.Parameters.AddWithValue("@Id", empleado.Id);
            cmd.ExecuteNonQuery();
        }
    }

    sw.Stop();

    AuditoriaStore.Registros.Add(new AuditoriaRegistro
    {
        FechaHora = DateTime.Now,
        Accion = "Update",
        TipoAcceso = "SQL Directa",
        Entidad = "Empleado",
        IdAfectado = empleado.Id,
        TiempoMs = sw.ElapsedMilliseconds,
        Ruta = "/EmpleadosSql/Edit",
        Metodo = "POST",
        Observacion = "Edición de empleado usando SQL directa"
    });

    return RedirectToAction(nameof(Index));
}

// GET: EmpleadosSql/Delete/5
public IActionResult Delete(int id)
{
    Empleado empleado = null;
    var sw = System.Diagnostics.Stopwatch.StartNew();

    using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
    {
        conn.Open();
        var query = "SELECT Id, Nombre, Puesto, FechaContratacion, Salario FROM Empleados WHERE Id = @Id";
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    empleado = new Empleado
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Puesto = reader.GetString(2),
                        FechaContratacion = reader.GetDateTime(3),
                        Salario = reader.GetDecimal(4)
                    };
                }
            }
        }
    }

    sw.Stop();

    if (empleado != null)
    {
        AuditoriaStore.Registros.Add(new AuditoriaRegistro
        {
            FechaHora = DateTime.Now,
            Accion = "Select",
            TipoAcceso = "SQL Directa",
            Entidad = "Empleado",
            IdAfectado = id,
            TiempoMs = sw.ElapsedMilliseconds,
            Ruta = "/EmpleadosSql/Delete",
            Metodo = "GET",
            Observacion = "Consulta de empleado para confirmación de eliminación (SQL directa)"
        });

        return View("Delete", empleado); // Reutiliza la vista Delete.cshtml
    }

    return NotFound();
}

// POST: EmpleadosSql/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public IActionResult DeleteConfirmed(int id)
{
    var sw = System.Diagnostics.Stopwatch.StartNew();

    using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
    {
        conn.Open();
        var query = "DELETE FROM Empleados WHERE Id = @Id";
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }

    sw.Stop();

    AuditoriaStore.Registros.Add(new AuditoriaRegistro
    {
        FechaHora = DateTime.Now,
        Accion = "Delete",
        TipoAcceso = "SQL Directa",
        Entidad = "Empleado",
        IdAfectado = id,
        TiempoMs = sw.ElapsedMilliseconds,
        Ruta = "/EmpleadosSql/Delete",
        Metodo = "POST",
        Observacion = "Eliminación de empleado usando SQL directa"
    });

    return RedirectToAction(nameof(Index));
}

// GET: EmpleadosSql/Details/5
public IActionResult Details(int id)
{
    Empleado empleado = null;
    var sw = System.Diagnostics.Stopwatch.StartNew();

    using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
    {
        conn.Open();
        var query = "SELECT Id, Nombre, Puesto, FechaContratacion, Salario FROM Empleados WHERE Id = @Id";
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    empleado = new Empleado
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Puesto = reader.GetString(2),
                        FechaContratacion = reader.GetDateTime(3),
                        Salario = reader.GetDecimal(4)
                    };
                }
            }
        }
    }

    sw.Stop();

    if (empleado != null)
    {
        AuditoriaStore.Registros.Add(new AuditoriaRegistro
        {
            FechaHora = DateTime.Now,
            Accion = "Select",
            TipoAcceso = "SQL Directa",
            Entidad = "Empleado",
            IdAfectado = id,
            TiempoMs = sw.ElapsedMilliseconds,
            Ruta = "/EmpleadosSql/Details",
            Metodo = "GET",
            Observacion = "Consulta de detalles de empleado (SQL directa)"
        });

        return View("Details", empleado); // Reutiliza la vista Details.cshtml
    }

    return NotFound();
}

[HttpPost]
public IActionResult InsertarMasivoSql(bool auditoriaIndividual = false)
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

    var connectionString = _config.GetConnectionString("RemoteConnection");

    if (auditoriaIndividual)
    {
        foreach (var emp in empleados)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var query = @"INSERT INTO Empleados (Nombre, Puesto, FechaContratacion, Salario)
                              VALUES (@Nombre, @Puesto, @FechaContratacion, @Salario)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", emp.Nombre);
                    cmd.Parameters.AddWithValue("@Puesto", emp.Puesto);
                    cmd.Parameters.AddWithValue("@FechaContratacion", emp.FechaContratacion);
                    cmd.Parameters.AddWithValue("@Salario", emp.Salario);
                    cmd.ExecuteNonQuery();
                }
            }

            sw.Stop();
            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Insert",
                TipoAcceso = "SQL Directa",
                Entidad = "Empleado",
                IdAfectado = null, // no obtenemos el ID insertado
                TiempoMs = sw.ElapsedMilliseconds,
                Ruta = "/EmpleadosSql/InsertarMasivoSql",
                Metodo = "POST",
                Observacion = "Inserción individual desde InsertarMasivoSql"
            });
        }
    }
    else
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            foreach (var emp in empleados)
            {
                var query = @"INSERT INTO Empleados (Nombre, Puesto, FechaContratacion, Salario)
                              VALUES (@Nombre, @Puesto, @FechaContratacion, @Salario)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Nombre", emp.Nombre);
                    cmd.Parameters.AddWithValue("@Puesto", emp.Puesto);
                    cmd.Parameters.AddWithValue("@FechaContratacion", emp.FechaContratacion);
                    cmd.Parameters.AddWithValue("@Salario", emp.Salario);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        sw.Stop();
        AuditoriaStore.Registros.Add(new AuditoriaRegistro
        {
            FechaHora = DateTime.Now,
            Accion = "Insert",
            TipoAcceso = "SQL Directa",
            Entidad = "Empleado",
            IdAfectado = null,
            TiempoMs = sw.ElapsedMilliseconds,
            Ruta = "/EmpleadosSql/InsertarMasivoSql",
            Metodo = "POST",
            Observacion = "Inserción de 40 empleados con datos predefinidos (SQL directa)"
        });
    }

    return RedirectToAction(nameof(Index));
}

[HttpPost]
public IActionResult EliminarMasivoSql(bool auditoriaIndividual = false)
{
    var nombres = new List<string>
    {
        "Juan Pérez", "Ana López", "Carlos García", "María Fernández", "Luis Torres", "Carmen Díaz", "Roberto Sánchez", "Lucía Gómez",
        "Alberto Ruiz", "Natalia Vargas", "Fernando Morales", "Javier Soto", "Laura Castillo", "Sofía Romero", "Diego Herrera", "Marta Peña",
        "Iván Castro", "Valeria Salinas", "Patricia Reyes", "Héctor Flores", "Gabriela Mejía", "Tomás Rivas", "Esteban León", "Daniela Pineda",
        "Francisco Solís", "Marcela Cortés", "Joaquín Molina", "Nicolás Duarte", "Andrea Lozano", "Emilio Aguirre", "Julia Esquivel", "Bruno Cordero",
        "Isabel Tapia", "Manuel Duarte", "Ximena Ríos", "Ricardo Palma", "Victoria Cárdenas", "Álvaro Pinto", "Paula Zamora", "Camilo Medina"
    };

    var swGlobal = System.Diagnostics.Stopwatch.StartNew();

    using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
    {
        conn.Open();

        if (auditoriaIndividual)
        {
            foreach (var nombre in nombres)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                var query = "DELETE FROM Empleados WHERE Nombre = @Nombre";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.ExecuteNonQuery();
                }

                sw.Stop();

                AuditoriaStore.Registros.Add(new AuditoriaRegistro
                {
                    FechaHora = DateTime.Now,
                    Accion = "Delete",
                    TipoAcceso = "SQL Directa",
                    Entidad = "Empleado",
                    IdAfectado = null,
                    TiempoMs = sw.ElapsedMilliseconds,
                    Ruta = "/EmpleadosSql/EliminarMasivoSql",
                    Metodo = "POST",
                    Observacion = $"Eliminación individual de '{nombre}' usando SQL directa"
                });
            }
        }
        else
        {
            var inClause = string.Join(",", nombres.Select((_, i) => $"@Nombre{i}"));
            var query = $"DELETE FROM Empleados WHERE Nombre IN ({inClause})";
            using (var cmd = new SqlCommand(query, conn))
            {
                for (int i = 0; i < nombres.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@Nombre{i}", nombres[i]);
                }

                cmd.ExecuteNonQuery();
            }

            swGlobal.Stop();

            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Delete",
                TipoAcceso = "SQL Directa",
                Entidad = "Empleado",
                IdAfectado = null,
                TiempoMs = swGlobal.ElapsedMilliseconds,
                Ruta = "/EmpleadosSql/EliminarMasivoSql",
                Metodo = "POST",
                Observacion = "Eliminación masiva de empleados predefinidos usando SQL directa"
            });
        }
    }

    return RedirectToAction(nameof(Index));
}

[HttpPost]
public IActionResult EditarMasivoSql(bool auditoriaIndividual = false)
{
    var nombres = new List<string>
    {
        "Juan Pérez", "Ana López", "Carlos García", "María Fernández", "Luis Torres", "Carmen Díaz", "Roberto Sánchez", "Lucía Gómez",
        "Alberto Ruiz", "Natalia Vargas", "Fernando Morales", "Javier Soto", "Laura Castillo", "Sofía Romero", "Diego Herrera", "Marta Peña",
        "Iván Castro", "Valeria Salinas", "Patricia Reyes", "Héctor Flores", "Gabriela Mejía", "Tomás Rivas", "Esteban León", "Daniela Pineda",
        "Francisco Solís", "Marcela Cortés", "Joaquín Molina", "Nicolás Duarte", "Andrea Lozano", "Emilio Aguirre", "Julia Esquivel", "Bruno Cordero",
        "Isabel Tapia", "Manuel Duarte", "Ximena Ríos", "Ricardo Palma", "Victoria Cárdenas", "Álvaro Pinto", "Paula Zamora", "Camilo Medina"
    };

    var puestos = new List<string>
    {
        "Analista", "Arquitecto", "Consultor", "Líder Técnico", "Desarrollador", "Tester QA",
        "Scrum Master", "DevOps", "Gerente", "Soporte Técnico"
    };

    var random = new Random();
    var swGlobal = System.Diagnostics.Stopwatch.StartNew();

    using (var conn = new SqlConnection(_config.GetConnectionString("RemoteConnection")))
    {
        conn.Open();

        if (auditoriaIndividual)
        {
            foreach (var nombre in nombres)
            {
                var nuevoPuesto = puestos[random.Next(puestos.Count)];
                var nuevoSalario = random.Next(40000, 90000);

                var sw = System.Diagnostics.Stopwatch.StartNew();

                var query = "UPDATE Empleados SET Puesto = @Puesto, Salario = @Salario WHERE Nombre = @Nombre";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Puesto", nuevoPuesto);
                    cmd.Parameters.AddWithValue("@Salario", nuevoSalario);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.ExecuteNonQuery();
                }

                sw.Stop();

                AuditoriaStore.Registros.Add(new AuditoriaRegistro
                {
                    FechaHora = DateTime.Now,
                    Accion = "Update",
                    TipoAcceso = "SQL Directa",
                    Entidad = "Empleado",
                    IdAfectado = null,
                    TiempoMs = sw.ElapsedMilliseconds,
                    Ruta = "/EmpleadosSql/EditarMasivoSql",
                    Metodo = "POST",
                    Observacion = $"Edición individual de '{nombre}' con nuevo puesto y salario"
                });
            }
        }
        else
        {
            foreach (var nombre in nombres)
            {
                var nuevoPuesto = puestos[random.Next(puestos.Count)];
                var nuevoSalario = random.Next(40000, 90000);

                var query = "UPDATE Empleados SET Puesto = @Puesto, Salario = @Salario WHERE Nombre = @Nombre";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Puesto", nuevoPuesto);
                    cmd.Parameters.AddWithValue("@Salario", nuevoSalario);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.ExecuteNonQuery();
                }
            }

            swGlobal.Stop();

            AuditoriaStore.Registros.Add(new AuditoriaRegistro
            {
                FechaHora = DateTime.Now,
                Accion = "Update",
                TipoAcceso = "SQL Directa",
                Entidad = "Empleado",
                IdAfectado = null,
                TiempoMs = swGlobal.ElapsedMilliseconds,
                Ruta = "/EmpleadosSql/EditarMasivoSql",
                Metodo = "POST",
                Observacion = "Edición masiva aleatoria de empleados usando SQL directa"
            });
        }
    }

    return RedirectToAction(nameof(Index));
}


    }
}
