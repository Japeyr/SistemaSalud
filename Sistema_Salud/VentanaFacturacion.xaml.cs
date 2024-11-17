using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using static Sistema_Salud.VentanaFacturacion;

namespace Sistema_Salud
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class VentanaFacturacion : Window, INotifyPropertyChanged
    {
        DataClasses1DataContext dataContex; // Declaración de la variable

        public ObservableCollection<string> EstadoFactura { get; set; }
        private string estadoSelected;
        public string EstadoFacturaSeleccionado
        {
            get { return estadoSelected; }
            set
            {
                if (estadoSelected != value)
                {
                    estadoSelected = value;
                    OnPropertyChanged(nameof(EstadoFacturaSeleccionado)); // Notifica el cambio
                }
            }
        }
        private bool puedeRegistrar;
        public bool PuedeRegistrar
        {
            get { return puedeRegistrar; }
            set
            {
                if (puedeRegistrar != value)
                {
                    puedeRegistrar = value;
                    OnPropertyChanged(nameof(PuedeRegistrar)); // Notificar cambios para que el binding funcione
                }
            }
        }
        public VentanaFacturacion()
        {
            InitializeComponent();
            PuedeRegistrar = false; // Inicialmente deshabilitado
            DataContext = this;
            // Inicializar la lista de Estados
            EstadoFactura = new ObservableCollection<string>
            {
                "Seleccionar","Pendiente", "Pagado"
            };

            DataContext = this; // Esto permite que el binding funcione
                                // Definir "Seleccionar" como texto predeterminado
            EstadoFacturaSeleccionado = "Seleccionar";
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
        public class Factura
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public int PacienteID { get; set; }
            public int TratamientoID { get; set; }
            public DateTime FechaTratamiento { get; set; }
            public decimal? Costo { get; set; }
            public string Estado { get; set; }
        }
        // Método para notificar cambios en la propiedad (implementación INotifyPropertyChanged)
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BuscarFacturasPaciente_Click(object sender, RoutedEventArgs e)
        {
            PuedeRegistrar = true; // Activar el botón Registrar
            MessageBox.Show("muestro facturas");
            string documentoFactura = facDNI.Text;
            if (!string.IsNullOrWhiteSpace(documentoFactura))
            {
                // Intentar convertir el DNI a un entero
                if (int.TryParse(documentoFactura, out int dniFacturaEntero)) // Intenta convertir a int
                {
                    // Consulta para obtener pacientes por DNI
                    var facturaPaciente = from p in dataContex.Pacientes
                                          where p.Documento == dniFacturaEntero // Comparar con el entero

                                          select new { p.PacienteID, p.Nombre, p.Apellido };


                    var resultado = facturaPaciente.Select(g => new { PacienteID = g.PacienteID, Nombre = g.Nombre, Apellido = g.Apellido }).ToList();
                    var facturaPacienteID = facturaPaciente.Select(g => g.PacienteID).FirstOrDefault();
                    if (resultado.Any())
                    {
                        // Consulta para obtener el historial médico del paciente usando su PacienteID
                        var facturacion = from t in dataContex.Tratamientos
                                          join p in dataContex.Pacientes on t.PacienteID equals p.PacienteID
                                          where t.PacienteID == facturaPacienteID
                                          select new Factura
                                          {
                                              Nombre = p.Nombre,
                                              Apellido = p.Apellido,
                                              PacienteID = t.PacienteID,
                                              TratamientoID = t.TratamientoID,
                                              FechaTratamiento = t.FechaTratamiento,
                                              Costo = t.Costo
                                          };

                        // Asignar los resultados al DataGrid
                        FacturacionPaciente.ItemsSource = facturacion.ToList();
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
        private void Facturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Factura facturaSeleccionada = (Factura)FacturacionPaciente.SelectedItem;

            if (facturaSeleccionada != null)
            {
                // Llenar los campos de texto con los datos del paciente seleccionado
                MessageBox.Show("Factura seleccionada");

            }
        }
        private void GuardarFactura_Click(object sender, RoutedEventArgs e)
        {
            {
                // Obtener la fila seleccionada
                var facturaSeleccionada = FacturacionPaciente.SelectedItem as Factura;

                if (facturaSeleccionada != null)
                {
                    // Asignar las variables con los valores de la factura seleccionada
                    string gNombre = facturaSeleccionada.Nombre;
                    string gApellido = facturaSeleccionada.Apellido;
                    int gPacienteId = facturaSeleccionada.PacienteID;
                    int gTratamientoId = facturaSeleccionada.TratamientoID;
                    string estadoSeleccionado = EstadoFacturaSeleccionado;
                    DateTime gFechaTratamiento = facturaSeleccionada.FechaTratamiento;
                    decimal gCosto = facturaSeleccionada.Costo ?? 0; // Usa 0 si es null

                    // Validar que se hayan seleccionado todos los campos necesarios
                    if (estadoSeleccionado != "Seleccionar" && gPacienteId != 0 && gTratamientoId != 0 && gFechaTratamiento != DateTime.MinValue)
                    {
                        // Crear la nueva factura para insertar
                        Facturacion nuevaFactura = new Facturacion
                        {
                            PacienteID = gPacienteId,
                            TratamientoID = gTratamientoId,
                            FechaFactura = gFechaTratamiento,
                            MontoTotal = gCosto,
                            Estado = estadoSeleccionado
                        };

                        try
                        {
                            // Agregar la nueva factura al contexto
                            dataContex.Facturacion.InsertOnSubmit(nuevaFactura);

                            // Confirmar los cambios en la base de datos
                            dataContex.SubmitChanges();
                            // Confirmación de éxito
                            MessageBox.Show("Factura guardada correctamente.");

                            // Confirmar que la factura se guardó correctamente antes de eliminar el tratamiento
                            var tratamientoAEliminar = dataContex.Tratamientos.SingleOrDefault(t => t.TratamientoID == gTratamientoId);
                            if (tratamientoAEliminar != null && estadoSeleccionado == "Pagado")
                            {
                                dataContex.Tratamientos.DeleteOnSubmit(tratamientoAEliminar);

                                // Confirmar la eliminación del tratamiento
                                dataContex.SubmitChanges();
                                // Actualizar directamente el DataGrid
                                var facturacion = from t in dataContex.Tratamientos
                                                  join p in dataContex.Pacientes on t.PacienteID equals p.PacienteID
                                                  where t.PacienteID == gPacienteId
                                                  select new Factura
                                                  {
                                                      Nombre = p.Nombre,
                                                      Apellido = p.Apellido,
                                                      PacienteID = t.PacienteID,
                                                      TratamientoID = t.TratamientoID,
                                                      FechaTratamiento = t.FechaTratamiento,
                                                      Costo = t.Costo
                                                  };
                                FacturacionPaciente.ItemsSource = facturacion.ToList();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Manejar cualquier error durante la inserción
                            MessageBox.Show($"Error al guardar la factura: {ex.Message}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Faltó seleccionar algún campo. Verifique.");
                    }
                }
            }
        }
        private void ListadoPagados_Click(object sender, RoutedEventArgs e)
        {
            var facturacion = from f in dataContex.Facturacion
                              join p in dataContex.Pacientes on f.PacienteID equals p.PacienteID
                              where f.Estado == "Pagado"
                              select new Factura
                              {
                                  Nombre = p.Nombre,
                                  Apellido = p.Apellido,
                                  PacienteID = f.PacienteID,
                                  TratamientoID = f.TratamientoID,
                                  FechaTratamiento = f.FechaFactura,
                                  Costo = f.MontoTotal
                              };
            FacturacionPaciente.ItemsSource = facturacion.ToList();
            PuedeRegistrar = false; // Desactivar el botón Registrar
        }
        private void ListadoPendientes_Click(object sender, RoutedEventArgs e)
        {
            var facturacion = from f in dataContex.Facturacion
                              join p in dataContex.Pacientes on f.PacienteID equals p.PacienteID
                              where f.Estado == "Pendiente"
                              select new Factura
                              {
                                  Nombre = p.Nombre,
                                  Apellido = p.Apellido,
                                  PacienteID = f.PacienteID,
                                  TratamientoID = f.TratamientoID,
                                  FechaTratamiento = f.FechaFactura,
                                  Costo = f.MontoTotal
                              };
            FacturacionPaciente.ItemsSource = facturacion.ToList();
            PuedeRegistrar = false; // Desactivar el botón Registrar
        }
    }
}
