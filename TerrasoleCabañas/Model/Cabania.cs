﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TerrasoleCabañas.Model
{
    public class Cabania
    {
        public Cabania() { }
        [Key]
        public int Id { get; set; }
        [Required]
        public int Categoria { get; set; }
        [Required]
        public int Capacidad { get; set; }
        [Required] 
        public decimal PrecioPorDia { get; set; }
    
    }
}
