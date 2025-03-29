namespace EMPLOYEE_MANAGER.Services
{
    public class AuditoriaRegistro
    {
        public DateTime FechaHora { get; set; }
        public string Accion { get; set; }
        public string TipoAcceso { get; set; }
        public string Entidad { get; set; }
        public int? IdAfectado { get; set; }
        public long TiempoMs { get; set; }
        public string Ruta { get; set; }
        public string Metodo { get; set; }
        public string Observacion { get; set; }
    }
}
