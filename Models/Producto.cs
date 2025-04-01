using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquimiaEsencial.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        public string ImagenUrl { get; set; }

        public int Stock { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relación con categoría
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public ICollection<DetalleVenta> DetallesVenta { get; set; }
        [NotMapped]
        public IFormFile ImagenArchivo { get; set; }

    }
}
