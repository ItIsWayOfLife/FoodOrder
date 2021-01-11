using Core.Constants;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Web.Reports
{
    public class ReportPDF
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _pathFont;

        public ReportPDF(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _pathFont = PathConstants.PATH_FONT_FOR_REPORT;
        }

        private int _maxColumn;
        private string _title;

        private PdfPTable _pdfTable;
        private Document _document;
        private Font _fontStyle;
        private PdfPCell _pdfPCell;

        private List<List<string>> _listReport = new List<List<string>>();

        private MemoryStream _memoryStream = new MemoryStream();

        private void EmptyRow(int nCount)
        {
            for (int i = 0; i < nCount; i++)
            {
                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = _maxColumn;
                _pdfPCell.Border = 0;
                _pdfPCell.ExtraParagraphSpace = 10;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();
            }
        }

        private float[] GetSize()
        {
            float[] sizes = new float[_maxColumn];
            for (var i = 0; i < _maxColumn; i++)
            {
                sizes[i] = 100;
            }
            return sizes;
        }

        public byte[] Report(List<List<string>> listReport, string title)
        {
            _listReport = listReport;
            _title = title;

            if (_listReport[0] != null)
                _maxColumn = _listReport[0].Count;

            _pdfTable = new PdfPTable(_maxColumn);
            _document = new Document();

            _document.SetPageSize(PageSize.A4);
            _document.SetMargins(5f, 5f, 20f, 5f);
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            string ttf = _webHostEnvironment.WebRootPath + _pathFont;
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            _fontStyle = new Font(baseFont, Font.DEFAULTSIZE, Font.NORMAL);
            PdfWriter pdfWriter = PdfWriter.GetInstance(_document, _memoryStream);

            _document.Open();

            float[] sizes = GetSize();

            _pdfTable.SetWidths(sizes);

            ReportHeader();
            EmptyRow(2);
            ReportBody();

            _pdfTable.HeaderRows = 2;
            _document.Add(_pdfTable);

            _document.Close();

            return _memoryStream.ToArray();
        }

        private void ReportHeader()
        {
            _pdfPCell = new PdfPCell(SetPageTitle());
            _pdfPCell.Colspan = _maxColumn;
            _pdfPCell.Border = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();
        }

        private void CellAddPhrase(string phrase)
        {
            _pdfPCell = new PdfPCell(new Phrase(phrase, _fontStyle));
            _pdfPCell.Colspan = _maxColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.ExtraParagraphSpace = 0;
        }

        private PdfPTable SetPageTitle()
        {
            PdfPTable pdfPTable = new PdfPTable(_maxColumn);

            CellAddPhrase(_title);
            pdfPTable.AddCell(_pdfPCell);
            pdfPTable.CompleteRow();

            CellAddPhrase($"Date and time of report { DateTime.Now.ToString()}");
            pdfPTable.AddCell(_pdfPCell);
            pdfPTable.CompleteRow();

            return pdfPTable;
        }

        private void ReportBody()
        {
            if (_listReport[0] != null)
            {
                foreach (string n in _listReport[0])
                {
                    _pdfPCell = new PdfPCell(new Phrase(n, _fontStyle));
                    AddCellForTableHeader();
                }

                _pdfTable.CompleteRow();
            }

            foreach (var reportL in _listReport)
            {
                if (_listReport.IndexOf(reportL) == 0)
                    continue;

                foreach (string reportC in reportL)
                {
                    _pdfPCell = new PdfPCell(new Phrase(reportC, _fontStyle));
                    AddCellForTableBody();
                }
            }

            _pdfTable.CompleteRow();
        }

        private void AddCellForTableHeader()
        {
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.Gray;
            _pdfTable.AddCell(_pdfPCell);
        }

        private void AddCellForTableBody()
        {
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.White;
            _pdfTable.AddCell(_pdfPCell);
        }
    }
}

