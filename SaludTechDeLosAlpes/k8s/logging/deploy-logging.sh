#!/bin/bash

# Script to deploy the logging infrastructure for SaludTech

echo "Deploying Loki..."
kubectl apply -f loki-configmap.yaml
kubectl apply -f loki-deployment.yaml
kubectl apply -f loki-service.yaml

echo "Deploying Promtail..."
kubectl apply -f promtail-configmap.yaml
kubectl apply -f promtail-daemonset.yaml

echo "Deploying Grafana..."
kubectl apply -f grafana-datasources-configmap.yaml
kubectl apply -f grafana-dashboards-configmap.yaml
kubectl apply -f grafana-deployment.yaml
kubectl apply -f grafana-service.yaml

echo "Waiting for deployments to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/loki
kubectl wait --for=condition=available --timeout=300s deployment/grafana

echo "Logging infrastructure deployed successfully!"
echo "To access Grafana, run: kubectl port-forward svc/grafana 3000:3000"
echo "Then open http://localhost:3000 in your browser"
echo "Default credentials: admin/admin"
