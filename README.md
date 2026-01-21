# MiniERP 🚀

MiniERP es un sistema ERP desarrollado en **.NET 8** con arquitectura limpia, autenticación JWT, roles de usuario y logs de seguridad.

## ✨ Funcionalidades

- Autenticación JWT
- Roles: Admin / User
- Gestión de usuarios
- Gestión de productos
- Logs de seguridad
- Protección del último administrador
- Reset y cambio de contraseña

## 🏗️ Arquitectura

- MiniERP.API → Web API
- MiniERP.Application → Casos de uso / DTOs / Servicios
- MiniERP.Core → Entidades y contratos
- MiniERP.Infrastructure → Acceso a datos (EF Core / PostgreSQL)

## 🛠️ Tecnologías

- .NET 8
- Entity Framework Core
- PostgreSQL
- JWT Authentication

## 🚀 Cómo ejecutar

1. Clonar el repo:
```bash
git clone https://github.com/bradparedes/MiniERP.git
