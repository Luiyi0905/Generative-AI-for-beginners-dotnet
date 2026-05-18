from reportlab.lib.pagesizes import letter
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph
from reportlab.lib import colors
from reportlab.lib.styles import getSampleStyleSheet

def create_pdf(filename):
    doc = SimpleDocTemplate(filename, pagesize=letter)
    elements = []
    styles = getSampleStyleSheet()

    # Page 1
    elements.append(Paragraph("Test PDF with Tables - Page 1", styles['Title']))
    data1 = [
        ["ID", "Name", "Score"],
        ["1", "Alice", "95"],
        ["2", "Bob", "88"],
        ["3", "Charlie", "92"]
    ]
    t1 = Table(data1)
    t1.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.grey),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
        ('GRID', (0, 0), (-1, -1), 1, colors.black)
    ]))
    elements.append(t1)

    from reportlab.platypus import PageBreak
    elements.append(PageBreak())

    # Page 2
    elements.append(Paragraph("Test PDF with Tables - Page 2", styles['Title']))
    data2 = [
        ["Product", "Price", "Stock"],
        ["Apple", "1.0", "100"],
        ["Banana", "0.5", "150"],
        ["Cherry", "2.0", "50"]
    ]
    t2 = Table(data2)
    t2.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.blue),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('GRID', (0, 0), (-1, -1), 1, colors.black)
    ]))
    elements.append(t2)

    doc.build(elements)

if __name__ == "__main__":
    create_pdf("test_sample.pdf")
