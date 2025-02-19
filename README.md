# Diseño y construcción de soluciones no monolíticas

## Grupo 20
Integrantes:

* Augusto Romero
* Juan David Orduz
* Camilo Barreto
* Alejandro Ramirez

## Descripción
Este proyecto contiene el codigo del proyecto del curso para el grupo 20.

## [Context Mapper (Entrega 1 y 2)](https://github.com/ramirez-alejo/miso-nomonoliticas-saludtech/blob/master/ContextMapper/README.md)


## Diseño Experimentación (Entrega 3)

###  1. Atributos de calidad definidos en la entrega 2: 

- Seguridad 

- Escalabilidad 

- Disponibilidad 

### 2. Definición de los 3 escenarios de calidad para cada atributo de calidad 

#### 2.1. Escenarios de  Escalabilidad
##### 2.1.1.  Escenario de pico de ingesta de datos o lectura de datos
| Escenario #: 1 | Escenario de pico de ingesta y consulta de datos de manera aleatoria |
|----------------|-------------------------------------------------------------------------------------------|
| Fuente | Alta demanda de diferentes usuarios y sistemas, trabajos de ingestión procesando nuevas imágenes médicas, Clientes consultando datos para entrenar modelos de IA y realizar análisis.  |
| Estímulo | Pico de ingesta de datos y consultas de manera aleatoria |
| Ambiente | Producción, en operación normal |
| Artefacto | Modulo de Ingestión y Anonimización, modulo de consultas y sus bases de datos asociadas |
| Respuesta | El sistema escala horizontalmente de manera independiente el modulo d eingestion y de consultas para manejar la demanda, sin que al alta demanda de ingestion afecte la consulta de datos  y viceversa |
| Medida de la respuesta | (a) 100% de las peticiones de ingestion y consulta son atendidas<br>(b) El tiempo de respuesta de las peticiones de ingestion y consulta es menor a 1 segundo <br>(c) El sistema escala horizontalmente de manera independiente el modulo de ingestion y de consultas para manejar la demanda <br>(d) Una alta demanda de ingestion no genera deadlosk en las consultas y viceversa |

| Decisión arquitectural | Punto de Sensibilidad | Trade-off | Riesgo |
|------------------------|-----------------------|-----------|--------|
| Usar CQRS para separar la escritura de la lectura | Modulo de Ingestión y Anonimización, modulo de consultas | Usar CQRS puede aumentar la complejidad de la arquitectura | Aumentar la complejidad de la arquitectura e incrementar el tiempo de desarrollo
| Usar una arquitectura basada en eventos con broker de mensajeria | Modulo de Ingestión y Anonimización, modulo de consultas | Usar una arquitectura basada en eventos con broker de mensajeria puede aumentar la complejidad de la arquitectura | Aumentar la complejidad de la arquitectura e incrementar el tiempo de desarrollo

- *Diagrama*:

- *Racionale*:
El uso de CQRS permite separar la escritura de la lectura, permitiendo escalar de manera independiente el modulo de ingestion y de consultas para manejar la demanda. Por otro lado, el uso de una arquitectura basada en eventos con broker de mensajeria permite escalar de manera horizontal los diferentes modulos del sistema y agrega resilencia al sistema.


##### 2.1.2. Escenario de acaparamiento de recursos por parte de un usuario
| Escenario #: 2 | Un usuario acapara los recursos del sistema |
|----------------|-------------------------------------------------------------------------------------------|
| Fuente | Un usuario |
| Estímulo | Un usuario realiza una gran cantidad de peticiones a la API |
| Ambiente | Producción, en operación normal |
| Artefacto | Modulo de consultas |
| Respuesta | El sistema enruta las peticiones de un usuario a la misma instancia del modulo de consultas, evitando que un usuario acapare y afecte el rendimiento para otros usuarios |
| Medida de la respuesta | (a) 100% de las peticiones de consulta son atendidas<br>(b) El tiempo de respuesta de las peticiones de consulta es menor a 1 segundo <br>(c) El sistema enruta las peticiones de un usuario a la misma instancia del modulo de consultas, evitando que un usuario acapare y afecte el rendimiento para otros usuarios |

