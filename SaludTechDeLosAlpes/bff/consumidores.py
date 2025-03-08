import os
import pulsar
import json

class Consumidor:
    def __init__(self):
        PULSAR_ENV: str = 'MessageBroker__Host'

        def broker_host():
            return os.getenv(PULSAR_ENV, default="localhost")

        # Configura la conexión a Pulsar
        self.client = pulsar.Client(f'pulsar://{broker_host()}:6650')  # URL del servidor Pulsar
        self.consumer = self.client.subscribe('imagen-anonimizar', 'my-subscription')  # Tópico y nombre de la suscripción
        
        print("[INFO] Consumidor iniciado. Esperando mensajes...")  # Mensaje al iniciar

    def consumir_mensajes(self):
        # Escuchar mensajes
        while True:
            print("[INFO] Esperando un nuevo mensaje...")  # Mensaje para ver que sigue en ejecución
            msg = self.consumer.receive()
            try:
                # Procesar el mensaje
                mensaje = msg.data().decode('utf-8')  # Decodificar el mensaje de bytes a string
                mensaje_json = json.loads(mensaje)  # Convertir el JSON de string a diccionario

                print(f"[INFO] Mensaje recibido: {mensaje_json}")  # Mensaje recibido

                # Confirmar que el mensaje fue procesado correctamente
                self.consumer.acknowledge(msg)

            except Exception as e:
                print(f"[ERROR] Error al procesar el mensaje: {str(e)}")
                self.consumer.negative_acknowledge(msg)

    def close(self):
        print("[INFO] Cerrando consumidor...")
        self.consumer.close()
        self.client.close()
        print("[INFO] Consumidor cerrado correctamente.")

if __name__ == "__main__":
    print("[INFO] Iniciando el consumidor...")
    consumidor = Consumidor()
    try:
        consumidor.consumir_mensajes()
    except KeyboardInterrupt:
        print("[INFO] Interrumpido por el usuario")
    finally:
        consumidor.close()
