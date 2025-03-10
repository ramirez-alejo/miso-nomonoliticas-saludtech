# Diseño y construcción de soluciones no monolíticas

Proyecto del curso de soluciones no monolíticas para el grupo 20.

## Integrantes

* Augusto Romero
* Camilo Barreto
* Alejandro Ramirez
* Juan David Orduz

## Documentación

* [Arquitectura (Entrega 1)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-1)
* [Arquitectura (Entrega 2)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-2)
* [Escenarios de Calidad (Entrega 3)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-3)
* [Implementación (Entrega 4)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-4)
* [Experimentación (Entrega 5)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-5)


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

7. **BBF Ingestion**
   - Interfaz para interactuar con el servicio de Ingestión
   - Permite crear eventos de dominio
   - Permite consultar el estado de las sagas de ingestión

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

3. Ejecución de Pruebas:
   - Use el archivo descargado como fuente de datos
   - Use el endpoint `CrearImagen BFF` en Postman
   - Verifique la respuesta y copie el correlation id generado
   - Use el endpoint `Consultar BFF Ingestion` en Postman con el correlation id
   - Verifique la respuesta y el estado de la saga
   

### Despliegue en GKE

   - Se debe tener en cuenta que el despliegue en GKE requiere de una cuenta de Google Cloud y de la instalación de `gcloud` y `kubectl` en la máquina local.
   - Autenticarse con Google Cloud:
     ```bash
     gcloud auth login
     ```
   - Compilar las imágenes de Docker:
     ```bash
     docker-compose build
     ```
   - Etiquetar las imágenes para subirlas a Google Container Registry:
     ```bash
       docker tag saludtechdelosalpes_ingestion-metadata gcr.io/[PROJECT_ID]/saludtechdelosalpes_xxxx-xxxxx
       ```
   - Subir las imágenes a Google Container Registry:
       ```bash
       docker compose push
       ```
   - Generar el manifiesto de Kubernetes usando Kompose:
     ```bash
     kompose convert
     ```
   - Desplegar en GKE:
     ```bash
       kubectl apply -f .
       ```
       