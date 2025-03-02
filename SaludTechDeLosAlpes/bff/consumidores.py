import pulsar
import json


#python consumidores.py

class Consumidor:
    def __init__(self):
        # Configura la conexión a Pulsar
        self.client = pulsar.Client('pulsar://localhost:6650')  # URL del servidor Pulsar
        self.consumer = self.client.subscribe('ingestion-anonimizar', 'my-subscription')  # Tópico y nombre de la suscripción

    def consumir_mensajes(self):
        # Escuchar mensajes
        while True:
            msg = self.consumer.receive()
            try:
                # Procesar el mensaje
                mensaje = msg.data().decode('utf-8')  # Decodificar el mensaje de bytes a string
                mensaje_json = json.loads(mensaje)  # Convertir el JSON de string a diccionario

                # Aquí puedes hacer lo que necesites con el mensaje, como loguearlo o procesarlo
                print(f"[INFO] Mensaje recibido: {mensaje_json}")

                # Confirmar que el mensaje fue procesado correctamente
                self.consumer.acknowledge(msg)

            except Exception as e:
                print(f"[ERROR] Error al procesar el mensaje: {str(e)}")
                self.consumer.negative_acknowledge(msg)

    def close(self):
        self.consumer.close()
        self.client.close()

if __name__ == "__main__":
    consumidor = Consumidor()
    try:
        consumidor.consumir_mensajes()
    except KeyboardInterrupt:
        print("[INFO] Interrumpido por el usuario")
    finally:
        consumidor.close()