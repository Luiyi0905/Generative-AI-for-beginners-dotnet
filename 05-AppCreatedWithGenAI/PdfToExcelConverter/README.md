# PDF to Excel Converter

Una aplicación web moderna para extraer tablas de documentos PDF y convertirlas a formato Excel (.xlsx) de forma selectiva.

## Características

- 📄 **Carga de PDF**: Sube cualquier documento PDF.
- 🎯 **Extracción Selectiva**: Elige extraer tablas de todo el documento o solo de páginas específicas (soporta rangos como 1-3).
- 📊 **Excel Multi-hoja**: Cada tabla detectada se guarda en una hoja separada del archivo Excel resultante.
- 💻 **Interfaz Moderna**: Construida con React y Tailwind CSS para una experiencia de usuario fluida.

---

## English Version

A modern web application to extract tables from PDF documents and convert them to Excel (.xlsx) format selectively.

## Features

- 📄 **PDF Upload**: Upload any PDF document.
- 🎯 **Selective Extraction**: Choose to extract tables from the entire document or only specific pages (supports ranges like 1-3).
- 📊 **Multi-sheet Excel**: Each detected table is saved in a separate sheet of the resulting Excel file.
- 💻 **Modern UI**: Built with React and Tailwind CSS for a smooth user experience.

---

## Requisitos / Requirements

- **Python 3.8+**
- **Node.js 18+**
- **npm**

## Instalación y Uso / Installation and Usage

### Opción 1: Usando el script automatizado (Linux/macOS)
1. Abre una terminal en la raíz del proyecto.
2. Ejecuta: `bash run.sh`
3. Abre tu navegador en `http://localhost:5173`

### Opción 2: Manual

#### Backend
```bash
cd backend
python -m venv venv
source venv/bin/activate  # En Windows: venv\Scripts\activate
pip install -r requirements.txt
python app.py
```

#### Frontend
```bash
cd frontend
npm install
npm run dev
```

## Tecnologías Utilizadas / Technologies Used

- **Backend**: Flask (Python), pdfplumber, pandas, openpyxl.
- **Frontend**: React (Vite), Tailwind CSS, Lucide React, Axios.
