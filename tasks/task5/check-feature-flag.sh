#!/bin/bash


echo "▶️ Проверка Feature Flag (X-Feature-Enabled: true)..."

# Отправляем запрос с заголовком, чтобы маршрутизировать трафик на `v2`
curl -s -H "X-Feature-Enabled: true" http://localhost:80/ping