| Decisión arquitectural | Punto de Sensibilidad | Trade-off | Riesgo |
|------------------------|-----------------------|-----------|--------|
| Uso del patron Bulkhead | Modulo de consultas | Usar el patron Bulkhead requiere la introducion de un nuevo componente en la arquitectura | - Aumentar la complejidad de la arquitectura e incrementar el tiempo de desarrollo <br> - Si el sistema cuenta con pocos usuarios, el uso del patron Bulkhead puede ser innecesario
| Uso de un balanceador de carga | Modulo de consultas | Usar un balanceador de carga puede aumentar la complejidad de la arquitectura | Aumentar la complejidad de la arquitectura e incrementar el tiempo de desarrollo

- *Diagrama*:

- *Racionale*: 
El uso del patron Bulkhead permite aislar los recursos de un usuario, evitando que un usuario acapare los recursos del sistema y afecte el rendimiento para otros usuarios. Por medio del uso de un balanceador de carga se puede enrutar las peticiones de un usuario a la misma instancia del modulo de consultas, asi si genera una demanda muy alta de peticiones no afectara el rendimiento de otros usuarios.


##### 2.1.3. Anomalía de Recursos (Memory Leak) en el Microservicio de Ingestión
| Escenario #: 3 | Un memory leak en el microservicio de Ingestión |
|----------------|-------------------------------------------------------------------------------------------|
| Fuente | El microservicio de Ingestión |
| Estímulo | Un memory leak en el microservicio de Ingestión |
| Ambiente | Producción, en operación normal |
| Artefacto | Modulo de Ingestión |
| Respuesta | El sistema detecta el memory leak y reinicia el microservicio de Ingestión |
| Medida de la respuesta | (a) 100% de las peticiones de ingestion son atendidas<br>(b) El tiempo de respuesta de las peticiones de ingestion es menor a 1 segundo <br>(c) El sistema detecta el memory leak y reinicia el microservicio de Ingestión en menos de 60 segundos |

| Decisión arquitectural | Punto de Sensibilidad | Trade-off | Riesgo |
|------------------------|-----------------------|-----------|--------|
| Uso de un orquestador de contenedores | Modulo de Ingestión | Usar un orquestador de contenedores puede aumentar la complejidad de la arquitectura | Aumentar la complejidad de la arquitectura e incrementar el tiempo de desarrollo
| Uso de un sistema de monitoreo | Modulo de Ingestión | Usar un sistema de monitoreo puede aumentar la complejidad de la arquitectura | Aumentar la complejidad de la arquitectura e incrementar el tiempo de desarrollo

- *Diagrama*:

- *Racionale*:
El uso de un orquestador de contenedores permite reiniciar el microservicio de Ingestión en caso de un memory leak al combinarlo con el uso de un sistema de monitoreo permite detectar el memory leak y reiniciar el microservicio de Ingestión en menos de 60 segundos.




#### 2.2. Escenarios de Seguridad 

##### 2.2.1.  Protección contra intrusión maliciosa (ataque externo)

| Escenario #: 4 | Un atacante externo intenta realizar una inyección de código en endpoints críticos de la API |
|----------------|-------------------------------------------------------------------------------------------|
| Fuente | Un atacante externo |
| Estímulo | Intento de inyección de código (SQL injection o similar) en uno de los endpoints críticos de la API |
| Ambiente | Producción, en operación normal |
| Artefacto | Servicio de Ingestión y Anonimización y sus bases de datos asociadas |
| Respuesta | El sistema rechaza las peticiones maliciosas y registra el evento para auditoría, sin filtrar información sensible ni comprometer la base de datos |
| Medida de la respuesta | (a) 100% de los datos sensibles permanecen protegidos<br>(b) Generación de alerta en el sistema de monitoreo en < 60 segundos |

| Decisión arquitectural | Punto de Sensibilidad | Trade-off | Riesgo |
|------------------------|-----------------------|-----------|--------|
| Añadir un sidecar de seguridad con logica para validar las peticiones | Servicio de Ingestión y Anonimización | Añadir un sidecar de seguridad puede aumentar la complejidad de la arquitectura | Reducir la velocidad de ingestion y anonimizacion
| Añadir un firewall de aplicaciones web | Servicio de Ingestión y Anonimización | Añadir un firewall de aplicaciones web puede aumentar la complejidad de la arquitectura | Rechazar peticiones legítimas


- *Diagrama*:

