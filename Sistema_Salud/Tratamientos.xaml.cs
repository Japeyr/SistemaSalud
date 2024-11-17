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
using System.Runtime.Remoting.Contexts;
using Sistema_Salud;
using System.Globalization;

namespace Sistema_Salud
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class VentanaTratamientos : Window
    {
        DataClasses1DataContext dataContex; // Declaración de la variable
        int pacienteID;
        int medicoId;
        DateTime fechaControl;
        public VentanaTratamientos()
        {
            InitializeComponent();
            tratFecha.SelectedDate = DateTime.Now;
            fechaControl = tratFecha.DisplayDate;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Sistema_Salud.Properties.Settings.SistemaSaludConnectionString"].ConnectionString;
                dataContex = new DataClasses1DataContext(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener la cadena de conexión: {ex.Message}");
            }
            EjecutarBusquedaProgramadaMedicos();
        }
        public class Tratamiento
        {
            public int PacienteID { get; set; }
            public int MedicoID { get; set; }
            public string FechaTratamiento { get; set; }
            public string Descripcion { get; set; }
            public decimal Costo { get; set; }
        }
        private void Medicos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Medicos medicoSeleccionado = (Medicos)ListMedicos.SelectedItem;

            if (medicoSeleccionado != null)
            {
                medicoId = medicoSeleccionado.MedicoID;
                //MessageBox.Show(medicoId.ToString());
            }
        }
        public void Mostrar_Medicos(object sender, RoutedEventArgs e)
        {
            ListMedicos.ItemsSource = dataContex.Medicos.ToList();
        }
        public void EjecutarBusquedaProgramadaMedicos()
        {
            // Simula el evento de clic en el botón.
            Mostrar_Medicos(null, new RoutedEventArgs());
        }
        public void BuscarDocumentoPaciente_Click(object sender, RoutedEventArgs e)
        {
            string DNI = tratDocumento.Text;
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
                        tratNombre.Text = resultados.First().Nombre;
                        tratApellido.Text = resultados.First().Apellido;
                        pacienteID = resultados.First().PacienteID;
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún paciente con ese DNI.");
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
        private void GuardarTratamiento_Click(object sender, RoutedEventArgs e)
        {

            // Convierte la fecha seleccionada al formato deseado
            string fechaFormateada = tratFecha.SelectedDate.Value.ToString("yyyy-MM-dd");


            if (pacienteID == 0)
            {
                MessageBox.Show("Por favor, ingrese un DNI válido y haga click en el botón Buscar DNI.");
                return;
            }
            if (medicoId == 0)
            {
                MessageBox.Show("Seleccione un médico de la lista");
                return;
            }
            if (tratFecha.SelectedDate.Value.Date != DateTime.Now.Date)
            {
                MessageBox.Show("La fecha debe ser la de hoy");
                tratFecha.SelectedDate = DateTime.Now;
                fechaControl = DateTime.Now;
                return;
            }
            decimal costoDecimal = 0;
            try
            {
                costoDecimal = Convert.ToDecimal(tratCosto.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                MessageBox.Show("El costo ingresado no es un número válido. ");
                tratCosto.Text = string.Empty;
                tratCosto.Focus();
                return;
            }
            var nuevoTratamiento = new Tratamientos
            {
                PacienteID = pacienteID,
                MedicoID = medicoId,
                FechaTratamiento = tratFecha.SelectedDate.Value,
                Descripcion = tratDescripcion.Text,
                Costo = costoDecimal
            };

            // Agregar el nuevo tratamiento al contexto
            dataContex.Tratamientos.InsertOnSubmit(nuevoTratamiento);

            try
            {
                dataContex.SubmitChanges();
                MessageBox.Show("Tratamiento guardado exitosamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar tratamiento: {ex.Message}");
            }
            var result = from t in dataContex.Tratamientos
                         where t.PacienteID == pacienteID
                         && t.MedicoID == medicoId
                         && t.FechaTratamiento.Date == fechaControl
                         select new
                         {
                             t.TratamientoID,
                             t.PacienteID,
                             t.MedicoID,
                             t.FechaTratamiento,
                             t.Descripcion,
                             t.Costo
                         };
            var tratamiento = result.FirstOrDefault();

            if (tratamiento != null)
            {
                MessageBox.Show($"TratamientoID: {tratamiento.TratamientoID}");
            }
            else
            {
                MessageBox.Show("No se encontró tratamiento para esos parámetros.");
            }
        }
        public void EliminarTratamiento_Click(object sender, EventArgs e)
        {
            if (pacienteID == 0)
            {
                MessageBox.Show("Por favor, ingrese un DNI válido y haga click en el botón Buscar DNI.");
                return;
            }
            if (medicoId == 0)
            {
                MessageBox.Show("Seleccione un médico de la lista");
                return;
            }
            if (tratFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar una fecha");
                return;
            }
            // Buscar el tratamiento con los parámetros especificados
            var tratamiento = (from t in dataContex.Tratamientos
                               where t.PacienteID == pacienteID
                               && t.MedicoID == medicoId
                               && t.FechaTratamiento.Date == tratFecha.SelectedDate.Value.Date
                               select t).FirstOrDefault();

            if (tratamiento != null)
            {
                dataContex.Tratamientos.DeleteOnSubmit(tratamiento);

                // Confirmar los cambios en la base de datos
                try
                {
                    dataContex.SubmitChanges();
                    MessageBox.Show("El tratamiento ha sido eliminado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error al eliminar el tratamiento: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No se encontró tratamiento para esos parámetros.");
            }
        }
        public void ActualizarTratamiento_Click(object sender, EventArgs e)
        {

            if (pacienteID == 0)
            {
                MessageBox.Show("Por favor, ingrese un DNI válido y haga click en el botón Buscar DNI.");
                return;
            }
            if (medicoId == 0)
            {
                MessageBox.Show("Seleccione un médico de la lista");
                return;
            }
            if (tratFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar una fecha");
                return;
            }
            decimal costoDecimal;
            if (string.IsNullOrWhiteSpace(tratCosto.Text))
            {
                MessageBox.Show("El campo de costo está vacío.");
                tratCosto.Focus();
                return;
            }

            try
            {
                costoDecimal = Convert.ToDecimal(tratCosto.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                MessageBox.Show("El costo ingresado no es un número válido.");
                tratCosto.Text = string.Empty;
                tratCosto.Focus();
                return;
            }
            // Buscar el tratamiento con los parámetros especificados
            var tratamiento = (from t in dataContex.Tratamientos
                               where t.PacienteID == pacienteID
                               && t.FechaTratamiento.Date == tratFecha.SelectedDate.Value.Date
                               select t).FirstOrDefault();
            if (tratamiento != null)
            {
                tratamiento.Costo = costoDecimal;
                tratamiento.FechaTratamiento = tratFecha.SelectedDate.Value;
                tratamiento.Descripcion = tratDescripcion.Text;
                tratamiento.MedicoID = medicoId;
                try
                {
                    dataContex.SubmitChanges();
                    MessageBox.Show("Tratamiento actualizado con éxito.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al actualizar el tratamiento: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No se encontró el tratamiento para actualizar.");
            }
        }
        public void LimpiarTratamiento_Click(object sender, EventArgs e)
        {
            // Limpiar los campos de texto
            tratNombre.Text = string.Empty;
            tratApellido.Text = string.Empty;
            tratDocumento.Text = string.Empty;
            tratDescripcion.Text = string.Empty;
            tratCosto.Text = string.Empty;
            tratFecha.Text = string.Empty;
        }
        private void MostraHistorialMedico_Click(object sender, RoutedEventArgs e)
        {
            HistoriaMedica historiaMedica = new HistoriaMedica(); // Crea una instancia de la ventana
            historiaMedica.Show(); // Abre la nueva ventana
        }
    }
}
