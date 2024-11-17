using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Linq;
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
using Sistema_Salud;
using System.ComponentModel;


namespace Sistema_Salud
{

    public partial class GestionTurnos : Window, INotifyPropertyChanged
    {
        private DatosCompartidos datosCompartidos;
        DataClasses1DataContext dataContex; // Declaración de la variable
        private int? selectedPacienteID;
        public ObservableCollection<string> HorasDisponibles { get; set; }
        //public string HoraSeleccionada { get; set; }
        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private string horaSelected;
        public string HoraSeleccionada
        {
            get { return horaSelected; }
            set
            {
                if (horaSelected != value)
                {
                    horaSelected = value;
                    OnPropertyChanged(nameof(HoraSeleccionada)); // Notifica el cambio
                }
            }
        }

        // Método para notificar cambios
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public GestionTurnos()
        {
            InitializeComponent();

            // Inicializar la lista de horas disponibles
            HorasDisponibles = new ObservableCollection<string>
    {
        "08:00 AM", "08:30 AM", "09:00 AM", "09:30 AM", "10:00 AM",
        "10:30 AM", "11:00 AM", "11:30 AM", "12:00 PM", "12:30 PM",
        "01:00 PM", "01:30 PM", "02:00 PM", "02:30 PM", "03:00 PM",
        "03:30 PM", "04:00 PM", "04:30 PM", "05:00 PM"
    };
            DataContext = this; // Esto permite que el binding funcione
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Sistema_Salud.Properties.Settings.SistemaSaludConnectionString"].ConnectionString;
                dataContex = new DataClasses1DataContext(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener la cadena de conexión: {ex.Message}");
            }
            gtDNI.Text = DatosCompartidos.Documento;
            EjecutarBusquedaProgramada(); // Llama a la función al iniciar la ventana
            gtFechaTurno.Text = DatosCompartidos.Fecha;
            gtMotivo.Text = DatosCompartidos.Motivo;
            if (DatosCompartidos.Hora != null)
            {
                HoraSeleccionada = DatosCompartidos.Hora.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).ToUpper();
            }
            
            horaSeleccionada.SelectedItem = HoraSeleccionada;
            this.Loaded += Mostrar_Medicos;
        }
        public void Mostrar_Medicos(object sender, RoutedEventArgs e)
        {
            listaMedicos.ItemsSource = dataContex.Medicos.ToList();
        }

        private void BuscarDNI_Click(object sender, RoutedEventArgs e)
        {
            string DNI = gtDNI.Text;
            if (!string.IsNullOrWhiteSpace(DNI))
            {
                // Intentar convertir el DNI a un entero
                if (int.TryParse(DNI, out int dniInt)) // Intenta convertir a int
                {
                    // Consulta para obtener pacientes por DNI y agrupar por ID, nombre y apellido
                    var pacientesDNI = from p in dataContex.Pacientes
                                       where p.Documento == dniInt // Comparar con el entero
                                       group p by new { p.PacienteID, p.Nombre, p.Apellido } into g
                                       select new
                                       {
                                           PacienteID = g.Key.PacienteID,
                                           Nombre = g.Key.Nombre,
                                           Apellido = g.Key.Apellido
                                       };

                    // Convertir a lista
                    var resultados = pacientesDNI.ToList();

                    if (resultados.Any())
                    {
                        listaPaciente.ItemsSource = resultados; // Asignar lista al DataGrid
                        selectedPacienteID = resultados.First().PacienteID; // Guardar el ID del primer paciente
                    }
                    else
                    {
                        selectedPacienteID = null;
                        MessageBox.Show("No se encontró ningún paciente con ese DNI.");
                        listaPaciente.ItemsSource = null; // Limpiar la lista en caso de no encontrar
                    }
                }
                else
                {
                    MessageBox.Show("El DNI ingresado no es válido. Debe ser un número.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa un DNI válido.");
            }
        }
        private void listaMedicos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Medicos medicoSeleccionado = listaMedicos.SelectedItem as Medicos;

            if (medicoSeleccionado != null)
            {
                // Llenar los campos de texto con los datos del medico seleccionado
                int medicoId = medicoSeleccionado.MedicoID;
            }
        }
        private void Otorgar_Turno(object sender, RoutedEventArgs e)
        {
            var horaBuscada = DatosCompartidos.Hora.ToString("hh:mmtt");

            // Buscar el ComboBoxItem que coincida con la hora formateada
            var item = horaSeleccionada.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == horaBuscada);

            // Obtener los valores necesarios
            string fecha = gtFechaTurno.Text;
            string hora = horaSeleccionada.Text;
            string motivo = gtMotivo.Text;

            // Verificar que todos los campos necesarios tengan valores
            if (selectedPacienteID.HasValue && !string.IsNullOrWhiteSpace(fecha) &&
                !string.IsNullOrWhiteSpace(hora) && !string.IsNullOrWhiteSpace(motivo))
            {
                try
                {
                    if (listaMedicos.SelectedItem is Medicos medicoSeleccionado)
                    {
                        // Crear una nueva cita
                        Citas nuevaCita = new Citas
                        {
                            PacienteID = selectedPacienteID.Value,
                            MedicoID = medicoSeleccionado.MedicoID,
                            FechaHora = DateTime.Parse($"{fecha} {hora}"),
                            MotivoConsulta = motivo
                        };

                        // Insertar la nueva cita en la base de datos
                        dataContex.Citas.InsertOnSubmit(nuevaCita);
                        dataContex.SubmitChanges();

                        MessageBox.Show("Turno otorgado con éxito.");
                    }
                    else
                    {
                        MessageBox.Show("Por favor, selecciona un médico.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al otorgar el turno: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Por favor, asegúrate de que todos los campos estén completos.");
            }
        }
        private void Paciente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        public void EjecutarBusquedaProgramada()
        {
            // Simula el evento de clic en el botón.
            BuscarDNI_Click(null, new RoutedEventArgs());
        }
        private void Modificar_Turno(object sender, EventArgs e)
        {
            Medicos medicoSeleccionado = listaMedicos.SelectedItem as Medicos;
            if (medicoSeleccionado == null)
            {
                // Mostrar un mensaje de advertencia si no se ha seleccionado un médico
                MessageBox.Show("Por favor, selecciona un médico antes de modificar el turno.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Salir del método si no se ha seleccionado un médico
            }

            int modMedicoId = medicoSeleccionado.MedicoID;
            string modMotivoConsulta = gtMotivo.Text;
            string modFecha = gtFechaTurno.Text;
            string modHora = horaSeleccionada.Text;

            try
            {
             // Combinar fecha y hora y convertir a DateTime
                DateTime fechaHoraCompleta = DateTime.ParseExact($"{modFecha} {modHora}", "d/M/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                // Formatear a "yyyy-MM-dd HH:mm:ss.fff"
                string fechaHoraFormateada = fechaHoraCompleta.ToString("yyyy-MM-dd HH:mm:ss.fff");
                Citas modificacionCita = dataContex.Citas.FirstOrDefault(c => c.PacienteID == selectedPacienteID.Value);
                modificacionCita.MedicoID = modMedicoId;
                modificacionCita.MotivoConsulta = modMotivoConsulta;
                modificacionCita.FechaHora = fechaHoraCompleta;
                dataContex.SubmitChanges();
                MessageBox.Show($"Se actualizó turno de fecha {fechaHoraFormateada} \nMotivo de la consulta {modMotivoConsulta}\n");
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Error al formatear la fecha: {ex.Message}");
            }
        }
    }
}
