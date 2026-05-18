# Smart PDF Table Extractor / Extractor Inteligente de Tablas PDF

## English

### Description
Smart PDF Table Extractor is a .NET 9 Blazor application that allows users to selectively extract tables from PDF documents using Generative AI (GPT-4o). Users can upload a PDF, preview it, select specific pages, and let the AI identify and format tables which can then be exported to Excel.

### Key Features
- **Selective Extraction**: Choose specific pages to process.
- **AI-Powered**: Uses Semantic Kernel and GPT-4o for intelligent table detection, even in complex layouts.
- **Live Preview**: Interactive PDF viewer using `pdf.js`.
- **Excel Export**: Download results directly as `.xlsx` files.

### Prerequisites
- .NET 10 SDK (or .NET 9)
- OpenAI API Key

### Installation & Setup
1. Clone the repository.
2. Navigate to `05-AppCreatedWithGenAI/PdfTableExtractor`.
3. Configure your OpenAI API Key in `appsettings.json` or via environment variables:
   ```json
   "OpenAI": {
     "ApiKey": "YOUR_API_KEY",
     "ModelId": "gpt-4o"
   }
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
5. Open your browser at `http://localhost:5000` (or the port indicated in the console).

---

## Español

### Descripción
Smart PDF Table Extractor es una aplicación Blazor en .NET 9 que permite a los usuarios extraer tablas de documentos PDF de forma selectiva utilizando IA Generativa (GPT-4o). Los usuarios pueden cargar un PDF, previsualizarlo, seleccionar páginas específicas y dejar que la IA identifique y formatee las tablas, las cuales pueden exportarse a Excel.

### Características Principales
- **Extracción Selectiva**: Selecciona qué páginas procesar.
- **Potenciado por IA**: Utiliza Semantic Kernel y GPT-4o para la detección inteligente de tablas, incluso en diseños complejos.
- **Previsualización en Vivo**: Visor de PDF interactivo usando `pdf.js`.
- **Exportación a Excel**: Descarga los resultados directamente como archivos `.xlsx`.

### Requisitos Previos
- .NET 10 SDK (o .NET 9)
- Clave de API de OpenAI (OpenAI API Key)

### Instalación y Configuración
1. Clone el repositorio.
2. Navegue a `05-AppCreatedWithGenAI/PdfTableExtractor`.
3. Configure su clave de API de OpenAI en `appsettings.json` o mediante variables de entorno:
   ```json
   "OpenAI": {
     "ApiKey": "TU_CLAVE_DE_API",
     "ModelId": "gpt-4o"
   }
   ```
4. Ejecute la aplicación:
   ```bash
   dotnet run
   ```
5. Abra su navegador en `http://localhost:5000` (o el puerto indicado en la consola).
