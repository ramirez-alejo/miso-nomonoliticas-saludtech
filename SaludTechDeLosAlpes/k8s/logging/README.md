# Logging Infrastructure for SaludTech

Este directorio contiene la configuración necesaria para desplegar una infraestructura de logging centralizada para las sagas de ingestión en el cluster GKE.

## Componentes

La solución de logging está compuesta por:

1. **Loki**: Sistema de almacenamiento y consulta de logs optimizado para Kubernetes.
2. **Promtail**: Agente que recolecta logs de los pods y los envía a Loki.
3. **Grafana**: Plataforma de visualización que permite consultar y visualizar los logs almacenados en Loki.

## Arquitectura

![Arquitectura de Logging](https://miro.medium.com/max/1400/1*LnKJcvuoONUUQBrDxVVmCw.png)

1. **Promtail** se ejecuta como un DaemonSet en cada nodo del cluster, recolectando logs de todos los pods.
2. Los logs son filtrados para capturar específicamente los eventos de saga marcados con "SAGA_EVENT".
3. **Loki** almacena y indexa los logs, permitiendo consultas eficientes.
4. **Grafana** proporciona dashboards para visualizar y analizar los logs de las sagas.

## Despliegue

Para desplegar la infraestructura de logging junto con los servicios actualizados, ejecute:

```bash
# Primero, desplegar la infraestructura de logging
cd SaludTechDeLosAlpes/k8s/logging
chmod +x deploy-logging.sh
./deploy-logging.sh

# Luego, desplegar los servicios con la configuración de logging actualizada
cd ../..
kubectl apply -f anonimizacion-service-deployment.yaml
kubectl apply -f ingestion-service-deployment.yaml
kubectl apply -f metadata-service-deployment.yaml
```

Nota: Los archivos de despliegue de los servicios ya han sido actualizados para incluir la configuración de logging necesaria para integrarse con la infraestructura de logging.

## Acceso a Grafana

Una vez desplegada la infraestructura, puede acceder a Grafana mediante:

```bash
kubectl port-forward svc/grafana 3000:3000
```

Luego abra http://localhost:3000 en su navegador.

Credenciales por defecto:
- Usuario: admin
- Contraseña: admin

## Dashboard de Sagas

El despliegue incluye un dashboard preconfigurado para monitorear las sagas:

1. **Saga Events**: Muestra todos los eventos de saga en tiempo real.
2. **Saga Completion Rate**: Gráfico que muestra la tasa de sagas completadas vs. fallidas.
3. **Failed Sagas**: Tabla con detalles de las sagas que han fallado.
4. **Compensation Transactions**: Tabla que muestra las transacciones de compensación ejecutadas.

## Integración con Aplicaciones

Las aplicaciones deben utilizar la clase `SagaLogger` para registrar eventos de saga. Esta clase está implementada en:

```
SaludTechDeLosAlpes/Ingestion/Ingestion.Infraestructura/Logging/SagaLogger.cs
```

Y se utiliza en el orquestador de sagas:

```
SaludTechDeLosAlpes/Ingestion/Ingestion.Aplicacion/Sagas/ImagenIngestionSagaOrchestrator.cs
```

## Configuración

La configuración de cada componente se encuentra en los siguientes archivos:

- **Loki**: `loki-configmap.yaml`
- **Promtail**: `promtail-configmap.yaml`
- **Grafana**: `grafana-datasources-configmap.yaml` y `grafana-dashboards-configmap.yaml`

## Recursos

Los recursos asignados a cada componente están optimizados para minimizar el consumo en el cluster:

- **Loki**: 100m CPU, 128Mi memoria
- **Promtail**: 50m CPU, 64Mi memoria
- **Grafana**: 100m CPU, 128Mi memoria

Estos valores pueden ajustarse según las necesidades en los archivos de despliegue correspondientes.

## Retención de Logs

Por defecto, los logs se conservan durante 7 días (168 horas). Este valor puede modificarse en `loki-configmap.yaml` ajustando el parámetro `retention_period`.

## Solución de Problemas

Si encuentra problemas con la infraestructura de logging, puede verificar el estado de los pods:

```bash
kubectl get pods -l app=loki
kubectl get pods -l app=promtail
kubectl get pods -l app=grafana
```

Para ver los logs de cada componente:

```bash
kubectl logs -l app=loki
kubectl logs -l app=promtail
kubectl logs -l app=grafana
