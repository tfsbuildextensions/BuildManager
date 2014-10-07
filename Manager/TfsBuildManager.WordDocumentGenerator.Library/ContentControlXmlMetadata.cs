//-----------------------------------------------------------------------
// <copyright file="ContentControlXmlMetadata.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    /// <summary>
    /// This class is used only for generic document generators that generate based on Xml, XPath and data bound content controls(optional)
    /// </summary>
    public class ContentControlXmlMetadata
    {
        public string PlaceholderName { get; set; }

        public PlaceholderType PlaceholderType { get; set; }

        public string ControlTagXPath { get; set; }

        public string ControlValueXPath { get; set; }
    }
}
