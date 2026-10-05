using FlexSpace.DAL;
using FlexSpace.Entidades;

namespace FlexSpace.BLL
{
    public class Negocio
    {
        private ClienteDatos _clienteDatos = new ClienteDatos();
        private PuestoDatos _puestoDatos = new PuestoDatos();
        private ReservaDatos _reservaDatos = new ReservaDatos();

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
            reserva.CalcularReserva(cliente, puesto);

            return reserva;
        }
        public void RegistrarReserva(Reserva reserva)
        {
            Cliente cliente = GetClientePorId(reserva.ClienteId);
            Puesto puesto = GetPuestoPorId(reserva.PuestoId);

            Validar(cliente, reserva.PuestoId, reserva.FechaInicio, reserva.FechaFin);

            reserva.Estado = Estado.Confirmada;
            reserva.CalcularReserva(cliente, puesto);
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

            bool tardia = reserva.CancelacionTardia(DateTime.Now);

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
            if (cliente.Bloqueado)
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
    }
}