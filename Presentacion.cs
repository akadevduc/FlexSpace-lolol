using FlexSpace.Entidades;
using FlexSpace.BLL;

namespace FlexSpace.UI
{
    internal class Presentacion
    {
        static void Main(string[] args)
        {
            Negocio negocio;

            try
            {
                negocio = new Negocio();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }

            MostrarMenu(negocio);
        }
        static void MostrarMenu(Negocio negocio)
        {
            int op;

            do
            {
                Console.Clear();

                Console.WriteLine("====================================");
                Console.WriteLine("       SISTEMA DE RESERVAS");
                Console.WriteLine("====================================");
                Console.WriteLine("1. Registrar Nueva Reserva");
                Console.WriteLine("2. Cancelar Reserva");
                Console.WriteLine("3. Consultar Reservas Activas por Puesto");
                Console.WriteLine("4. Listar Clientes Sancionados");
                Console.WriteLine("0. Salir");
                Console.WriteLine("====================================");
                Console.Write("Seleccione una opción: ");

                if (!int.TryParse(Console.ReadLine(), out op))
                {
                    Console.WriteLine("Opción inválida.");
                    Console.ReadKey();
                    continue;
                }

                switch (op)
                {
                    case 1:
                        RegistrarNuevaReserva(negocio);
                        break;

                    case 2:
                        CancelarReserva(negocio);
                        break;

                    case 3:
                        ConsultarReservasActivasPorPuesto(negocio);
                        break;

                    case 4:
                        ListarClientesSancionados(negocio);
                        break;

                    case 0:
                        Console.WriteLine("Saliendo del sistema...");
                        break;

                    default:
                        Console.WriteLine("Opción inválida.");
                        Console.ReadKey();
                        break;
                }

            } while (op != 0);
        }
        static void RegistrarNuevaReserva(Negocio negocio)
        {
            Console.Clear();
            Console.WriteLine("=== REGISTRAR NUEVA RESERVA ===");

            Console.Write("Cliente ID: ");
            int clienteId = int.Parse(Console.ReadLine());

            Console.Write("Puesto ID: ");
            int puestoId = int.Parse(Console.ReadLine());

            Console.Write("Fecha y hora de inicio (dd/MM/aaaa HH:mm): ");
            DateTime fechaInicio = DateTime.Parse(Console.ReadLine());

            Console.Write("Fecha y hora de fin (dd/MM/aaaa HH:mm): ");
            DateTime fechaFin = DateTime.Parse(Console.ReadLine());

            Reserva NuevaReserva = negocio.Cotizar(clienteId, puestoId, fechaInicio, fechaFin);

            Console.WriteLine();
            Console.WriteLine("=== RESUMEN DE LA RESERVA ===");
            Console.WriteLine($"Cliente: {NuevaReserva.ClienteId}");
            Console.WriteLine($"Puesto: {NuevaReserva.PuestoId}");
            Console.WriteLine($"Inicio: {NuevaReserva.FechaInicio}");
            Console.WriteLine($"Fin: {NuevaReserva.FechaFin}");
            Console.WriteLine($"Precio: ${NuevaReserva.CostoTotal}");

            Console.WriteLine();
            Console.Write("¿Confirmar reserva? (S/N): ");

            string confirmacion = Console.ReadLine();

            if (confirmacion?.ToUpper() == "S")
            {
                negocio.RegistrarReserva(NuevaReserva);

                Console.WriteLine("Reserva registrada correctamente.");
            }
            else
            {
                Console.WriteLine("Reserva cancelada.");
            }

            Console.ReadKey();
        }


        static void CancelarReserva(Negocio negocio)
        {
            Console.Clear();
            Console.WriteLine("=== CANCELAR RESERVA ===");

            Console.Write("Ingrese el ID de la reserva: ");
            int reservaId = int.Parse(Console.ReadLine());

            negocio.CancelarReserva(reservaId);

            Console.WriteLine();
            Console.WriteLine("Reserva cancelada correctamente.");

            Console.ReadKey();
        }


        static void ConsultarReservasActivasPorPuesto(Negocio negocio)
        {
            Console.Clear();
            Console.WriteLine("=== RESERVAS ACTIVAS POR PUESTO ===");

            Console.Write("Ingrese el id del puesto: ");
            string idPuesto = Console.ReadLine();

            List<Reserva> reservas = negocio.ObtenerReservasActivasPorPuesto(idPuesto);

            Console.WriteLine();
            Console.WriteLine($"Reservas futuras del puesto {idPuesto}:");
            Console.WriteLine("--------------------------------------------");

            foreach (Reserva reserva in reservas)
            {
                Console.WriteLine(
                    $"Reserva: {reserva.Id} | " +
                    $"Inicio: {reserva.FechaInicio} | " +
                    $"Fin: {reserva.FechaFin}"
                );
            }

            Console.ReadKey();
        }

        static void ListarClientesSancionados(Negocio negocio)
        {

            Console.Clear();
            Console.WriteLine("=== CLIENTES SANCIONADOS ===");

            List<Cliente> clientes = negocio.ObtenerClientesSancionados();

            foreach (Cliente cliente in clientes)
            {
                Console.WriteLine(
                    $"ID: {cliente.Id} | " +
                    $"Nombre: {cliente.Nombre} | " +
                    $"Sanciones: {cliente.SancionesActivas}"
                );
            }

            Console.ReadKey();
            
        }
    }
}
