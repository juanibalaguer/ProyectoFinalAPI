using System.ComponentModel.DataAnnotations;

namespace TerrasoleCabañas.Model
{
    public class Producto_Servicio
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        [Required]
        public byte Consumible { get; set; }
        public decimal Precio { get; set; }
        public string Foto { get; set; }

    }
}
