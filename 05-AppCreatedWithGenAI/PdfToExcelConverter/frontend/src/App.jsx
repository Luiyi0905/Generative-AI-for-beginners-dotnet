import React, { useState } from 'react';
import axios from 'axios';
import { Upload, FileSpreadsheet, FileText, CheckCircle, AlertCircle, Loader2 } from 'lucide-react';

const API_BASE_URL = 'http://localhost:5000';

function App() {
  const [file, setFile] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [extracting, setExtracting] = useState(false);
  const [serverFile, setServerFile] = useState(null);
  const [pageCount, setPageCount] = useState(0);
  const [selectedPages, setSelectedPages] = useState('');
  const [extractAll, setExtractAll] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile && selectedFile.type === 'application/pdf') {
      setFile(selectedFile);
      setError('');
    } else {
      setError('Por favor seleccione un archivo PDF válido.');
      setFile(null);
    }
  };

  const uploadFile = async () => {
    if (!file) return;
    setUploading(true);
    setError('');
    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await axios.post(`${API_BASE_URL}/upload`, formData);
      setServerFile(response.data.filename);
      setPageCount(response.data.page_count);
      setSuccess('Archivo subido correctamente.');
    } catch (err) {
      setError('Error al subir el archivo: ' + (err.response?.data?.error || err.message));
    } finally {
      setUploading(false);
    }
  };

  const extractTables = async () => {
    if (!serverFile) return;
    setExtracting(true);
    setError('');
    setSuccess('');

    const pages = [];
    selectedPages.split(',').forEach(part => {
      if (part.includes('-')) {
        const [start, end] = part.split('-').map(p => parseInt(p.trim()));
        if (!isNaN(start) && !isNaN(end)) {
          for (let i = start; i <= end; i++) pages.push(i);
        }
      } else {
        const p = parseInt(part.trim());
        if (!isNaN(p)) pages.push(p);
      }
    });

    try {
      const response = await axios.post(`${API_BASE_URL}/extract`, {
        filename: serverFile,
        pages: pages,
        extract_all: extractAll
      }, { responseType: 'blob' });

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `${serverFile.split('.')[0]}_tablas.xlsx`);
      document.body.appendChild(link);
      link.click();
      setSuccess('Extracción completada. El archivo Excel se está descargando.');
    } catch (err) {
      setError('Error en la extracción: ' + (err.response?.data?.error || 'Error desconocido'));
    } finally {
      setExtracting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col items-center py-12 px-4">
      <header className="mb-10 text-center">
        <h1 className="text-4xl font-bold text-blue-800 mb-2 flex items-center justify-center">
          <FileSpreadsheet className="mr-2" /> PDF a Excel
        </h1>
        <p className="text-gray-600">Extrae tablas de tus documentos PDF de forma selectiva</p>
      </header>

      <main className="w-full max-w-2xl bg-white rounded-xl shadow-lg p-8">
        {!serverFile ? (
          <div className="space-y-6">
            <div className="border-2 border-dashed border-gray-300 rounded-lg p-10 text-center hover:border-blue-500 transition-colors">
              <input
                type="file"
                accept=".pdf"
                onChange={handleFileChange}
                className="hidden"
                id="pdf-upload"
              />
              <label htmlFor="pdf-upload" className="cursor-pointer flex flex-col items-center">
                <Upload className="w-12 h-12 text-gray-400 mb-4" />
                <span className="text-lg font-medium text-gray-700">
                  {file ? file.name : 'Haz clic para seleccionar o arrastra un PDF'}
                </span>
                <span className="text-sm text-gray-500 mt-2">Solo archivos PDF</span>
              </label>
            </div>

            {file && (
              <button
                onClick={uploadFile}
                disabled={uploading}
                className="w-full bg-blue-600 text-white py-3 rounded-lg font-semibold hover:bg-blue-700 disabled:bg-blue-300 flex items-center justify-center transition-colors"
              >
                {uploading ? (
                  <><Loader2 className="animate-spin mr-2" /> Subiendo...</>
                ) : (
                  'Continuar a la extracción'
                )}
              </button>
            )}
          </div>
        ) : (
          <div className="space-y-6">
            <div className="flex items-center p-4 bg-blue-50 rounded-lg">
              <FileText className="text-blue-600 mr-3" />
              <div>
                <p className="font-semibold text-blue-900">{serverFile}</p>
                <p className="text-sm text-blue-700">{pageCount} páginas encontradas</p>
              </div>
              <button
                onClick={() => {setServerFile(null); setFile(null); setSuccess('');}}
                className="ml-auto text-sm text-blue-600 hover:underline"
              >
                Cambiar archivo
              </button>
            </div>

            <div className="space-y-4">
              <h3 className="font-medium text-gray-800">Opciones de extracción:</h3>

              <div className="flex items-center space-x-4">
                <label className="flex items-center">
                  <input
                    type="radio"
                    checked={extractAll}
                    onChange={() => setExtractAll(true)}
                    className="mr-2"
                  />
                  Extraer todo el documento
                </label>
                <label className="flex items-center">
                  <input
                    type="radio"
                    checked={!extractAll}
                    onChange={() => setExtractAll(false)}
                    className="mr-2"
                  />
                  Seleccionar páginas
                </label>
              </div>

              {!extractAll && (
                <div className="mt-2">
                  <label className="block text-sm text-gray-600 mb-1">
                    Números de página (separados por coma, ej: 1, 3, 5-7):
                  </label>
                  <input
                    type="text"
                    value={selectedPages}
                    onChange={(e) => setSelectedPages(e.target.value)}
                    placeholder="Ej: 1, 2, 5"
                    className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                  />
                </div>
              )}

              <button
                onClick={extractTables}
                disabled={extracting}
                className="w-full bg-green-600 text-white py-3 rounded-lg font-semibold hover:bg-green-700 disabled:bg-green-300 flex items-center justify-center transition-colors mt-6"
              >
                {extracting ? (
                  <><Loader2 className="animate-spin mr-2" /> Procesando...</>
                ) : (
                  <><FileSpreadsheet className="mr-2" /> Convertir a Excel</>
                )}
              </button>
            </div>
          </div>
        )}

        {error && (
          <div className="mt-6 p-4 bg-red-50 border-l-4 border-red-500 text-red-700 flex items-start">
            <AlertCircle className="mr-3 shrink-0" />
            <p>{error}</p>
          </div>
        )}

        {success && (
          <div className="mt-6 p-4 bg-green-50 border-l-4 border-green-500 text-green-700 flex items-start">
            <CheckCircle className="mr-3 shrink-0" />
            <p>{success}</p>
          </div>
        )}
      </main>

      <footer className="mt-12 text-gray-500 text-sm">
        PDF to Excel Converter &copy; {new Date().getFullYear()}
      </footer>
    </div>
  );
}

export default App;
