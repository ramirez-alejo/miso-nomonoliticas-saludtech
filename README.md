# Diseño y construcción de soluciones no monolíticas

## Grupo 20
Integrantes:

* Augusto Romero
* Juan David Orduz
* Camilo Barreto
* Alejandro Ramirez

## Descripción
Este proyecto contiene el codigo del proyecto del curso para el grupo 20.


## [Arquitectura (Entrega 1)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-1)

## [Arquitectura (Entrega 2)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-2)

## [ (Entrega 3)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-3)

### Experimentacion

En nuestro experimento vamos a crear una prueba de concepto para el escenario de calidad 1 [Consultas masivas con filtros específicos de metadata](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/wiki/Entrega-3#211-consultas-masivas-con-filtros-espec%C3%ADficos-de-metadata).

Tenemos los siguientes componentes: un servicio de ingestion con una funcionalidad muy básica. En este caso, lo que queremos es que genere los eventos de dominio, como los que se generarían en una implementación completa (Evento Gordo).

Estos eventos son publicados en nuestro broker de mensajería y, a su vez, se almacena la información. Usamos Postgres para la prueba de concepto, pero en la vida real debería ser un data lake.

Contamos con un manejador de eventos que forma parte del subdominio de consultas. Este manejador toma los eventos de dominio y, basándose en ellos, genera comandos para cada uno de los manejadores de filtros.

Contamos con dos de los manejadores de filtros que se muestran en el diagrama (consideramos que 2 de los 4 son suficientes para la prueba de concepto). Escogimos Demografía y Modalidad, basándonos en la imagen que podemos ver de la UI de la solución en el PDF del planteamiento del problema. Estos manejadores de filtros reciben los comandos para el almacenamiento de las entidades y persisten la información en su propia base de datos, en este caso, Postgres.

Cuando se genera una consulta, el servicio de consultas hace un llamado a los filtros. En este caso, lo hacemos de manera sincrónica usando una API, pero consideramos que, para un escenario real, se podría implementar de manera asincrónica, posiblemente usando sagas. Dado el tiempo que tenemos, decidimos hacerlo usando una API sincrónica.

Una vez que el servicio de consultas recibe la respuesta de los filtros, utiliza los IDs retornados por estos para obtener la agregación completa de la base de datos de ingestion (que será un data lake en la vida real).

Generamos dos pruebas: una en la que tenemos dos copias de cada uno de los dos filtros, y otra sin el uso de los filtros, en la que pretendemos comparar el rendimiento obtenido mediante la implementación de esta solución.


#### Como correr el experimento

El repositorio cuenta con un archivo docker-compose.yml que permite correr el experimento. Para correrlo, se debe tener instalado Docker y Docker Compose.
En la ruta SaludTechDeLosAlpes\postman\SaludTechDeLosAlpes.postman_collection.json del repositorio contamos con la colleccion de postman que se muestra en el video de sustentaccion.
Ajuste las variables de entorno de postman para apuntar a los servicion de ingestion y consultas que se estan ejecutando en su maquina local.
Un se de datos de prueba se puede generar decargar de [aqui](https://my.api.mockaroo.com/salud_tech_imagen.json?key=062d8850)
Para cargar los datos necesarios para poder ejecutar el escenario, corra como una colleccion el endpoint CrearImagen usando como fuente el archivo descargado en el paso anterior.
Una vez cuante con datos cargados puede utilizar los endpoint de consultas, la idea es comparar los tiempo de respuesta entre los endpoint te consultas, recomendamos usar solo un sub-set the los datos para reducir el tiempo que toma correr la prueba, pero es posible usar el archivo completo.