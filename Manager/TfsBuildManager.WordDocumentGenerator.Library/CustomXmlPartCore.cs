//-----------------------------------------------------------------------
// <copyright file="CustomXmlPartCore.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using DocumentFormat.OpenXml.CustomXmlDataProperties;
    using DocumentFormat.OpenXml.Packaging;

    /// <summary>
    /// Helper class for Word CustomXml part operations
    /// </summary>
    public class CustomXmlPartCore
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomXmlPartCore"/> class.
        /// </summary>
        /// <param name="namespaceUri">The namespace URI.</param>
        public CustomXmlPartCore(Uri namespaceUri)
        {
            this.NamespaceUri = namespaceUri;
        }        

        #endregion

        #region Members

        public Uri NamespaceUri { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Removes the custom XML part.
        /// </summary>
        /// <param name="mainDocumentPart">The main part.</param>
        /// <param name="customXmlPart">The custom XML part.</param>
        public static void RemoveCustomXmlPart(MainDocumentPart mainDocumentPart, CustomXmlPart customXmlPart)
        {
            if (mainDocumentPart == null)
            {
                throw new ArgumentNullException("mainDocumentPart");
            }

            if (customXmlPart != null)
            {
                RemoveCustomXmlParts(mainDocumentPart, new List<CustomXmlPart>(new[] { customXmlPart }));
            }
        }

        /// <summary>
        /// Removes the custom XML parts.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        /// <param name="customXmlParts">The custom XML parts.</param>
        public static void RemoveCustomXmlParts(OpenXmlPartContainer mainDocumentPart, IList<CustomXmlPart> customXmlParts)
        {
            if (mainDocumentPart == null)
            {
                throw new ArgumentNullException("mainDocumentPart");
            }

            if (customXmlParts != null)
            {
                mainDocumentPart.DeleteParts(customXmlParts);
            }
        }

        /// <summary>
        /// Writes the element to custom XML part.
        /// </summary>
        /// <param name="customXmlPart">The custom XML part.</param>
        /// <param name="rootElement">The root element.</param>
        public static void WriteElementToCustomXmlPart(OpenXmlPart customXmlPart, XNode rootElement)
        {
            if (customXmlPart == null)
            {
                throw new ArgumentNullException("customXmlPart");
            }

            if (rootElement == null)
            {
                throw new ArgumentNullException("rootElement");
            }

            using (var writer = XmlWriter.Create(customXmlPart.GetStream(FileMode.Create, FileAccess.Write)))
            {
                rootElement.WriteTo(writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Adds the custom XML part.
        /// </summary>
        /// <param name="mainDocumentPart">The main part.</param>
        /// <param name="rootElementName">Name of the root element.</param>
        /// <returns>
        /// Returns CustomXmlPart
        /// </returns>
        public CustomXmlPart AddCustomXmlPart(MainDocumentPart mainDocumentPart, string rootElementName)
        {
            if (mainDocumentPart == null)
            {
                throw new ArgumentNullException("mainDocumentPart");
            }

            if (string.IsNullOrEmpty(rootElementName))
            {
                throw new ArgumentNullException("rootElementName");
            }

            var rootElementXName = XName.Get(rootElementName, this.NamespaceUri.ToString());
            var rootElement = new XElement(rootElementXName);
            var customXmlPart = mainDocumentPart.AddCustomXmlPart(CustomXmlPartType.CustomXml);
            var customXmlPropertiesPart = customXmlPart.AddNewPart<CustomXmlPropertiesPart>();
            this.GenerateCustomXmlPropertiesPartContent(customXmlPropertiesPart);
            WriteElementToCustomXmlPart(customXmlPart, rootElement);

            return customXmlPart;
        }

        /// <summary>
        /// Gets the custom XML part.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        /// <returns>Returns the custom xml part</returns>
        public CustomXmlPart GetCustomXmlPart(MainDocumentPart mainDocumentPart)
        {
            if (mainDocumentPart == null)
            {
                throw new ArgumentNullException("mainDocumentPart");
            }

            CustomXmlPart result = null;

            foreach (var part in mainDocumentPart.CustomXmlParts)
            {
                using (var reader = new XmlTextReader(part.GetStream(FileMode.Open, FileAccess.Read)))
                {
                    reader.MoveToContent();
                    var exists = reader.NamespaceURI.Equals(this.NamespaceUri);

                    if (!exists)
                    {
                        continue;
                    }

                    result = part;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the store item id.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        /// <returns>Returns the store item id</returns>
        public string GetStoreItemId(MainDocumentPart mainDocumentPart)
        {
            if (mainDocumentPart == null)
            {
                throw new ArgumentNullException("mainDocumentPart");
            }

            var customXmlPart = this.GetCustomXmlPart(mainDocumentPart);
            var customXmlPropertiesPart = customXmlPart.CustomXmlPropertiesPart;
            return customXmlPropertiesPart.DataStoreItem.ItemId.ToString();
        }

        /// <summary>
        /// Gets the first element from custom XML part.
        /// </summary>
        /// <param name="customXmlPart">The custom XML part.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>Returns the first element that matches the element name and the custom xml part</returns>
        public XElement GetFirstElementFromCustomXmlPart(OpenXmlPart customXmlPart, string elementName)
        {
            if (customXmlPart == null)
            {
                throw new ArgumentNullException("customXmlPart");
            }

            if (string.IsNullOrEmpty(elementName))
            {
                throw new ArgumentNullException("elementName");
            }

            XDocument customPartDoc;

            using (var reader = XmlReader.Create(customXmlPart.GetStream(FileMode.Open, FileAccess.Read)))
            {
                customPartDoc = XDocument.Load(reader);
            }

            var elementXName = XName.Get(elementName, this.NamespaceUri.ToString());
            var element = (from e in customPartDoc.Descendants(elementXName)
                                select e).FirstOrDefault();

            return element;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates the content of the custom XML properties part.
        /// </summary>
        /// <param name="customXmlPropertiesPart">The custom XML properties part1.</param>
        private void GenerateCustomXmlPropertiesPartContent(CustomXmlPropertiesPart customXmlPropertiesPart)
        {
            var dataStoreItem = new DataStoreItem() { ItemId = Guid.NewGuid().ToString() };
            dataStoreItem.AddNamespaceDeclaration("ds", "http://schemas.openxmlformats.org/officeDocument/2006/customXml");
            var schemaReferences = new SchemaReferences();
            var schemaReference = new SchemaReference() { Uri = this.NamespaceUri.ToString() };
            schemaReferences.Append(schemaReference);
            dataStoreItem.Append(schemaReferences);
            customXmlPropertiesPart.DataStoreItem = dataStoreItem;
        }

        #endregion
    }
}