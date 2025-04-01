using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AlquimiaEsencial.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string UsuarioId { get; set; }  // Puede ser null para visitantes
        public IdentityUser Usuario { get; set; }

        [Required]
        [Display(Name = "Nombre del Cliente")]
        public string NombreCliente { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        public bool Aprobado { get; set; } = false;

        // ✅ NUEVO: Relación con el producto pedido (opcional)
        public int? ProductoId { get; set; }  // Puede ser null si se pidió desde contacto general
        public Producto Producto { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; }
    }
}
