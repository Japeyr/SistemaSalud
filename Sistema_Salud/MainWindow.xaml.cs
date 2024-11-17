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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace Sistema_Salud
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataClasses1DataContext dataContex; // Declaración de la variable
        
        public MainWindow()
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
        // Evento para abrir la ventana de médicos
        private void AbrirVentanaMedicos_Click(object sender, RoutedEventArgs e)
        {
            VentanaMedicos ventanaMedicos = new VentanaMedicos(); // Crea una instancia de la ventana
            ventanaMedicos.Show(); // Abre la nueva ventana
        }
        
        private void Guardar_Paciente(object sender, RoutedEventArgs e)
        {
            AgregarPaciente();
        }

        public void Mostrar_Pacientes(object sender, RoutedEventArgs e)
        {
            
            dgPacientes.ItemsSource = dataContex.Pacientes.ToList();
        }
        public void AgregarPaciente()
        {
            ///dataContex.ExecuteCommand("delete from Pacientes");

            Pacientes paciente = new Pacientes();
            // Para un campo de tipo int (entero):
            int documento;
            if (int.TryParse(txtDocumento.Text, out documento))
            {
                paciente.Documento = documento; // Asignar el valor entero al campo Documento
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un número de documento válido.");
                return; // Salir del método si la conversión falla
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor, ingrese un nombre.");
                return; // Salir del método si el nombre está vacío
            }
            paciente.Nombre = txtNombre.Text;

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Por favor, ingrese un apellido.");
                return; // Salir del método si el nombre está vacío
            }
            paciente.Apellido = txtApellido.Text;

            // Obtener la fecha de nacimiento del DatePicker
            if (dpFechaNacimiento.SelectedDate.HasValue)
            {
                paciente.FechaNacimiento = dpFechaNacimiento.SelectedDate.Value; // Asignar el valor de DatePicker
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una fecha de nacimiento.");
                return; // Salir del método si no hay fecha seleccionada
            }

            // Obtener el género del ComboBox
            ComboBoxItem itemSeleccionado = (ComboBoxItem)cbGenero.SelectedItem; // Obtener el item seleccionado
            if (itemSeleccionado != null)
            {
                paciente.Genero = itemSeleccionado.Content.ToString()[0]; // Asignar el primer carácter
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un género.");
                return; // Salir del método si no hay selección
            }

            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MessageBox.Show("Por favor, ingrese una dirección.");
                return; // Salir del método si el nombre está vacío
            }
            paciente.Direccion = txtDireccion.Text;

            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor, ingrese un Telefono.");
                return; // Salir del método si el nombre está vacío
            }
            paciente.Telefono = txtTelefono.Text;

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Por favor, ingrese un E-mail.");
                return; // Salir del método si el nombre está vacío
            }
            paciente.Email = txtEmail.Text;

            dataContex.Pacientes.InsertOnSubmit(paciente);

            dataContex.SubmitChanges();

            dgPacientes.ItemsSource = dataContex.Pacientes.ToList();
        }
        public void Borrar_Paciente(object sender, EventArgs e)
        {
            // Verifica si hay un paciente seleccionado en el DataGrid
            Pacientes pacienteSeleccionado = (Pacientes)dgPacientes.SelectedItem;

            if (pacienteSeleccionado != null)
            {
                // Muestra un mensaje de confirmación antes de eliminar el paciente
                MessageBoxResult resultado = MessageBox.Show($"¿Estás seguro de que deseas eliminar al paciente {pacienteSeleccionado.Nombre} {pacienteSeleccionado.Apellido}?",
                                                             "Confirmar eliminación",
                                                             MessageBoxButton.YesNo,
                                                             MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    // Elimina el paciente de la base de datos
                    dataContex.Pacientes.DeleteOnSubmit(pacienteSeleccionado);
                    dataContex.SubmitChanges();

                    // Actualiza el DataGrid para reflejar los cambios
                    dgPacientes.ItemsSource = dataContex.Pacientes.ToList();

                    // Muestra un mensaje de éxito
                    MessageBox.Show("El paciente ha sido eliminado correctamente.");
                }
            }
            else
            {
                // Si no hay un paciente seleccionado, muestra un mensaje de advertencia
                MessageBox.Show("Por favor, selecciona un paciente de la lista para eliminar.");
            }
        }
        public void Limpiar_Lista(object sender, EventArgs e)
        {
            // Limpiar los campos de texto
            txtDocumento.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            txtEmail.Text = string.Empty;

            // Limpiar el DatePicker
            dpFechaNacimiento.SelectedDate = null;

            // Limpiar el ComboBox (deseleccionar cualquier opción)
            cbGenero.SelectedIndex = -1;

            // Opcional: Limpiar la selección del DataGrid
            dgPacientes.ItemsSource = null;
        }

        private void dgPacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pacientes pacienteSeleccionado = (Pacientes)dgPacientes.SelectedItem;

            if (pacienteSeleccionado != null)
            {
                // Llenar los campos de texto con los datos del paciente seleccionado
                txtDocumento.Text = pacienteSeleccionado.Documento.ToString();
                txtNombre.Text = pacienteSeleccionado.Nombre;
                txtApellido.Text = pacienteSeleccionado.Apellido;
                txtDireccion.Text = pacienteSeleccionado.Direccion;
                txtTelefono.Text = pacienteSeleccionado.Telefono;
                txtEmail.Text = pacienteSeleccionado.Email;

                // Asignar la fecha de nacimiento
                dpFechaNacimiento.SelectedDate = pacienteSeleccionado.FechaNacimiento;

                // Asignar el género
                cbGenero.SelectedItem = cbGenero.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString()[0] == pacienteSeleccionado.Genero);
            }
        }
        public void Actualizar_Paciente(object sender, EventArgs e)
        {
            Pacientes pacienteSeleccionado = (Pacientes)dgPacientes.SelectedItem;

            if (pacienteSeleccionado != null)
            {
                // Muestra un mensaje de confirmación antes de modificar el paciente
                MessageBoxResult resultado = MessageBox.Show($"¿Estás seguro de que deseas modificar datos del paciente {pacienteSeleccionado.Nombre} {pacienteSeleccionado.Apellido}?",
                                                             "Confirmar modificación",
                                                             MessageBoxButton.YesNo,
                                                             MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    Pacientes elegido = dataContex.Pacientes.FirstOrDefault(el => el.PacienteID == pacienteSeleccionado.PacienteID);
                    elegido.Nombre = txtNombre.Text;
                    elegido.Apellido = txtApellido.Text;
                    // Obtener la fecha de nacimiento del DatePicker
                    if (dpFechaNacimiento.SelectedDate.HasValue)
                    {
                        elegido.FechaNacimiento = dpFechaNacimiento.SelectedDate.Value; // Asignar el valor de DatePicker
                    }
                    else
                    {
                        MessageBox.Show("Por favor, seleccione una fecha de nacimiento.");
                        return; // Salir del método si no hay fecha seleccionada
                    }
                    elegido.Direccion = txtDireccion.Text;
                    elegido.Telefono = txtTelefono.Text;
                    elegido.Email = txtEmail.Text;

                    // Para un campo de tipo int (entero):
                    int documento;
                    if (int.TryParse(txtDocumento.Text, out documento))
                    {
                        elegido.Documento = documento; // Asignar el valor entero al campo Documento
                    }
                    else
                    {
                        MessageBox.Show("Por favor, ingrese un número de documento válido.");
                        return; // Salir del método si la conversión falla
                    }
                    dataContex.SubmitChanges();

                    
                    // Actualiza el DataGrid para reflejar los cambios
                    dgPacientes.ItemsSource = dataContex.Pacientes.ToList();

                    // Muestra un mensaje de éxito
                    MessageBox.Show("El paciente ha sido modificado correctamente.");
                }
            }
            else
            {
                // Si no hay un paciente seleccionado, muestra un mensaje de advertencia
                MessageBox.Show("Por favor, selecciona un paciente de la lista a modificar.");
            }
        }
        private void Dar_Turno(object sender, RoutedEventArgs e)
        {
            Turnos turnos = new Turnos();
            turnos.Show();
        }
        private void AbrirVentanaFacturacion_Click(object sender, RoutedEventArgs e)
        {
            VentanaFacturacion ventanaFacturacion = new VentanaFacturacion(); // Crea una instancia de la ventana
            ventanaFacturacion.Show(); // Abre la nueva ventana
        }
    }
}
