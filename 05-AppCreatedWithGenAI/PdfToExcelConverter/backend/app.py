import os
import io
import pandas as pd
import pdfplumber
from flask import Flask, request, send_file, jsonify
from flask_cors import CORS
from werkzeug.utils import secure_filename

app = Flask(__name__)
CORS(app)

UPLOAD_FOLDER = 'uploads'
if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

@app.route('/upload', methods=['POST'])
def upload_file():
    if 'file' not in request.files:
        return jsonify({"error": "No file part"}), 400
    file = request.files['file']
    if file.filename == '':
        return jsonify({"error": "No selected file"}), 400

    if file and file.filename.endswith('.pdf'):
        filename = secure_filename(file.filename)
        filepath = os.path.join(UPLOAD_FOLDER, filename)
        file.save(filepath)

        try:
            with pdfplumber.open(filepath) as pdf:
                page_count = len(pdf.pages)
            return jsonify({
                "filename": filename,
                "page_count": page_count,
                "message": "File uploaded successfully"
            }), 200
        except Exception as e:
            return jsonify({"error": str(e)}), 500

    return jsonify({"error": "Invalid file type. Please upload a PDF."}), 400

@app.route('/extract', methods=['POST'])
def extract_tables():
    data = request.json
    filename = data.get('filename')
    pages = data.get('pages', []) # List of page numbers (1-indexed)
    extract_all = data.get('extract_all', False)

    if not filename:
        return jsonify({"error": "No filename provided"}), 400

    filepath = os.path.join(UPLOAD_FOLDER, filename)
    if not os.path.exists(filepath):
        return jsonify({"error": "File not found"}), 404

    try:
        all_tables = []
        with pdfplumber.open(filepath) as pdf:
            if extract_all:
                pages_to_extract = range(len(pdf.pages))
            else:
                pages_to_extract = [p - 1 for p in pages if 0 < p <= len(pdf.pages)]

            for p_idx in pages_to_extract:
                page = pdf.pages[p_idx]
                tables = page.extract_tables()
                for table in tables:
                    if table:
                        df = pd.DataFrame(table[1:], columns=table[0])
                        all_tables.append(df)

        if not all_tables:
            return jsonify({"error": "No tables found in the selected pages"}), 400

        # Combine all tables into a single Excel file with multiple sheets if necessary,
        # or just one after another in the same sheet.
        output = io.BytesIO()
        with pd.ExcelWriter(output, engine='openpyxl') as writer:
            for i, df in enumerate(all_tables):
                sheet_name = f'Table_{i+1}'
                df.to_excel(writer, sheet_name=sheet_name[:31], index=False)

        output.seek(0)

        return send_file(
            output,
            as_attachment=True,
            download_name=f"{os.path.splitext(filename)[0]}_extracted.xlsx",
            mimetype="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        )

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True, port=5000)
