using System.ComponentModel.DataAnnotations;

namespace AlquimiaEsencial.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Debe pedir al menos 1 unidad.")]
        public int Cantidad { get; set; }
    }
}
