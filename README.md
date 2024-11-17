![MasterHead](https://newfutureit.com.au/wp-content/uploads/2020/05/img-break-3.jpg)
# Sistema de Gestión de Salud

Este repositorio contiene el código de un Sistema de Gestión de Salud desarrollado en C# utilizando WPF y LINQ to SQL, con una base de datos en SQL Server. El sistema permite gestionar pacientes, médicos, tratamientos, citas y facturación.

## Características principales

- **Gestión de pacientes y médicos**:
  - Crear, modificar, eliminar y listar pacientes y médicos registrados en el sistema.
- **Gestión de tratamientos**:
  - Asignar tratamientos a pacientes.
  - Modificar y eliminar tratamientos existentes.
  - Registro automático en el historial médico mediante un trigger en la base de datos.
- **Gestión de citas**:
  - Otorgar citas a pacientes, incluyendo la selección de médico, fecha y hora.
  - Modificar y cancelar citas existentes.
- **Historial médico**:
  - Búsqueda de pacientes por DNI.
  - Visualización de todas las atenciones registradas para el paciente seleccionado.
- **Facturación y tratamientos pendientes**:
  - Registro de tratamientos pagados en la tabla de facturación.
  - Visualización exclusiva de tratamientos pendientes para facilitar su pago posterior.
  - Visualización de listado de Pagados y Pendientes.

## Requisitos del sistema

Software necesario:
- Microsoft SQL Server (cualquier versión compatible).
- Visual Studio 2022 con soporte para C# y .NET Framework.
- SQL Server Management Studio (SSMS) para ejecutar el script de la base de datos.

## Instalación

1. **Base de datos**:
   - Ejecuta el archivo `script_SistemaSalud.sql` en tu SQL Server para crear la base de datos, tablas, relaciones, stored procedures y triggers necesarios.
2. **Configuración del proyecto**:
   - Clona este repositorio.
   - Abre el proyecto en Visual Studio.
   - Asegúrate de que la cadena de conexión en el archivo `App.config` apunte correctamente a tu instancia de SQL Server.
3. **Ejecución del programa**:
   - Compila y ejecuta el proyecto desde Visual Studio.

## Uso del sistema

Pantallas disponibles:
- **Gestión de pacientes**: Registro, modificación y eliminación.
- **Gestión de médicos**: Registro, modificación y eliminación.
- **Asignación de tratamientos**: Crear, modificar y eliminar tratamientos.
- **Citas médicas**: Gestión de turnos.
- **Historial médico**: Búsqueda por DNI y visualización de atenciones.
- **Facturación y pagos**: Gestión de tratamientos pagados y pendientes.
- 
![Pantalla 1](https://github.com/Japeyr/SistemaSalud/blob/master/Sistema%20de%20Salud%201.png)
-
![Pantalla 2](https://github.com/Japeyr/SistemaSalud/blob/master/Sistema%20de%20Salud%202.png)
-
![Pantalla 3](https://github.com/Japeyr/SistemaSalud/blob/master/Sistema%20de%20Salud%203.png)
