namespace FlexSpace.BLL
{
    public class Negocio
    {
        public enum TipoCliente
        {
            Estandar,
            VIP
        }
        public enum TipoPuesto
        {
            EscritorioIndividual,
            SalaReuniones,
            CabinaPrivada
        }
        public enum Estado
        {
            Confirmada, Cancelada, Finalizada
        }
        public class Cliente
        {
            public int Id;
            public string Nombre;
            public string Email;
            public TipoCliente TipoCliente;
            public int SancionesActivas;
        }
        public class Puesto
        {
            public int Id;
            public string Codigo;
            public TipoPuesto TipoPuesto;
            public decimal TarifaBasePorHora;
        }
        public class Reserva
        {
            public int Id;
            public int ClienteId;
            public int PuestoId;
            public DateTime FechaInicio;
            public DateTime FechaFin;
            public Estado Estado;
            public decimal CostoTotal;
        }
    }
}
