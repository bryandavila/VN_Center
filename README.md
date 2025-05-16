<p align="center">
  <img src="https://placehold.co/100x100/0275d8/FFFFFF?text=VNC" alt="VN Center Logo Placeholder" width="100px" height="auto">
</p>

<h1 align="center">
    VN Center - Sistema de Gestión Integral para ONG
</h1>

<p align="center">
  Una solución robusta y personalizable desarrollada en ASP.NET Core MVC para la gestión eficiente de las operaciones de la ONG "VN Center".
</p>

<p align="center">
  <em>Transformando la gestión de voluntariado, proyectos y beneficiarios con tecnología.</em>
</p>

---

## Introducción 🚀

**VN Center** es una aplicación web diseñada específicamente para optimizar y centralizar los procesos clave de organizaciones no gubernamentales (ONGs) dedicadas al voluntariado, pasantías, y desarrollo comunitario. Este sistema proporciona una plataforma integral para administrar desde la captación de solicitantes hasta el seguimiento detallado de programas, proyectos, y el impacto en los beneficiarios.

Construido con ASP.NET Core MVC y aprovechando la robustez de .NET 8, VN Center ofrece una interfaz de usuario moderna, intuitiva y adaptable (basada originalmente en la plantilla Sneat), asegurando una experiencia fluida tanto para administradores como para usuarios del sistema.

## Descripción General del Proyecto 🎯

El objetivo principal de **VN Center** es empoderar a las ONGs con una herramienta tecnológica que facilite la organización, el seguimiento y la generación de informes, permitiéndoles enfocar más recursos en su misión principal. El sistema está diseñado para ser escalable y adaptable a las necesidades específicas de la organización.

**Contexto de Aplicación:**
Este sistema ha sido desarrollado pensando en las necesidades de "VN Center", una ONG que gestiona:
* Solicitudes de voluntarios y pasantes nacionales e internacionales.
* Una base de datos de beneficiarios y las asistencias que reciben.
* Múltiples programas y proyectos en diversas comunidades.
* La vinculación de participantes y beneficiarios con estos programas y grupos comunitarios.
* La evaluación y seguimiento del impacto de sus iniciativas.

## Funcionalidades Clave ✨

El sistema VN Center incluye, entre otras, las siguientes funcionalidades:

* **Gestión de Solicitudes (Voluntarios/Pasantías):**
    * Registro y seguimiento de solicitudes.
    * Gestión de estados (Recibida, En Revisión, Aprobada, Rechazada).
    * Almacenamiento de información detallada del solicitante.
* **Gestión de Beneficiarios:**
    * Registro completo de datos demográficos, socioeconómicos y de necesidades.
    * Seguimiento de asistencia recibida.
    * Vinculación con programas y grupos.
* **Gestión de Programas y Proyectos de la ONG:**
    * Creación y administración de iniciativas.
    * Asignación de responsables y seguimiento de estados.
    * Vinculación con comunidades y grupos específicos.
* **Gestión de Comunidades y Grupos Comunitarios:**
    * Catálogo de comunidades donde opera la ONG.
    * Registro y gestión de grupos comunitarios asociados.
* **Vinculaciones y Seguimiento:**
    * Asignación de voluntarios/pasantes a programas (Participaciones Activas).
    * Registro de evaluaciones de programa por parte de los participantes.
    * Vinculación de beneficiarios con programas y grupos.
    * Registro de asistencia a beneficiarios.
* **Gestión de Consultas Generales:**
    * Recepción y seguimiento de solicitudes de información general.
    * Asignación a usuarios responsables para respuesta.
* **Administración del Sistema:**
    * Gestión de usuarios del sistema (empleados de la ONG).
    * Gestión de roles (Administrador, Usuario).
    * (Próximamente) Gestión de permisos granulares.
    * Restablecimiento de contraseñas por administradores.
* **Auditoría de Cambios:**
    * Registro detallado de acciones importantes realizadas en el sistema (creación, edición, eliminación de usuarios, etc.).
    * Visualización de logs de auditoría con filtros.
* **Exportación de Datos:**
    * Generación de reportes en formato PDF y Excel para diversas entidades (Beneficiarios, Evaluaciones, Auditoría, Usuarios, etc.).
* **Autenticación y Autorización Robustas:**
    * Sistema de inicio de sesión seguro basado en ASP.NET Core Identity.
    * Protección de rutas y funcionalidades basada en roles.
    * Funcionalidad de "Olvidé mi Contraseña" con envío de correo electrónico.

## Tecnologías Utilizadas 🛠️

* **Backend:** ASP.NET Core MVC (.NET 8)
* **Base de Datos:** Microsoft SQL Server (configurable)
* **ORM:** Entity Framework Core 8
* **Identidad y Seguridad:** ASP.NET Core Identity
* **Frontend (Base Visual):** Plantilla Sneat (Bootstrap 5, HTML, CSS, JS)
* **Generación de PDF:** QuestPDF
* **Generación de Excel:** ClosedXML
* **Envío de Correos:** SendGrid (configurable)
* **Lenguaje Principal:** C#

## Instalación y Configuración ⚙️

1.  **Clonar el Repositorio:**
    ```bash
    git clone https://URL_DE_TU_REPOSITORIO.git
    cd NOMBRE_DE_LA_CARPETA_DEL_PROYECTO
    ```
2.  **Configurar la Cadena de Conexión:**
    * Abre `appsettings.json` (y `appsettings.Development.json`).
    * Modifica la sección `ConnectionStrings` con los detalles de tu instancia de SQL Server:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Server=TU_SERVIDOR;Database=VN_Center;User ID=TU_USUARIO;Password=TU_CONTRASENA;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;"
        }
        ```
3.  **Aplicar Migraciones de Base de Datos:**
    * Abre la Consola del Administrador de Paquetes en Visual Studio.
    * Ejecuta: `Update-Database`
4.  **Configurar Secretos de Usuario (para claves de API):**
    * Para SendGrid y otras claves sensibles, utiliza "Secretos de Usuario":
        * Haz clic derecho en el proyecto en Visual Studio > "Administrar secretos de usuario".
        * Añade tus claves, por ejemplo:
            ```json
            {
              "SendGrid:ApiKey": "TU_API_KEY_DE_SENDGRID",
              "AdminUser:Password": "TU_CONTRASENA_ADMIN_INICIAL" 
            }
            ```
5.  **Ejecutar la Aplicación:**
    * Desde Visual Studio (F5 o Ctrl+F5) o mediante el CLI de .NET (`dotnet run`).

## Licencia ©

Este proyecto se distribuye bajo la Licencia MIT. Consulta el archivo `LICENSE` para más detalles.

---

<p align="center">
  Desarrollado con ❤️ para VN Center.
</p>
