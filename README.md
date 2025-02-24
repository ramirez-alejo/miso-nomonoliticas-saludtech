# Diseño y construcción de soluciones no monolíticas

Proyecto del curso de soluciones no monolíticas para el grupo 20.

## Integrantes

* Augusto Romero
* Juan David Orduz
* Camilo Barreto
* Alejandro Ramirez

## Documentación

* [Arquitectura (Entrega 1)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-1)
* [Arquitectura (Entrega 2)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-2)
* [Entrega 3](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-3)

## Experimentación

En nuestro experimento implementamos una prueba de concepto para el escenario de calidad 1: [Consultas masivas con filtros específicos de metadata](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-3#211-consultas-masivas-con-filtros-espec%C3%ADficos-de-metadata).

### Componentes

1. **Servicio de Ingestión**
   - Funcionalidad básica para generar eventos de dominio (Evento Gordo)
   - Publica eventos en el broker de mensajería
   - Almacena información en Postgres (simulando un data lake)

2. **Manejador de Eventos**
   - Parte del subdominio de consultas
   - Procesa eventos de dominio
   - Genera comandos para los manejadores de filtros

3. **Manejadores de Filtros**
   - Implementados: Demografía y Modalidad
   - Reciben comandos para almacenamiento de entidades
   - Bases de datos independientes (Postgres)

4. **Servicio de Consultas**
   - Integra resultados de los filtros
   - API sincrónica (considerado asincrónico con sagas para escenarios reales, implementado sincrónicamente para simplificar esta prueba de concepto)
   - Obtiene agregación completa usando IDs de los filtros

### Configuración del Ambiente

#### Prerrequisitos

- Docker
- Docker Compose
- Postman

#### Instalación

1. Clone el repositorio
2. Navegue al directorio del proyecto
3. Ejecute el ambiente con Docker Compose:
   ```bash
   cd SaludTechDeLosAlpes
   docker-compose up -d
   ```

### Datos de Prueba

1. Descargue el set de datos de prueba desde [Mockaroo](https://my.api.mockaroo.com/salud_tech_imagen.json?key=062d8850)

2. Configure Postman:
   - Importe la colección desde: `SaludTechDeLosAlpes/postman/SaludTechDeLosAlpes.postman_collection.json`
   - Ajuste las variables de entorno de ser necesario para apuntar a los servicios locales

3. Cargue los datos:
   - Use el endpoint `CrearImagen` en Postman
   - Use el archivo descargado como fuente de datos

### Ejecución de Pruebas

- Compare tiempos de respuesta entre endpoints de consulta
- Recomendación: Use un subconjunto de datos para reducir tiempos de prueba
- Opcionalmente puede usar el archivo completo para pruebas exhaustivas

### Escenarios de Prueba

1. **Con Filtros**
   - Implementación con dos copias de cada filtro (Demografía y Modalidad)
   - Permite consultas específicas por metadata

2. **Sin Filtros**
   - Implementación base para comparación de rendimiento
   - Consultas directas sin optimización
