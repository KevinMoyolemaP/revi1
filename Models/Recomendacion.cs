namespace AlquimiaEsencial.Models
{
    public class Recomendacion
    {
        public int Id { get; set; }

        public string Sintoma { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
    }
}
