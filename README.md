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
* [Escenarios de Calidad (Entrega 3)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-3)
* [Implementación (Entrega 4)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-4)


### Componentes

1. **Servicio de Ingestión**
   - Funcionalidad básica para generar eventos de dominio (Evento Gordo)
   - Publica eventos en el broker de mensajería
   - Almacena información en Postgres (simulando un data lake)
   - Orquesta la Saga de Ingestion

2. **Ingestion - Anominización**
   - Procesa comandos de dominio
   - Genera eventos de dominio
   - Publica eventos en el broker de mensajería

3. **Ingestion - Metadata**
   - Procesa comandos de dominio
   - Genera eventos de dominio
   - Publica eventos en el broker de mensajería

4. **Servicio de Consultas**
   - Parte del subdominio de consultas
   - Procesa eventos de dominio
   - Genera comandos para los manejadores de filtros
   - Orquesta la Saga de Consultas

5. **Manejadores de Filtros - Consultas**
   - Implementados: Demografía y Modalidad
   - Reciben comandos para almacenamiento de entidades
   - Bases de datos independientes (Postgres)

6. **Servicio de Consultas**
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
   - Use el endpoint `CrearImagen Saga` en Postman
   - Use el archivo descargado como fuente de datos

### Ejecución de Pruebas

- Ejecute un llamado a `ConsultaUsandoFiltros Saga` en Postman
- Verifique la respuesta y copie el id de la saga generada
- Ejecute un llamado a `Consultar Saga Ingestion` en Postman con el id de la saga
- Verifique la respuesta y el estado de la saga
