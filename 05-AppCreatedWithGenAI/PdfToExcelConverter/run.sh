#!/bin/bash

# PDF to Excel Converter - Startup Script

echo "Iniciando instalacion y arranque..."

# Backend
echo "Configurando Backend..."
cd backend
python3 -m venv venv
source venv/bin/activate
pip install -r requirements.txt
python app.py > backend.log 2>&1 &
BACKEND_PID=$!
cd ..

# Frontend
echo "Configurando Frontend..."
cd frontend
npm install
npm run dev > frontend.log 2>&1 &
FRONTEND_PID=$!
cd ..

echo "Aplicacion iniciada!"
echo "Backend corriendo en http://localhost:5000 (PID: $BACKEND_PID)"
echo "Frontend corriendo en http://localhost:5173 (PID: $FRONTEND_PID)"
echo "Presiona Ctrl+C para detener (pero los procesos seguiran en segundo plano, usa 'kill' si es necesario)"

# Mantener el script vivo o esperar
wait
