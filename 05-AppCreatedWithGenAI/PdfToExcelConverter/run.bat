@echo off
echo Iniciando instalacion y arranque...

echo Configurando Backend...
cd backend
python -m venv venv
call venv\Scripts\activate
pip install -r requirements.txt
start /b python app.py
cd ..

echo Configurando Frontend...
cd frontend
npm install
start /b npm run dev -- --port 5173
cd ..

echo Aplicacion iniciada!
echo Backend corriendo en http://localhost:5000
echo Frontend corriendo en http://localhost:5173
pause
