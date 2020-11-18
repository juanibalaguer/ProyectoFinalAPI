using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TerrasoleCabañas.Model
{
    public enum estadoPedido
    {
        Borrador = 0, // El inquilino está armando un pedido
        Confirmado = 1, 
        EnPreparacion = 2,
        Entregado = 3
    }
    public class Pedido
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime FechaPedido { get; set; }
        [Required]
        public int Estado { get; set; }
        [Required]
        public decimal MontoPedido { get; set; }
        [Required]
        public int EstadiaId { get; set; }

        public string NombreEstado()
        {
            return Estado >= 0 ? ((estadoPedido) Estado).ToString() : "";
        }
        public static IDictionary<int, string> ObtenerEstados()
        {
            SortedDictionary<int, string> estadoPedidos = new SortedDictionary<int, string>();
            Type tipoPedido = typeof(estadoPedido);
            foreach (var valor in Enum.GetValues(tipoPedido))
            {
                estadoPedidos.Add((int)valor, Enum.GetName(tipoPedido, valor));
            }
            return estadoPedidos;
        }
    }
}
