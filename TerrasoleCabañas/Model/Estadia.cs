using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TerrasoleCabañas.Model
{
    public class Estadia
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CabaniaId { get; set; }
        public Cabania Cabania { get; set; }
        [Required]
        public int InquilinoId { get; set; }
        public Inquilino Inquilino { get; set; }
        [Required]
        public DateTime FechaDesde { get; set; }
        [Required] 
        public DateTime FechaHasta { get; set; }
        [Required]
        public decimal MontoTotal { get; set; }
        [Required]
        public int Personas { get; set; }


    }
}
