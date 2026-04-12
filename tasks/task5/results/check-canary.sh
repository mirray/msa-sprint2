#!/bin/bash


echo "▶️ Checking canary release (90% v1, 10% v2)..."

# Посылаем 100 запросов
for i in $(seq 1 100);
do
  curl -s "http://localhost:80/ping"
  echo ""
done