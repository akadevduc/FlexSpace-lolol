using FlexSpace.Entidades;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace FlexSpace.DAL
{
    public abstract class DatosBase
    {
        private const string NombreBase = "FlexSpace";
        private static bool _baseLista = false;

        private string _conexionStringSinBase = "Server=localhost;Uid=root;Pwd=;";
        protected string _conexionString = $"Server=localhost;Database={NombreBase};Uid=root;Pwd=;";

        protected DatosBase()
        {
            if (_baseLista) return;

            try
            {
                InicializarBaseDeDatos();
                _baseLista = true;
            }
            catch (MySqlException ex)
            {
                throw new Exception("No se pudo conectar a la base de datos. ¿Está prendido XAMPP/MySQL? Detalle: " + ex.Message, ex);
            }
        }

        private void InicializarBaseDeDatos()
        {
            // se conecta a mysql sin base de datos para crearla si no existe
            using (MySqlConnection conexion = new MySqlConnection(_conexionStringSinBase))
            {
                conexion.Open();

                using (MySqlCommand comando = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {NombreBase}", conexion))
                {
                    comando.ExecuteNonQuery();
                }
            }

            // se conecta a la base de datos para crear las tablas si no existen
            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                conexion.Open();

                string crearCliente = @"
                    CREATE TABLE IF NOT EXISTS cliente (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        Nombre VARCHAR(100) NOT NULL,
                        Email VARCHAR(100) NOT NULL,
                        TipoCliente VARCHAR(20) NOT NULL,
                        SancionesActivas INT NOT NULL DEFAULT 0
                    )";

                string crearPuesto = @"
                    CREATE TABLE IF NOT EXISTS puesto (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        Codigo VARCHAR(20) NOT NULL UNIQUE,
                        TipoPuesto VARCHAR(30) NOT NULL,
                        TarifaBasePorHora DECIMAL(10,2) NOT NULL
                    )";

                string crearReserva = @"
                    CREATE TABLE IF NOT EXISTS reserva (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        ClienteId INT NOT NULL,
                        PuestoId INT NOT NULL,
                        FechaInicio DATETIME NOT NULL,
                        FechaFin DATETIME NOT NULL,
                        Estado VARCHAR(20) NOT NULL,
                        CostoTotal DECIMAL(12,2) NOT NULL,
                        FOREIGN KEY (ClienteId) REFERENCES cliente(Id),
                        FOREIGN KEY (PuestoId) REFERENCES puesto(Id)
                    )";

                foreach (string sql in new string[] { crearCliente, crearPuesto, crearReserva })
                {
                    using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }
                }

                CargarDatosDePrueba(conexion);
            }
        }

        // datos de prueba para poder probar la app
        private void CargarDatosDePrueba(MySqlConnection conexion)
        {
            using (MySqlCommand contar = new MySqlCommand("SELECT COUNT(*) FROM cliente", conexion))
            {
                if (Convert.ToInt32(contar.ExecuteScalar()) > 0) return;
            }

            string clientes = "INSERT INTO cliente (Nombre, Email, TipoCliente, SancionesActivas) VALUES " +
                              "('Ana Pérez', 'ana@mail.com', 'Estandar', 0), " +
                              "('Bruno Gómez', 'bruno@mail.com', 'VIP', 0), " +
                              "('Carla Díaz', 'carla@mail.com', 'Estandar', 1), " +
                              "('Diego Ruiz', 'diego@mail.com', 'Estandar', 3)";

            string puestos = "INSERT INTO puesto (Codigo, TipoPuesto, TarifaBasePorHora) VALUES " +
                             "('ESC-01', 'EscritorioIndividual', 1000.00), " +
                             "('SAL-01', 'SalaReuniones', 3500.00), " +
                             "('CAB-01', 'CabinaPrivada', 2000.00)";

            foreach (string sql in new string[] { clientes, puestos })
            {
                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.ExecuteNonQuery();
                }
            }
        }
    }


    public class ClienteDatos : DatosBase
    {

        private const string ColumnasSelect = "Id, Nombre, Email, TipoCliente, SancionesActivas";

        private Cliente LeerFila(MySqlDataReader reader)
        {
            return new Cliente
            {
                Id = Convert.ToInt32(reader["Id"]),
                Nombre = reader["Nombre"].ToString(),
                Email = reader["Email"].ToString(),
                TipoCliente = Enum.Parse<TipoCliente>(reader["TipoCliente"].ToString()),
                SancionesActivas = Convert.ToInt32(reader["SancionesActivas"])
            };
        }

        public Cliente BuscarPorId(int id)
        {
            string query = $"SELECT {ColumnasSelect} FROM cliente WHERE Id = @Id";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", id);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return LeerFila(reader);
                    }
                }
            }

            return null;
        }

        public List<Cliente> ListarSancionados()
        {
            var resultados = new List<Cliente>();
            string query = $"SELECT {ColumnasSelect} FROM cliente WHERE SancionesActivas > 0";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultados.Add(LeerFila(reader));
                    }
                }
            }

            return resultados;
        }
    }

    public class PuestoDatos : DatosBase
    {
        private const string ColumnasSelect = "Id, Codigo, TipoPuesto, TarifaBasePorHora";

        private Puesto LeerFila(MySqlDataReader reader)
        {
            return new Puesto
            {
                Id = Convert.ToInt32(reader["Id"]),
                Codigo = reader["Codigo"].ToString(),
                TipoPuesto = Enum.Parse<TipoPuesto>(reader["TipoPuesto"].ToString()),
                TarifaBasePorHora = Convert.ToDecimal(reader["TarifaBasePorHora"])
            };
        }

        public Puesto BuscarPorId(int id)
        {
            string query = $"SELECT {ColumnasSelect} FROM puesto WHERE Id = @Id";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", id);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return LeerFila(reader);
                    }
                }
            }

            return null;
        }

        public Puesto BuscarPorCodigo(string codigo)
        {
            string query = $"SELECT {ColumnasSelect} FROM puesto WHERE Codigo = @Codigo";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Codigo", codigo);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return LeerFila(reader);
                    }
                }
            }

            return null;
        }
    }

    public class ReservaDatos : DatosBase
    {
        private const string ColumnasSelect = "Id, ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal";

        private Reserva LeerFila(MySqlDataReader reader)
        {
            return new Reserva
            {
                Id = Convert.ToInt32(reader["Id"]),
                ClienteId = Convert.ToInt32(reader["ClienteId"]),
                PuestoId = Convert.ToInt32(reader["PuestoId"]),
                FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                FechaFin = Convert.ToDateTime(reader["FechaFin"]),
                Estado = Enum.Parse<Estado>(reader["Estado"].ToString()),
                CostoTotal = Convert.ToDecimal(reader["CostoTotal"])
            };
        }

        public Reserva BuscarPorId(int id)
        {
            string query = $"SELECT {ColumnasSelect} FROM reserva WHERE Id = @Id";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", id);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return LeerFila(reader);
                    }
                }
            }

            return null;
        }

        public bool ExisteSolapamiento(int puestoId, DateTime inicio, DateTime fin)
        {
            string query = "SELECT COUNT(*) FROM reserva " +
                           "WHERE PuestoId = @PuestoId AND Estado = 'Confirmada' " +
                           "AND FechaInicio < @Fin AND FechaFin > @Inicio";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@PuestoId", puestoId);
                comando.Parameters.AddWithValue("@Inicio", inicio);
                comando.Parameters.AddWithValue("@Fin", fin);

                conexion.Open();

                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }


        public bool Add(Reserva reserva)
        {
            string query = "INSERT INTO reserva (ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal) " +
                           "VALUES (@ClienteId, @PuestoId, @FechaInicio, @FechaFin, @Estado, @CostoTotal)";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@ClienteId", reserva.ClienteId);
                comando.Parameters.AddWithValue("@PuestoId", reserva.PuestoId);
                comando.Parameters.AddWithValue("@FechaInicio", reserva.FechaInicio);
                comando.Parameters.AddWithValue("@FechaFin", reserva.FechaFin);
                comando.Parameters.AddWithValue("@Estado", reserva.Estado.ToString());
                comando.Parameters.AddWithValue("@CostoTotal", reserva.CostoTotal);

                conexion.Open();

                int filasAfectadas = comando.ExecuteNonQuery();

                if (filasAfectadas > 0)
                {
                    reserva.Id = (int)comando.LastInsertedId;
                }

                return filasAfectadas > 0;
            }
        }


        public List<Reserva> ListarConfirmadasFuturasPorPuesto(int puestoId, DateTime desde)
        {
            var resultados = new List<Reserva>();

            string query = $"SELECT {ColumnasSelect} FROM reserva " +
                           "WHERE PuestoId = @PuestoId AND Estado = 'Confirmada' AND FechaInicio > @Desde " +
                           "ORDER BY FechaInicio";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@PuestoId", puestoId);
                comando.Parameters.AddWithValue("@Desde", desde);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultados.Add(LeerFila(reader));
                    }
                }
            }

            return resultados;
        }


        public bool Cancelar(Reserva reserva, bool sancionarCliente)
        {
            string queryReserva = "UPDATE reserva SET Estado = 'Cancelada' WHERE Id = @Id AND Estado = 'Confirmada'";
            string queryCliente = "UPDATE cliente SET SancionesActivas = SancionesActivas + 1 WHERE Id = @ClienteId";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                conexion.Open();

                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand comandoReserva = new MySqlCommand(queryReserva, conexion, transaccion);
                        comandoReserva.Parameters.AddWithValue("@Id", reserva.Id);

                        int filasReserva = comandoReserva.ExecuteNonQuery();

                        if (filasReserva == 0)
                        {
                            transaccion.Rollback();
                            return false;
                        }

                        if (sancionarCliente)
                        {
                            MySqlCommand comandoCliente = new MySqlCommand(queryCliente, conexion, transaccion);
                            comandoCliente.Parameters.AddWithValue("@ClienteId", reserva.ClienteId);
                            comandoCliente.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}