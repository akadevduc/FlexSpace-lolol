namespace FlexSpace.Entidades
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

        public bool Bloqueado
        {
            get
            {
                return SancionesActivas >= 3;
            }
        }
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

        public double HorasReservadas
        {
            get
            {
                return (FechaFin - FechaInicio).TotalHours;
            }
        }

        public decimal CalcularReserva(Cliente cliente, Puesto puesto)
        {
            CostoTotal = (decimal)HorasReservadas * puesto.TarifaBasePorHora;

            if (IncluyeFinDeSemana())
            {
                CostoTotal += CostoTotal * 0.15m;
            }
            if (cliente.SancionesActivas > 0)
            {
                CostoTotal += CostoTotal * 0.20m;
                return CostoTotal;
            }
            if (HorasReservadas >= 5)
            {
                CostoTotal -= CostoTotal * 0.10m;
            }
            if (cliente.TipoCliente == TipoCliente.VIP)
            {
                CostoTotal -= CostoTotal * 0.05m;
            }
            return CostoTotal;
        }

        public bool CancelacionTardia(DateTime ahora)
        {
            return (FechaInicio - ahora).TotalHours < 2; // true si es sancionable
        }

        private bool IncluyeFinDeSemana()
        {
            DateTime dia = FechaInicio.Date;
            DateTime ultimoDia = FechaFin.AddTicks(-1).Date;//para que no cuente justo las 12 de la noche del último día

            while (dia <= ultimoDia)
            {
                if (dia.DayOfWeek == DayOfWeek.Saturday ||
                    dia.DayOfWeek == DayOfWeek.Sunday)
                {
                    return true;
                }

                dia = dia.AddDays(1);
            }

            return false;
        }
    }
}