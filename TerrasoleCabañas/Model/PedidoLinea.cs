using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TerrasoleCabañas.Model
{
    public class PedidoLinea
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PedidoId { get; set; }
        [Required]
        public int Producto_ServicioId { get; set; }
        [Required]
        public Producto_Servicio Producto_Servicio { get; set; }
        [Required]
        public decimal PrecioPorUnidad { get; set; }
        [Required] 
        public int Cantidad { get; set; }
    }
}
