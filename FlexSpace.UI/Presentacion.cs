namespace FlexSpace.UI
{
    internal class Presentacion
    {
        static void Main(string[] args)
        {
            MostrarMenu();
        }
        static void MostrarMenu()
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
                        RegistrarNuevaReserva();
                        break;

                    case 2:
                        CancelarReserva();
                        break;

                    case 3:
                        ConsultarReservasActivasPorPuesto();
                        break;

                    case 4:
                        ListarClientesSancionados();
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
        static void RegistrarNuevaReserva()
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

            // Reserva reserva = reservaBLL.CalcularReserva(
            //     clienteId,
            //     puestoId,
            //     fechaInicio,
            //     fechaFin
            // );

            Console.WriteLine();
            Console.WriteLine("=== RESUMEN DE LA RESERVA ===");
            Console.WriteLine($"Cliente: {clienteId}");
            Console.WriteLine($"Puesto: {puestoId}");
            Console.WriteLine($"Inicio: {fechaInicio}");
            Console.WriteLine($"Fin: {fechaFin}");

            // Ejemplo:
            // Console.WriteLine($"Precio: ${reserva.Precio}");

            Console.WriteLine();
            Console.Write("¿Confirmar reserva? (S/N): ");

            string confirmacion = Console.ReadLine();

            if (confirmacion?.ToUpper() == "S")
            {
                // reservaBLL.Registrar(reserva);

                Console.WriteLine("Reserva registrada correctamente.");
            }
            else
            {
                Console.WriteLine("Reserva cancelada.");
            }

            Console.ReadKey();
        }


        static void CancelarReserva()
        {
            Console.Clear();
            Console.WriteLine("=== CANCELAR RESERVA ===");

            Console.Write("Ingrese el ID de la reserva: ");
            int reservaId = int.Parse(Console.ReadLine());

            // La BLL debería encargarse de:
            // - Buscar la reserva
            // - Verificar si puede cancelarse
            // - Calcular cuánto falta para el inicio
            // - Si faltan menos de 2 horas, incrementar SancionesActivas
            // - Cancelar la reserva en la BD

            // reservaBLL.CancelarReserva(reservaId);

            Console.WriteLine();
            Console.WriteLine("Reserva cancelada correctamente.");

            Console.ReadKey();
        }


        static void ConsultarReservasActivasPorPuesto()
        {
            Console.Clear();
            Console.WriteLine("=== RESERVAS ACTIVAS POR PUESTO ===");

            Console.Write("Ingrese el código del puesto: ");
            string codigoPuesto = Console.ReadLine();

            // List<Reserva> reservas =
            //     reservaBLL.ObtenerReservasActivasPorPuesto(codigoPuesto);

            Console.WriteLine();
            Console.WriteLine($"Reservas futuras del puesto {codigoPuesto}:");
            Console.WriteLine("--------------------------------------------");

            // foreach (Reserva reserva in reservas)
            // {
            //     Console.WriteLine(
            //         $"Reserva: {reserva.Id} | " +
            //         $"Inicio: {reserva.FechaInicio} | " +
            //         $"Fin: {reserva.FechaFin}"
            //     );
            // }

            Console.ReadKey();
        }

        static void ListarClientesSancionados()
        {
            static void ListarClientesSancionados()
            {
                Console.Clear();
                Console.WriteLine("=== CLIENTES SANCIONADOS ===");

                // List<Cliente> clientes =
                //     clienteBLL.ObtenerClientesSancionados();

                // foreach (Cliente cliente in clientes)
                // {
                //     Console.WriteLine(
                //         $"ID: {cliente.Id} | " +
                //         $"Nombre: {cliente.Nombre} | " +
                //         $"Sanciones: {cliente.SancionesActivas}"
                //     );
                // }

                Console.ReadKey();
            }
        }
    }
}//despues termino con negocio
