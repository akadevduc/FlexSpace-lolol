using FlexSpace.DAL;
using FlexSpace.Entidades;

namespace FlexSpace.BLL
{
    public class Negocio
    {
        private ClienteDatos _clienteDatos = new ClienteDatos();
        private PuestoDatos _puestoDatos = new PuestoDatos();
        private ReservaDatos _reservaDatos = new ReservaDatos();

        private const int SancionesParaBloqueo = 3;
        private const double HorasMinimasParaDescuento = 5;
        private const double HorasLimiteCancelacionTardia = 2;
        public class ClienteSancionadoException : Exception
        {
            public ClienteSancionadoException()
                : base("El cliente está sancionado 3 veces o más y no puede realizar reservas.")
            { }
        }

        public Cliente GetClientePorId(int id)
        {
            Cliente cliente = _clienteDatos.BuscarPorId(id);

            if (cliente == null)
            {
                throw new InvalidOperationException($"No existe el cliente con ID {id}.");
            }

            return cliente;
        }

        public Puesto GetPuestoPorId(int id)
        {
            Puesto puesto = _puestoDatos.BuscarPorId(id);

            if (puesto == null)
            {
                throw new InvalidOperationException($"No existe el puesto con ID {id}.");
            }

            return puesto;
        }

        public Reserva GetReservaPorId(int id)
        {
            Reserva reserva = _reservaDatos.BuscarPorId(id);

            if (reserva == null)
            {
                throw new InvalidOperationException($"No existe la reserva con ID {id}.");
            }

            return reserva;
        }

        //CASO 1
        public Reserva Cotizar(int clienteId, int puestoId, DateTime fechaInicio, DateTime fechaFin)
        {
            Cliente cliente = GetClientePorId(clienteId);
            Puesto puesto = GetPuestoPorId(puestoId);

            Validar(cliente, puestoId, fechaInicio, fechaFin);

            Reserva reserva = new Reserva
            {
                ClienteId = clienteId,
                PuestoId = puestoId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Estado = Estado.Confirmada
            };
            reserva.CostoTotal = CalcularCosto(reserva, cliente, puesto);

            return reserva;
        }
        public void RegistrarReserva(Reserva reserva)
        {
            Cliente cliente = GetClientePorId(reserva.ClienteId);
            Puesto puesto = GetPuestoPorId(reserva.PuestoId);

            Validar(cliente, reserva.PuestoId, reserva.FechaInicio, reserva.FechaFin);

            reserva.Estado = Estado.Confirmada;
            reserva.CostoTotal = CalcularCosto(reserva, cliente, puesto);
            if (!_reservaDatos.Add(reserva))
            {
                throw new InvalidOperationException("No se pudo guardar la reserva.");
            }
        }

        //CASO 2
        public bool CancelarReserva(int reservaId)
        {
            Reserva reserva = GetReservaPorId(reservaId);

            if (reserva.Estado != Estado.Confirmada)
            {
                throw new InvalidOperationException("Solo se pueden cancelar reservas confirmadas.");
            }

            bool tardia = EsCancelacionTardia(reserva, DateTime.Now);

            if (!_reservaDatos.Cancelar(reserva, tardia))
            {
                throw new InvalidOperationException("No se pudo cancelar la reserva (ya no estaba confirmada).");
            }

            return tardia;
        }
        //CASO 3
        public List<Reserva> ObtenerReservasActivasPorPuesto(string idPuesto)
        {
            Puesto puesto = _puestoDatos.BuscarPorCodigo(idPuesto);

            if (puesto == null)
            {
                throw new InvalidOperationException($"No existe el puesto con código '{idPuesto}'.");
            }

            return _reservaDatos.ListarConfirmadasFuturasPorPuesto(puesto.Id, DateTime.Now);
        }
        //CASO 4
        public List<Cliente> ObtenerClientesSancionados()
        {
            return _clienteDatos.ListarSancionados();
        }

        //GENERAL
        private void Validar(Cliente cliente, int puestoId, DateTime fechaInicio, DateTime fechaFin)
        {
            if (EstaBloqueado(cliente))
            {
                throw new ClienteSancionadoException();
            }
            if (fechaFin <= fechaInicio)
            {
                throw new InvalidOperationException("La fecha de fin debe ser posterior a la de inicio.");
            }
            if (fechaInicio < DateTime.Now)
            {
                throw new InvalidOperationException("No se puede reservar en una fecha y hora pasada.");
            }
            if (_reservaDatos.ExisteSolapamiento(puestoId, fechaInicio, fechaFin))
            {
                throw new InvalidOperationException("El puesto ya tiene una reserva confirmada que se solapa con ese horario.");
            }
        }
        //REGLAS DE NEGOCIO
        private bool EstaBloqueado(Cliente cliente)
        {
            return cliente.SancionesActivas >= SancionesParaBloqueo;
        }

        private double CalcularHoras(Reserva reserva)
        {
            return (reserva.FechaFin - reserva.FechaInicio).TotalHours;
        }

        private decimal CalcularCosto(Reserva reserva, Cliente cliente, Puesto puesto)
        {
            double horas = CalcularHoras(reserva);
            decimal costo = (decimal)horas * puesto.TarifaBasePorHora;

            if (IncluyeFinDeSemana(reserva))
            {
                costo += costo * 0.15m;
            }
            if (cliente.SancionesActivas > 0)
            {
                costo += costo * 0.20m;
                return costo;
            }
            if (horas >= HorasMinimasParaDescuento)
            {
                costo -= costo * 0.10m;
            }
            if (cliente.TipoCliente == TipoCliente.VIP)
            {
                costo -= costo * 0.05m;
            }
            return costo;
        }

        private bool EsCancelacionTardia(Reserva reserva, DateTime ahora)
        {
            return (reserva.FechaInicio - ahora).TotalHours < HorasLimiteCancelacionTardia; // true si es sancionable
        }

        private bool IncluyeFinDeSemana(Reserva reserva)
        {
            DateTime dia = reserva.FechaInicio.Date;
            DateTime ultimoDia = reserva.FechaFin.AddTicks(-1).Date; // para que no cuente justo las 12 de la noche del último día

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