using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AlquimiaEsencial.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Relación con usuario de Identity
        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; }
    }
}
