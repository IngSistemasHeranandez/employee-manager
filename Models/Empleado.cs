using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMPLOYEE_MANAGER.Models
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El puesto es obligatorio")]
        [StringLength(50)]
        public string Puesto { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaContratacion { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000, ErrorMessage = "El salario debe estar entre 0 y 1,000,000")]
        public decimal Salario { get; set; }
    }

}
