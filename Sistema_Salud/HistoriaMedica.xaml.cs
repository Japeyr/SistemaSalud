using System;
using System.Collections.Generic;
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

namespace Sistema_Salud
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class HistoriaMedica : Window
    {
        DataClasses1DataContext dataContex; // Declaración de la variable
        public HistoriaMedica()
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
        public void MostrarHistoriaClinica(object sender, RoutedEventArgs e)
        {
            string documentoHC = dniHM.Text;
            if (!string.IsNullOrWhiteSpace(documentoHC))
            {
                // Intentar convertir el DNI a un entero
                if (int.TryParse(documentoHC, out int dniEntero)) // Intenta convertir a int
                {
                    // Consulta para obtener pacientes por DNI
                    var historiaClinicaDNI = from p in dataContex.Pacientes
                                       where p.Documento == dniEntero // Comparar con el entero

                                       select new { p.PacienteID, p.Nombre, p.Apellido };


                    var resultado = historiaClinicaDNI.Select(g => new { PacienteID = g.PacienteID, Nombre = g.Nombre, Apellido = g.Apellido }).ToList();
                    var historiaMedicaPacienteID = historiaClinicaDNI.Select(g => g.PacienteID).FirstOrDefault();
                    if (resultado.Any())
                    {
                        // Consulta para obtener el historial médico del paciente usando su PacienteID
                        var historialMedico = from h in dataContex.HistorialMedico
                                              where h.PacienteID == historiaMedicaPacienteID
                                              select new
                                              {
                                                  resultado[0].Nombre,     // Nombre del paciente
                                                  resultado[0].Apellido,   // Apellido del paciente
                                                  h.FechaActualizacion,    // Fecha de actualización del historial
                                                  h.Detalles               // Detalles del historial
                                              };

                        // Asignar los resultados al DataGrid
                        HistoriaClinica.ItemsSource = historialMedico.ToList();
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
    }
}
