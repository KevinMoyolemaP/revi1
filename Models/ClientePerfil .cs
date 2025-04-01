using Microsoft.AspNetCore.Identity;

namespace AlquimiaEsencial.Models
{
    public class ClientePerfil
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }

        public string Direccion { get; set; }
        public string Telefono { get; set; }
    }
}
