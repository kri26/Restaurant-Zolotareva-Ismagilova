﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using RestaurantBusinessLogic.HelperModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantBusinessLogic.BusinessLogics
{
    static class SupplierSaveToWord
    {
        public static void CreateDoc(WordInfo info)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(info.FileName, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body docBody = mainPart.Document.AppendChild(new Body());
                docBody.AppendChild(CreateParagraph(new WordParagraph
                {
                    Texts = new List<string> { info.Title },
                    TextProperties = new WordParagraphProperties
                    {
                        Bold = true,
                        Size = "24",
                        JustificationValues = JustificationValues.Center
                    }
                }));
                docBody.AppendChild(CreateParagraph(new WordParagraph
                {
                    Texts = new List<string> { "Поставщик:" + " " + info.SupplierFIO },
                    TextProperties = new WordParagraphProperties
                    {
                        Bold = false,
                        Size = "20",
                        JustificationValues = JustificationValues.Left
                    }
                }));
                docBody.AppendChild(CreateParagraph(new WordParagraph
                {
                    Texts = new List<string> { "Дата выполнения:" + " " + info.DateComplete },
                    TextProperties = new WordParagraphProperties
                    {
                        Bold = false,
                        Size = "18",
                        JustificationValues = JustificationValues.Left
                    }
                }));
                Table table = new Table();
                TableProperties tblProp = new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 }
                    )
                );
                table.AppendChild<TableProperties>(tblProp);
                TableRow headerRow = new TableRow();
                TableCell headerNumberCell = new TableCell(new Paragraph(new Run(new Text("№ продукта"))));
                TableCell headerNameCell = new TableCell(new Paragraph(new Run(new Text("Продукт"))));
                TableCell headerCountryCell = new TableCell(new Paragraph(new Run(new Text("Количество"))));
                headerRow.Append(headerNumberCell);
                headerRow.Append(headerNameCell);
                headerRow.Append(headerCountryCell);
                table.Append(headerRow);
                int i = 1;
                foreach (var food in info.RequestFoods)
                {
                    TableRow foodRow = new TableRow();
                    TableCell numberCell = new TableCell(new Paragraph(new Run(new Text(food.Key.ToString()))));
                    TableCell foodnameCell = new TableCell(new Paragraph(new Run(new Text(food.Value.Item1))));
                    TableCell countCell = new TableCell(new Paragraph(new Run(new Text(food.Value.Item2.ToString()))));
                    foodRow.Append(numberCell);
                    foodRow.Append(foodnameCell);
                    foodRow.Append(countCell);
                    table.Append(foodRow);
                    i++;
                }
                docBody.Append(table);
                docBody.AppendChild(CreateSectionProperties());
                wordDocument.MainDocumentPart.Document.Save();
            }
        }
        private static SectionProperties CreateSectionProperties()
        {
            SectionProperties properties = new SectionProperties();
            PageSize pageSize = new PageSize
            {
                Orient = PageOrientationValues.Portrait
            };
            properties.AppendChild(pageSize);
            return properties;
        }
        private static Paragraph CreateParagraph(WordParagraph paragraph)
        {
            if (paragraph != null)
            {
                Paragraph docParagraph = new Paragraph();
                docParagraph.AppendChild(CreateParagraphProperties(paragraph.TextProperties));
                foreach (var run in paragraph.Texts)
                {
                    Run docRun = new Run();
                    RunProperties properties = new RunProperties();
                    properties.AppendChild(new FontSize
                    {
                        Val = paragraph.TextProperties.Size
                    });
                    if (paragraph.TextProperties.Bold)
                    {
                        properties.AppendChild(new Bold());
                    }
                    docRun.AppendChild(properties);
                    docRun.AppendChild(new Text
                    {
                        Text = run,
                        Space = SpaceProcessingModeValues.Preserve
                    });
                    docParagraph.AppendChild(docRun);
                }
                return docParagraph;
            }
            return null;
        }
        private static ParagraphProperties
        CreateParagraphProperties(WordParagraphProperties paragraphProperties)
        {
            if (paragraphProperties != null)
            {
                ParagraphProperties properties = new ParagraphProperties();
                properties.AppendChild(new Justification()
                {
                    Val = paragraphProperties.JustificationValues
                });
                properties.AppendChild(new SpacingBetweenLines
                {
                    LineRule = LineSpacingRuleValues.Auto
                });
                properties.AppendChild(new Indentation());
                ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
                if (!string.IsNullOrEmpty(paragraphProperties.Size))
                {
                    paragraphMarkRunProperties.AppendChild(new FontSize
                    {
                        Val = paragraphProperties.Size
                    });
                }
                if (paragraphProperties.Bold)
                {
                    paragraphMarkRunProperties.AppendChild(new Bold());
                }
                properties.AppendChild(paragraphMarkRunProperties);
                return properties;
            }
            return null;
        }
    }
}