import os
import pulsar
import json
from api.v1.esquemas import IngestionResponse, IngestionInput

class Despachador:
    def __init__(self):

        self.client = pulsar.Client(f'pulsar://{broker_host()}:6650')  
        self.producer = self.client.create_producer('ingestion-anonimizar')

    def procesar_ingestion(self, input: IngestionInput) -> IngestionResponse:
        imagen_id = input.ImagenId
        version = input.Version
        tipo_imagen = input.TipoImagen
        atributos_imagen = input.AtributosImagen
        contexto_procesal = input.ContextoProcesal
        metadatos = input.Metadatos
        paciente = input.Paciente

        # Crear el objeto a enviar a Pulsar (aquí se usa un diccionario que luego convertimos a JSON)
        mensaje = {
            'imagen_id': imagen_id,
            'version': version,
            'tipo_imagen': {
                'modalidad': tipo_imagen.Modalidad.Nombre,
                'descripcion_modalidad': tipo_imagen.Modalidad.Descripcion,
                'region_anatomica': tipo_imagen.RegionAnatomica.Nombre,
                'descripcion_region_anatomica': tipo_imagen.RegionAnatomica.Descripcion,
                'patologia': tipo_imagen.Patologia.Descripcion,
            },
            'atributos_imagen': {
                'resolucion': atributos_imagen.Resolucion,
                'contraste': atributos_imagen.Contraste,
                'es_3d': atributos_imagen.Es3D,
                'fase_escaner': atributos_imagen.FaseEscaner,
            },
            'contexto_procesal': {
                'etapa': contexto_procesal.Etapa
            },
            'metadatos': {
                'entorno_clinico': metadatos.EntornoClinico.TipoAmbiente,
                'sintomas': [sintoma.Descripcion for sintoma in metadatos.Sintomas],
            },
            'paciente': {
                'token_anonimo': paciente.TokenAnonimo,
                'grupo_edad': paciente.Demografia.GrupoEdad,
                'sexo': paciente.Demografia.Sexo,
                'etnicidad': paciente.Demografia.Etnicidad,
                'historial': {
                    'fumador': 'Sí' if paciente.Historial.Fumador else 'No',
                    'diabetico': 'Sí' if paciente.Historial.Diabetico else 'No',
                    'condiciones_previas': ', '.join(paciente.Historial.CondicionesPrevias) if paciente.Historial.CondicionesPrevias else 'Ninguna',
                }
            }
        }

        mensaje_json = json.dumps(mensaje)
        
        try:
            message_id = self.producer.send(mensaje_json.encode('utf-8'))

            # Confirmar que el mensaje ha sido enviado y registrado en el tópico de Pulsar
            print(f"[INFO] El mensaje con Imagen ID: {imagen_id} ha sido recibido en el tópico 'ingestion-anonimizar'.")
            print(f"[INFO] Message ID: {message_id}")
            print(f"[INFO] Mensaje enviado correctamente: {mensaje_json}")

            return IngestionResponse(
                status="success",
                message=f"Ingestión procesada correctamente para la Imagen ID {imagen_id}. Message ID: {message_id}"
            )
        
        except Exception as e:
            print(f"[ERROR] Error al enviar el mensaje: {str(e)}")
            return IngestionResponse(
                status="error",
                message=f"Error al enviar el mensaje a Pulsar: {str(e)}"
            )

    def close(self):
        self.producer.close()

PULSAR_ENV: str = 'MessageBroker__Host'
def broker_host():
    return os.getenv(PULSAR_ENV, default="localhost")