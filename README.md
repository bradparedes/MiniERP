# MiniERP 🧾⚙️

**MiniERP** es un sistema backend empresarial modular desarrollado en **.NET 8** utilizando **Clean Architecture** y principios de diseño avanzados. El proyecto está optimizado para garantizar transacciones atómicas, un desacoplamiento absoluto de capas mediante mensajería y robustez de producción lista para auditorías de seguridad.

---

## 🏗️ Arquitectura y Patrones de Diseño Avanzados

Para este proyecto se optó por un diseño de **Arquitectura Limpia (Clean Architecture)** acoplado a patrones corporativos de alta escalabilidad, dividiendo la solución en capas bien definidas:

- **MiniERP.Core (Dominio):** Contiene las entidades puras de negocio (User, Product, Category), constantes globales y abstracciones puras (interfaces de repositorios y servicios). Cero dependencias externas.
- **MiniERP.Application (Lógica de Negocio):** Implementa el patrón **CQRS (Command Query Responsibility Segregation)** a través de **MediatR v11**. Los controladores de la API están 100% desacoplados; solo envían Comandos inmutables (`records`) y Consultas, delegando la ejecución a Handlers independientes.
- **MiniERP.Infrastructure (Datos y Servicios):** Administra la persistencia nativa en **PostgreSQL** mediante Entity Framework Core. Implementa el patrón **Unit of Work** para coordinar transacciones atómicas (ACID), asegurando que los múltiples impactos en la base de datos se completen exitosamente en conjunto o se reviertan por completo.
- **MiniERP.API (Presentación):** Fachada HTTP delgada (Thin Controllers) encargada exclusivamente de la serialización, ruteo y validación de tokens.

---

## 🔐 Seguridad y Robustez de Producción

- **Autenticación Estricta:** Implementación de tokens **JWT Bearer Auth** con validación segura de claims.
- **Seguridad por Diseño:** Control de acceso granular basado en Roles (**Admin** / **User**) y políticas de protección del último administrador del sistema.
- **Trazabilidad Empresarial (Auditoría):** Servicio asíncrono de logs de seguridad (`ISecurityLogService`) integrado directamente en la tubería de MediatR para registrar acciones críticas con ID del actor y objetivo.
- **Middleware Global de Excepciones:** Interceptor centralizado de errores en el Pipeline de ASP.NET Core que captura fallas controladas (400 BadRequest, 404 NotFound, 401 Unauthorized) y errores críticos del sistema, implementando **ILogger estructurado** para guardar el *Stack Trace* en la consola de Linux sin exponer datos sensibles al cliente frontend.

---

## 📡 Endpoints Principales y CQRS Routing

### 🔐 Autenticación y Usuarios (MediatR Commands)
- `POST /api/auth/login` → Envía `LoginCommand`, valida credenciales y retorna JWT.
- `POST /api/auth/register` → Envía `RegisterCommand` para usuarios estándar con hash automático.
- `POST /api/auth/register-admin` → Protegido por Rol Admin. Registra nuevos administradores.
- `DELETE /api/users/{id}` → Protegido por Rol Admin. Ejecuta un borrado seguro de la entidad.

### 📦 Gestión de Inventario y Negocio
- `GET /api/product` → Envía `GetAllProductsQuery` optimizado con `.AsNoTracking()`.
- `POST /api/product` → Envía `CreateProductCommand` con validación transaccional.
- `GET /api/category` → Consulta asíncrona de categorías del ERP.

---

## 📸 Capturas del Proyecto (Swagger UI Activo)

### 🔐 Autenticación e Interactividad JWT
<img width="1913" height="358" alt="Botón-Authorize-Swagger" src="https://github.com/user-attachments/assets/7631047a-4a8c-4344-9da9-7df337286ce9" />


### 📦 Gestión de Productos, Categorías y Auditoría Desacoplada (MediatR)
<img width="1913" height="946" alt="Endpoints-MiniERP-en-Swagger" src="https://github.com/user-attachments/assets/9217264f-3522-4a52-87a7-dd47210a59df" />

### 📊 Modelado de Comandos y Esquemas de Datos (CQRS)
<img width="1918" height="948" alt="Schemas-SwaggerUI" src="https://github.com/user-attachments/assets/c7a7f32f-2858-491d-9bb6-041a2eeadda1" />



---

## 📈 Estado del Proyecto

- [x] Arquitectura de Software Base (Clean Architecture)
- [x] Desacoplamiento Total de Controladores (MediatR + CQRS)
- [x] Transaccionalidad Atómica y Coordinada (Unit of Work)
- [x] Motor de Base de Datos PostgreSQL Optimizado (Fase 2 Indexes)
- [x] Robustez y Logging Estructurado (Exception Middleware)
- [ ] Módulos ERP Expandidos (Inventario Avanzado, Facturación, Clientes)

---

## 🛠️ Instalación y Configuración (Multiplataforma con Docker)

El proyecto está preparado para automatizar su entorno de base de datos mediante **Docker Compose**, lo que permite levantarlo tanto en **Linux (Ubuntu)** como en **Windows (Docker Desktop)** sin necesidad de configurar servidores de forma manual.

1. **Clonar el repositorio:**
```bash
git clone https://github.com
cd MiniERP
```

2. **Levantar la base de datos PostgreSQL en un contenedor de Docker:**
```bash
docker compose up -d
```
*(Este comando descargará la imagen oficial aislada de Postgres y activará el puerto 5432 de forma automática).*

3. **Restaurar las dependencias de .NET:**
```bash
dotnet restore
```

4. **Aplicar las migraciones para estructurar las tablas en Docker:**
```bash
dotnet ef database update --project MiniERP.Infrastructure --startup-project MiniERP.API
```

5. **Compilar y ejecutar la Web API:**
```bash
dotnet run --project MiniERP.API
```

---

## 🧪 Cómo Probar la API de Forma Interactiva

1. Una vez ejecutado el proyecto con `dotnet run`, abre la documentación interactiva en tu navegador:
```text
http://localhost:5121/swagger/index.html
```
2. Ejecuta el endpoint `POST /api/auth/login` con tus credenciales de prueba.
3. Copia el token JWT de la respuesta JSON.
4. Haz clic en el botón superior **"Authorize" (Candado de Seguridad)**, escribe la palabra `Bearer` seguida de un espacio, pega tu token y haz clic en Authorize.
5. Los endpoints protegidos por políticas y roles (CQRS / MediatR) quedarán completamente desbloqueados para pruebas en tiempo real.


---

## 👨‍💻 Autor

**Bradley Paredes**  
*Backend Developer | .NET Core | C# | SQL & PostgreSQL | Software Security*  

- **LinkedIn:** [Bradley Castañeda Paredes](https://www.linkedin.com/in/bradley-casta%C3%B1eda-paredes-1577b5236/)
- **GitHub:** [@bradparedes](https://github.com/bradparedes)



