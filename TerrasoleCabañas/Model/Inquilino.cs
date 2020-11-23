using System.ComponentModel.DataAnnotations;

namespace TerrasoleCabañas.Model
{
    public class Inquilino
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string DNI { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Telefono { get; set; }
    }
}
