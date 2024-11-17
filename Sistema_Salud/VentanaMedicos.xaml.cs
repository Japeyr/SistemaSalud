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

namespace Sistema_Salud
{
    /// <summary>
    /// Lógica de interacción para VentanaMedicos.xaml
    /// </summary>
    public partial class VentanaMedicos : Window
    {
        DataClasses1DataContext dataContex; // Declaración de la variable
        public VentanaMedicos()
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

        public void Mostrar_Medicos(object sender, RoutedEventArgs e)
        {
            Medicos.ItemsSource = dataContex.Medicos.ToList();

        }
        public void Guardar_Medico(object sender, RoutedEventArgs e)
        {
            Medicos newMedico = new Medicos();

            if (string.IsNullOrWhiteSpace(medNombre.Text))
            {
                MessageBox.Show("Por favor, ingrese un nombre.");
                return; // Salir del método si el nombre está vacío
            }
            newMedico.Nombre = medNombre.Text;

            if (string.IsNullOrWhiteSpace(medApellido.Text))
            {
                MessageBox.Show("Por favor, ingrese un apellido.");
                return; // Salir del método si el nombre está vacío
            }
            newMedico.Apellido = medApellido.Text;

            if (string.IsNullOrWhiteSpace(medEspecialidad.Text))
            {
                MessageBox.Show("Por favor, ingrese una especialidad.");
                return; // Salir del método si el nombre está vacío
            }
            newMedico.Especialidad = medEspecialidad.Text;

            if (string.IsNullOrWhiteSpace(medTelefono.Text))
            {
                MessageBox.Show("Por favor, ingrese un Telefono.");
                return; // Salir del método si el nombre está vacío
            }
            newMedico.Telefono = medTelefono.Text;

            if (string.IsNullOrWhiteSpace(medEmail.Text))
            {
                MessageBox.Show("Por favor, ingrese un E-mail.");
                return; // Salir del método si el nombre está vacío
            }
            newMedico.Email = medEmail.Text;

            dataContex.Medicos.InsertOnSubmit(newMedico);

            dataContex.SubmitChanges();

            Medicos.ItemsSource = dataContex.Medicos.ToList();
        }
        public void Eliminar_Medico(object sender, RoutedEventArgs e)
        {
            // Verifica si hay un paciente seleccionado en el DataGrid
            Medicos medicoSeleccionado = (Medicos)Medicos.SelectedItem;

            if (medicoSeleccionado != null)
            {
                // Muestra un mensaje de confirmación antes de eliminar el paciente
                MessageBoxResult resultado = MessageBox.Show($"¿Estás seguro de que deseas eliminar el Médico {medicoSeleccionado.Nombre} {medicoSeleccionado.Apellido}?",
                                                             "Confirmar eliminación",
                                                             MessageBoxButton.YesNo,
                                                             MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    // Elimina al médico de la base de datos
                    dataContex.Medicos.DeleteOnSubmit(medicoSeleccionado);
                    dataContex.SubmitChanges();

                    // Actualiza el DataGrid para reflejar los cambios
                    Medicos.ItemsSource = dataContex.Medicos.ToList();

                    // Muestra un mensaje de éxito
                    MessageBox.Show("El Médico ha sido eliminado correctamente.");
                }
            }
            else
            {
                // Si no hay un paciente seleccionado, muestra un mensaje de advertencia
                MessageBox.Show("Por favor, selecciona un Medico de la lista para eliminar.");
            }
        }
        public void Limpiar_Medico(object sender, EventArgs e)
        {
            // Limpiar los campos de texto
            medNombre.Text = string.Empty;
            medApellido.Text = string.Empty;
            medEspecialidad.Text = string.Empty;
            medTelefono.Text = string.Empty;
            medEmail.Text = string.Empty;

            // Opcional: Limpiar la selección del DataGrid
            Medicos.ItemsSource = null;
        }

        private void Medicos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Medicos medicoSeleccionado = (Medicos)Medicos.SelectedItem;

            if (medicoSeleccionado != null)
            {
                // Llenar los campos de texto con los datos del paciente seleccionado
                medNombre.Text = medicoSeleccionado.Nombre;
                medApellido.Text = medicoSeleccionado.Apellido;
                medEspecialidad.Text = medicoSeleccionado.Especialidad;
                medTelefono.Text = medicoSeleccionado.Telefono;
                medEmail.Text = medicoSeleccionado.Email;
            }
        }
        public void Actualizar_Medico(object sender, EventArgs e)
        {
            Medicos medicoSeleccionado = (Medicos)Medicos.SelectedItem;

            if (medicoSeleccionado != null)
            {
                // Muestra un mensaje de confirmación antes de modificar el paciente
                MessageBoxResult resultado = MessageBox.Show($"¿Estás seguro de que deseas modificar datos del Medico {medicoSeleccionado.Nombre} {medicoSeleccionado.Apellido}?",
                                                             "Confirmar modificación",
                                                             MessageBoxButton.YesNo,
                                                             MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    Medicos medElegido = dataContex.Medicos.FirstOrDefault(me => me.MedicoID == medicoSeleccionado.MedicoID);
                    medElegido.Nombre = medNombre.Text;
                    medElegido.Apellido = medApellido.Text;
                    medElegido.Especialidad = medEspecialidad.Text;
                    medElegido.Telefono = medTelefono.Text;
                    medElegido.Email = medEmail.Text;

                    dataContex.SubmitChanges();


                    // Actualiza el DataGrid para reflejar los cambios
                    Medicos.ItemsSource = dataContex.Medicos.ToList();

                    // Muestra un mensaje de éxito
                    MessageBox.Show("El Médico ha sido modificado correctamente.");
                }
            }
            else
            {
                // Si no hay un paciente seleccionado, muestra un mensaje de advertencia
                MessageBox.Show("Por favor, selecciona un Médico de la lista a modificar.");
            }
        }
        private void Especialidad_Medico(object sender, RoutedEventArgs e)
        {
            string especialidad = medEspecialidad.Text.ToLower();
            if (especialidad == "")
            {
                MessageBox.Show("Antes de hacer click en Especialidad debe ingresar la Especialidad en el campo correspondiente.");
            }
            else 
            { 
                var medicoEspecialidad = from medico in dataContex.Medicos where medico.Especialidad.ToLower() == especialidad select medico;

                Medicos.ItemsSource = medicoEspecialidad.ToList();
            }
        }
        private void AbrirVentanaTratamientos_Click(object sender, RoutedEventArgs e)
        {
            VentanaTratamientos ventanaTratamientos = new VentanaTratamientos(); // Crea una instancia de la ventana
            ventanaTratamientos.Show(); // Abre la nueva ventana
        }

    }       
}
