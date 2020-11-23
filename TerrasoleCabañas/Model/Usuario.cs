using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TerrasoleCabañas.Model
{
    public enum roles
    {
        Administrador = 1,
        Empleado = 2,
        Inquilino = 3,
    }
    public class Usuario
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Contraseña { get; set; }
        [Required]
        public int Rol { get; set; }

        public string NombreRol()
        {
            return Rol > 0 ? ((roles)Rol).ToString() : "";
        }
        public static IDictionary<int, string> ObtenerRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoRol = typeof(roles);
            foreach (var valor in Enum.GetValues(tipoRol))
            {
                roles.Add((int)valor, Enum.GetName(tipoRol, valor));
            }
            return roles;
        }
    }
}