- *Racionale*:
El uso de un sidecar de seguridad con logica para validar las peticiones permite rechazar las peticiones maliciosas y registrar el evento para auditoría, sin filtrar información sensible ni comprometer la base de datos. Por otro lado, el uso de un firewall de aplicaciones web permite rechazar las peticiones maliciosas y registrar el evento para auditoría, sin filtrar información sensible ni comprometer la base de datos.

##### 2.2.2.  Protección contra fuga de información (ataque interno)

| Escenario #: 5 | Un usuario interno administrador intenta acceder a información  sin anonimizar de un proveedor diferente al pais diferente al pais donde se encuentra asignado |
|----------------|-------------------------------------------------------------------------------------------|
| Fuente | Un usuario interno administrador |
| Estímulo | Intento de acceso a información sin anonimizar de un proveedor diferente al pais donde se encuentra asignado |
| Ambiente | Producción, en operación normal | 
| Artefacto | Servicio de Ingestión y Anonimización y sus bases de datos temporales |
| Respuesta | El sistema rechaza las peticiones maliciosas y registra el evento para auditoría, sin filtrar información sensible ni comprometer la base de datos |
| Medida de la respuesta | (a) 100% de los datos sensibles permanecen protegidos<br>(b) Generación de alerta en el sistema de monitoreo en < 60 segundos |

| Decisión arquitectural | Punto de Sensibilidad | Trade-off | Riesgo |
|------------------------|-----------------------|-----------|--------|
| Añadir un sidecar de seguridad con logica para validar las peticiones | Servicio de Ingestión y Anonimización | Añadir un sidecar de seguridad puede aumentar la complejidad de la arquitectura | Reducir la velocidad de ingestion y anonimizacion
| Añadir un firewall de aplicaciones web | Servicio de Ingestión y Anonimización | Añadir un firewall de aplicaciones web puede aumentar la complejidad de la arquitectura | Rechazar peticiones legítimas

- *Diagrama*:

- *Racionale*:
El uso de un sidecar de seguridad con logica para validar las peticiones permite rechazar las peticiones maliciosas y registrar el evento para auditoría, sin filtrar información sensible ni comprometer la base de datos. Por otro lado, el uso de un firewall de aplicaciones web permite rechazar las peticiones maliciosas y registrar el evento para auditoría, sin filtrar información sensible ni comprometer la base de datos.



##### 2.2.3.  Escenario de manipulación de datos en tránsito 

| Escenario #: 6 | Un atacante intenta manipular los datos en tránsito |
|----------------|-------------------------------------------------------------------------------------------|
| Fuente | Un atacante |
| Estímulo | Intento de manipulación de los datos en tránsito |
| Ambiente | Producción, en operación normal |
| Artefacto | Servicio de Ingestión y Anonimización y sus bases de datos temporales |
| Respuesta | El sistema rechaza las peticiones maliciosas y registra el evento para auditoría, sin filtrar información sensible ni comprometer la base de datos |
| Medida de la respuesta | (a) 100% de los datos sensibles permanecen protegidos<br>(b) Generación de alerta en el sistema de monitoreo en < 60 segundos |

| Decisión arquitectural | Punto de Sensibilidad | Trade-off | Riesgo |
|------------------------|-----------------------|-----------|--------|
| Uso de un protocolo de comunicación seguro | Servicio de Ingestión y Anonimización | Usar un protocolo de comunicación seguro puede aumentar la complejidad de la arquitectura | Aumentar la complejidad de la arquitectura e incrementar el tiempo de desarrollo
| Agregar una firma a los mensajes usando un certificado digital a acada uno de los eventos generados en el sistema, para garantizar la integridad de los datos | Servicio de Ingestión y Anonimización | Agregar una firma a los mensajes usando un certificado digital requiere de trabajo adicional de infraestructura para la generacion y manejo de los certificados | Vencimiento de los certificados o perdida de mensajes por problemas de sincronización (generacion de la firma con un certificado y verificacion con otro)

- *Diagrama*:

- *Racionale*:
El uso de un protocolo de comunicación seguro permite rechazar las peticiones maliciosas y registrar el evento para auditoría, admitir datos manipulados ni comprometer la base de datos. Por ejemplo un proveedor podria intentar manipular los datos de otros proveedores para simular que le pertenecen y asi al momento de la compensacion se le pague a el, si el mensaje es firmado con un certificado digital se garantiza la integridad de los datos y se evita la manipulacion de los datos en transito ya que al hacer cualquier cambio en el mensaje la firma no coincidira con el mensaje y se rechazara la peticion.
