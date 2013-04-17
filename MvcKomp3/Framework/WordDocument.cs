using System;
using Microsoft.Office.Interop.Word;

namespace MvcKompApp.Framework
{
    public static class WordDocument
    {
        public const String CurrentDateBookmark = "CurrentDate";
        public const String SignatureBookmark = "Signature";

        public static void CreateSampleWordDocument(String fileName, String templateName, DateTime current, String author)
        {
            // Run Word and make it visible for demo purposes
            var wordApp = new Application { Visible = false };

            // Create a new document
            var templatedDocument = wordApp.Documents.Add(templateName);
            templatedDocument.Activate();

            // Fill the bokmarks in the document
            templatedDocument.Bookmarks[CurrentDateBookmark].Range.Select();
            wordApp.Selection.TypeText(current.ToString("dddd, dd MMMM yyyy @ hh:mm"));
            templatedDocument.Bookmarks[SignatureBookmark].Range.Select();
            wordApp.Selection.TypeText(author);

            // Save the document 
            templatedDocument.SaveAs(fileName, WdSaveFormat.wdFormatPDF);

            // Clean up
            templatedDocument.Close(WdSaveOptions.wdDoNotSaveChanges);
            wordApp.Quit();

        }
    }
}