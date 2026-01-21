# MiniERP 🧾⚙️

Sistema ERP modular desarrollado en **.NET 8/9** con arquitectura limpia, autenticación segura y control de roles.

---

## 🚀 Tecnologías

- ASP.NET Core Web API  
- Entity Framework Core  
- PostgreSQL  
- JWT Authentication  
- Clean Architecture (API, Application, Domain, Infrastructure)

---

## 🔐 Seguridad

- Autenticación con JWT  
- Roles: **Admin**, **User**  
- Protección del último administrador  
- Logs de seguridad (auditoría)  
- Reseteo de contraseña  
- Control de acceso por roles y políticas

---

## 📦 Estructura del proyecto

```bash
MiniERP/
├── MiniERP.API
├── MiniERP.Application
├── MiniERP.Domain
└── MiniERP.Infrastructure
```

---

## 📡 Endpoints principales
- POST /api/auth/login
- POST /api/auth/register
- GET /api/productos (User, Admin)
- POST /api/productos (Admin)
- DELETE /api/users/{id} (Admin)
- GET /api/securitylogs (Admin)

---

## 📸 Capturas del proyecto

### 🔐 Autenticación

<img width="1920" height="988" alt="login" src="https://github.com/user-attachments/assets/e79f0a91-4b26-4b0e-8fe6-2d80e97974f2" />

### 📦 Gestión de productos y Logs

<img width="1920" height="988" alt="ProductosYSecurityLogs" src="https://github.com/user-attachments/assets/69217dc7-aeb9-43d1-a689-f6889b65ab83" />


### 📊 Schemas

<img width="1920" height="988" alt="Schemas" src="https://github.com/user-attachments/assets/502baf30-78c4-49e7-a749-5d82f7642cbd" />


---

## 📈 Estado del proyecto

✔ Autenticación
✔ Roles
✔ Logs de seguridad
✔ Protección del último admin
🚧 Módulos ERP en desarrollo (Inventario, Ventas, Clientes)

---

## 🧠 Buenas prácticas aplicadas

- Arquitectura limpia

- Principio de responsabilidad única (SRP)

- Separación de capas

- Validaciones centralizadas

- Seguridad por diseño

---

## 👨‍💻 Autor

Desarrollado por Brad Paredes
Backend Developer | .NET | SQL | Seguridad
Linkedin: https://www.linkedin.com/in/bradley-casta%C3%B1eda-paredes-1577b5236/

## 🛠️ Instalación

```bash
git clone https://github.com/bradparedes/MiniERP.git
cd MiniERP
dotnet restore
dotnet ef database update
dotnet run --project MiniERP.API
```

---

## 🧪 Cómo probar la API

1. Ejecuta el proyecto:
```bash
dotnet run --project MiniERP.API
```

2. Abre Swagger:
```bash
http://localhost:5121/swagger/index.html
```

3. Regístrate → inicia sesión → copia el token → autoriza → prueba endpoints protegidos.
