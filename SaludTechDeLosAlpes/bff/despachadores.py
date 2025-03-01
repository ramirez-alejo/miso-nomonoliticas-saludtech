import uuid
import datetime
from bff.api.v1.esquemas import IngestionResponse, IngestionInput

class Despachador:
    def procesar_ingestion(self, input: IngestionInput) -> IngestionResponse:
        imagen_id = input.ImagenId
        version = input.Version
        tipo_imagen = input.TipoImagen
        atributos_imagen = input.AtributosImagen
        contexto_procesal = input.ContextoProcesal
        metadatos = input.Metadatos
        paciente = input.Paciente

        # Logs detallados
        print(f"[INFO] Iniciando procesamiento de ingestión para Imagen ID: {imagen_id}")
        print(f"[INFO] Versión: {version}")
        
        # Información del tipo de imagen
        print(f"[INFO] Tipo de Imagen:")
        print(f"  - Modalidad: {tipo_imagen.Modalidad.Nombre} ({tipo_imagen.Modalidad.Descripcion})")
        print(f"  - Región Anatómica: {tipo_imagen.RegionAnatomica.Nombre} ({tipo_imagen.RegionAnatomica.Descripcion})")
        print(f"  - Patología: {tipo_imagen.Patologia.Descripcion}")

        # Atributos de la imagen
        print(f"[INFO] Atributos de Imagen:")
        print(f"  - Resolución: {atributos_imagen.Resolucion}")
        print(f"  - Contraste: {atributos_imagen.Contraste}")
        print(f"  - Es 3D: {atributos_imagen.Es3D}")
        print(f"  - Fase Escáner: {atributos_imagen.FaseEscaner}")

        # Contexto procesal
        print(f"[INFO] Contexto Procesal: {contexto_procesal.Etapa}")

        # Metadatos
        print(f"[INFO] Entorno Clínico: {metadatos.EntornoClinico.TipoAmbiente}")
        print(f"[INFO] Síntomas asociados:")
        for sintoma in metadatos.Sintomas:
            print(f"  - {sintoma.Descripcion}")

        # Datos del paciente
        print(f"[INFO] Datos del Paciente:")
        print(f"  - Token Anónimo: {paciente.TokenAnonimo}")
        print(f"  - Grupo de Edad: {paciente.Demografia.GrupoEdad}")
        print(f"  - Sexo: {paciente.Demografia.Sexo}")
        print(f"  - Etnicidad: {paciente.Demografia.Etnicidad}")

        print(f"[INFO] Historial Médico:")
        print(f"  - Fumador: {'Sí' if paciente.Historial.Fumador else 'No'}")
        print(f"  - Diabético: {'Sí' if paciente.Historial.Diabetico else 'No'}")
        print(f"  - Condiciones Previas: {', '.join(paciente.Historial.CondicionesPrevias) if paciente.Historial.CondicionesPrevias else 'Ninguna'}")

        return IngestionResponse(
            status="success",
            message=f"Ingestión procesada correctamente para la Imagen ID {imagen_id}."
        )
