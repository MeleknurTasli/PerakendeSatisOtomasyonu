using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class Document
    {
        Aspose.Words.Document doc;
        Aspose.Words.DocumentBuilder builder;

        public Document()
        {
            doc = new Aspose.Words.Document();
            builder = new Aspose.Words.DocumentBuilder(doc);
            builder.InsertBreak(BreakType.PageBreak);
        }

        public Document(string filepath)
        {
            doc = new Aspose.Words.Document(filepath);
            builder = new Aspose.Words.DocumentBuilder(doc);
        }

        public void SetFontSytle(int size, System.Drawing.Color color, string fontWeight, string fontWeight2, string fontFamily, Underline underline)
       {
            Aspose.Words.Font font = builder.Font;
            font.Size = size;
            font.Color = color;
            if(!String.IsNullOrWhiteSpace(fontFamily)) font.Name = fontFamily;
            if (fontWeight=="Bold") font.Bold = true;
            else if (fontWeight == "Italic") font.Italic = true;
            if (fontWeight2 == "Bold") font.Bold = true;
            else if (fontWeight2 == "Italic") font.Italic = true;
            font.Underline = underline;
        }
        
        public void Write(string text)
        {
            builder.Writeln(text);
            builder.Writeln();
        }

        public void LoadImg(string filepath, RelativeHorizontalPosition horzPos, double left, RelativeVerticalPosition vertPos, double top, double width, double height, WrapType wrapType)
        {
            builder.InsertImage(filepath, horzPos, left, vertPos, top, width, height, wrapType);
        }

        Table table;
        public void createVerticaltalTable(List<string> columnnames, List<List<string>> rowstr)
        {
            if(columnnames.Count != 0)
            {
                Font font = builder.Font;
                font.Size = 12;
                font.Bold = true;
                font.Color = System.Drawing.Color.Black;
                font.Name = "Arial";

                table = builder.StartTable();
                builder.InsertCell();
                table.AutoFit(AutoFitBehavior.AutoFitToContents);
                builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                builder.Write(columnnames[0]);
                for(int i = 1; i < columnnames.Count; i++)
                {
                    builder.InsertCell();
                    builder.Write(columnnames[i]);
                }
                builder.EndRow();

                font.Bold = false;

                foreach (List<string> list in rowstr)
                {
                    foreach(string item in list)
                    {
                        builder.InsertCell();
                        builder.Write(item);
                    }
                    builder.EndRow();
                }
                builder.EndTable();
            }
        }

        public void createHorizontaltalTable(List<string> columnnames, List<string> rowstr)
        {
            if (columnnames.Count != 0 && rowstr.Count != 0)
            {
                Font font = builder.Font;
                font.Size = 12;
                font.Bold = true;
                font.Color = System.Drawing.Color.Black;
                font.Name = "Arial";
                table = builder.StartTable();
                builder.InsertCell();
                table.AutoFit(AutoFitBehavior.AutoFitToContents);
                builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                builder.Write(columnnames[0]);
                font.Bold = false;
                builder.InsertCell();
                builder.Write(rowstr[0]);
                builder.EndRow();
                for (int i = 1; i < columnnames.Count; i++)
                {
                    font.Bold = true;
                    builder.InsertCell();
                    builder.Write(columnnames[i]);
                    font.Bold = false;
                    builder.InsertCell();
                    builder.Write(rowstr[i]);
                    builder.EndRow();
                }
                builder.EndTable();
            }
        }

        public void Bosluk()
        {
            builder.Writeln();
        }

        public void Save(string filepath)
        {
            doc.Save(filepath);
        }

        public void UpdateText(string text, int section, int par, int runs)
        {
            var paragraph = doc.Sections[section].Body.Paragraphs[par].Runs[runs];
            paragraph.Text = text;
        }

    }
}
