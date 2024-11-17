using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;
using Sistema_Salud;

namespace Sistema_Salud
{
    
    public partial class Turnos : Window
    {
        DataClasses1DataContext dataContex; // Declaración de la variable
        private PacienteTurno pacienteTraslado;
        public Turnos()
        {
            
            InitializeComponent();
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Sistema_Salud.Properties.Settings.SistemaSaludConnectionString"].ConnectionString;
                dataContex = new DataClasses1DataContext(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener la cadena de conexión: {ex.Message}");
            }
        }
        public class PacienteTurno
        {
            public int ID { get; set; }
            public string DNI { get; set; }
            public string NombrePaciente { get; set; }
            public string ApellidoPaciente { get; set; }
            public string NombreMedico { get; set; }
            public string ApellidoMedico { get; set; }
            public string Especialidad { get; set; }
            public string Motivo { get; set; }
            public string Fecha {  get; set; }
            public DateTime Hora { get; set; }
        }
        public void Mostrar_Turnos_Pacientes(object sender, RoutedEventArgs e)
        {

            try
            {
                // Obtener la fecha seleccionada del DatePicker
                DateTime? fechaSeleccionada = dia_turno.SelectedDate;
                // Verificar si se ha seleccionado una fecha
                if (fechaSeleccionada.HasValue)
                {
                    // Consulta con LINQ para unir las tablas
                    var consulta = from p in dataContex.Pacientes
                                   join c in dataContex.Citas on p.PacienteID equals c.PacienteID
                                   join m in dataContex.Medicos on c.MedicoID equals m.MedicoID
                                   where c.FechaHora.Date == fechaSeleccionada.Value.Date // Filtrar por fecha
                                   select new 
                                   {
                                       ID = p.PacienteID,
                                       DNI = p.Documento.ToString(),
                                       NombrePaciente = p.Nombre,
                                       ApellidoPaciente = p.Apellido,
                                       NombreMedico = m.Nombre,
                                       ApellidoMedico = m.Apellido,
                                       Especialidad = m.Especialidad,
                                       Motivo = c.MotivoConsulta,
                                       c.FechaHora                                      
                                   };
                    // Transformar el resultado y asignar el formato deseado
                    var resultados = consulta.ToList().Select(c => new PacienteTurno
                    {
                        ID = c.ID,
                        DNI = c.DNI.ToString(),
                        NombrePaciente = c.NombrePaciente,
                        ApellidoPaciente = c.ApellidoPaciente,
                        NombreMedico = c.NombreMedico,
                        ApellidoMedico = c.ApellidoMedico,
                        Especialidad = c.Especialidad,
                        Motivo = c.Motivo, 
                        Fecha = c.FechaHora.ToString("yyyy-MM-dd"),
                        Hora = c.FechaHora
                    }).ToList();
                    // Asignar el resultado al ItemsSource del DataGrid
                    Pacientes.ItemsSource = resultados;
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione una fecha.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los turnos de pacientes: {ex.Message}");
            }
        }
        public void Buscar_Paciente_Documento(object sender, RoutedEventArgs e)
        {
            try
            {

                int documento = int.Parse(pacDocumento.Text);

                try
                {
                    // Obtener la fecha seleccionada del DatePicker
                    DateTime? fechaSeleccionada = dia_turno.SelectedDate;
                    // Verificar si se ha seleccionado una fecha
                    if (fechaSeleccionada.HasValue)
                    {
                        // Consulta con LINQ para unir las tablas
                        var consulta = from p in dataContex.Pacientes
                                       join c in dataContex.Citas on p.PacienteID equals c.PacienteID
                                       join m in dataContex.Medicos on c.MedicoID equals m.MedicoID
                                       where p.Documento == documento && c.FechaHora.Date == fechaSeleccionada.Value.Date
                                       select new
                                       {
                                           ID = p.PacienteID,
                                           DNI = p.Documento.ToString(),
                                           NombrePaciente = p.Nombre,
                                           ApellidoPaciente = p.Apellido,
                                           NombreMedico = m.Nombre,
                                           ApellidoMedico = m.Apellido,
                                           Especialidad = m.Especialidad,
                                           Motivo = c.MotivoConsulta,
                                           c.FechaHora
                                       };

                        // Transformar el resultado y asignar el formato deseado
                        var resultados = consulta.ToList().Select(c => new PacienteTurno
                        {
                            ID = c.ID,
                            DNI = c.DNI.ToString(),
                            NombrePaciente = c.NombrePaciente,
                            ApellidoPaciente = c.ApellidoPaciente,
                            NombreMedico = c.NombreMedico,
                            ApellidoMedico = c.ApellidoMedico,
                            Especialidad = c.Especialidad,
                            Motivo = c.Motivo,
                            Fecha = c.FechaHora.ToString("yyyy-MM-dd"),
                            Hora = c.FechaHora
                        }).ToList();

                        // Asignar el resultado al ItemsSource del DataGrid
                        Pacientes.ItemsSource = resultados;
                    }
                    else
                    {
                        MessageBox.Show("Por favor, seleccione una fecha.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar los turnos de pacientes: {ex.Message}");
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show($"Debe ingresar un numero de documento: {ex.Message}");
            }
        }
        public void Buscar_Paciente_Especialidad(object sender, RoutedEventArgs e)
        {
            string especialidad = pacEspecialidad.Text;

            try
            {
                // Obtener la fecha seleccionada del DatePicker
                DateTime? fechaSeleccionada = dia_turno.SelectedDate;
                // Verificar si se ha seleccionado una fecha
                if (fechaSeleccionada.HasValue)
                {
                    // Consulta con LINQ para unir las tablas
                    var consulta = from p in dataContex.Pacientes
                                   join c in dataContex.Citas on p.PacienteID equals c.PacienteID
                                   join m in dataContex.Medicos on c.MedicoID equals m.MedicoID
                                   where m.Especialidad == especialidad && c.FechaHora.Date == fechaSeleccionada.Value.Date
                                   select new
                                   {
                                       ID = p.PacienteID,
                                       DNI = p.Documento.ToString(),
                                       NombrePaciente = p.Nombre,
                                       ApellidoPaciente = p.Apellido,
                                       NombreMedico = m.Nombre,
                                       ApellidoMedico = m.Apellido,
                                       Especialidad = m.Especialidad,
                                       Motivo = c.MotivoConsulta,
                                       c.FechaHora
                                   };

                    // Transformar el resultado y asignar el formato deseado
                    var resultados = consulta.ToList().Select(c => new PacienteTurno
                    {
                        ID = c.ID,
                        DNI = c.DNI.ToString(),
                        NombrePaciente = c.NombrePaciente,
                        ApellidoPaciente = c.ApellidoPaciente,
                        NombreMedico = c.NombreMedico,
                        ApellidoMedico = c.ApellidoMedico,
                        Especialidad = c.Especialidad,
                        Motivo = c.Motivo,
                        Fecha = c.FechaHora.ToString("yyyy-MM-dd"),
                        Hora = c.FechaHora
                    }).ToList();

                    // Asignar el resultado al ItemsSource del DataGrid
                    Pacientes.ItemsSource = resultados;
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione una fecha.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los turnos de pacientes: {ex.Message}");
            }
        }
        private void GestionTurnos(object sender, RoutedEventArgs e)
        {
            GestionTurnos gestionTurnos = new GestionTurnos();
            gestionTurnos.Show();
        }
        public void Cancelar_Turno(object sender, RoutedEventArgs e)
        {
            if (pacienteTraslado == null)
            {
                MessageBox.Show("Seleccione un Paciente primero.");
                return;
            }

            try
            {
                // Consulta LINQ para encontrar el turno a cancelar
                var turnoACancelar = (from c in dataContex.Citas
                                      where c.PacienteID == pacienteTraslado.ID && c.FechaHora == pacienteTraslado.Hora
                                      select c).FirstOrDefault();

                if (turnoACancelar != null)
                {
                    // Cancelar el turno (aquí puedes eliminar o cambiar un estado)
                    dataContex.Citas.DeleteOnSubmit(turnoACancelar);
                    dataContex.SubmitChanges();

                    MessageBox.Show("El turno ha sido cancelado exitosamente.");

                    // Actualizar el DataGrid después de cancelar el turno
                    Pacientes.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show("No se encontró el turno para cancelar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cancelar el turno: {ex.Message}");
            }
        }

        private void Turnos_Pacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PacienteTurno pacienteSeleccionado = (PacienteTurno)Pacientes.SelectedItem;

            if (pacienteSeleccionado != null)
            {
                // Muestra un mensaje de confirmación antes de eliminar el paciente
                MessageBoxResult resultado = MessageBox.Show($"Elija su accion a realizar: Yes:Cancela el turno -- No:Modifica el turno. Despues seleccione el boton correspondiente {pacienteSeleccionado.NombrePaciente} {pacienteSeleccionado.ApellidoPaciente}",
                                                             "Confirmar eliminación",
                                                             MessageBoxButton.YesNo,
                                                             MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    // Elimina el turno del paciente de la base de datos
                    pacienteTraslado = pacienteSeleccionado;
                }
            }
            else
            {
                // Si no hay un paciente seleccionado, muestra un mensaje de advertencia
                MessageBox.Show("Por favor, selecciona un paciente de la lista para eliminar su turno.");
            }     
        }
        public void Limpiar_Pantalla(object sender, RoutedEventArgs e)
        {
            // Limpiar los campos de texto
            pacEspecialidad.Text = string.Empty;
            pacDocumento.Text = string.Empty;
            dia_turno.Text = string.Empty; //limpia fecha para volver a seleccionar

            // Opcional: Limpiar la selección del DataGrid
            Pacientes.ItemsSource = null;
        }
        public void Modificar_Turno(object sender, RoutedEventArgs e)
        {
            PacienteTurno pacienteSeleccionado = (PacienteTurno)Pacientes.SelectedItem;
            if (pacienteSeleccionado == null)
            {
                MessageBox.Show("Debe elegir un paciente de la lista para modificar el turno");
            }
            else
            {
                DatosCompartidos.Documento = pacienteSeleccionado.DNI;
                DatosCompartidos.Fecha = pacienteSeleccionado.Fecha;
                DatosCompartidos.Hora = pacienteSeleccionado.Hora; // Ahora es DateTime
                DatosCompartidos.Motivo = pacienteSeleccionado.Motivo;
                GestionTurnos(sender, e); // Llama al método que abre la ventana
            }
        }
    }
}
