namespace ServicioItemsTrabajo.Models
{
    //Aquí se mantienen las reglas de distribución que afectan al algoritmo de asignación
    public class DistributionOptions
    {
        public int HighRelevanceSaturationLimit { get; set; } = 3;
    }
}
