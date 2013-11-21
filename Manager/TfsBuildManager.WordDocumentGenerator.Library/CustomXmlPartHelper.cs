//-----------------------------------------------------------------------
// <copyright file="CustomXmlPartHelper.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using DocumentFormat.OpenXml.Packaging;

    /// <summary>
    /// Helper class for Word CustomXml part operations
    /// </summary>
    public class CustomXmlPartHelper
    {        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomXmlPartHelper"/> class.
        /// </summary>
        /// <param name="namespaceUri">The namespace URI.</param>
        public CustomXmlPartHelper(Uri namespaceUri)
        {
            this.CustomXmlPartCore = new CustomXmlPartCore(namespaceUri);
        }

        #endregion

        #region Members

        public CustomXmlPartCore CustomXmlPartCore { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the type of the element from name to value collection for.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        /// <param name="rootElementName">Name of the root element.</param>
        /// <param name="childElementName">Name of the child element.</param>
        /// <param name="nameToValueCollection">The name to value collection.</param>
        /// <param name="forNodeType">Type of for node.</param>
        public void SetElementFromNameToValueCollectionForType(MainDocumentPart mainDocumentPart, string rootElementName, string childElementName, Dictionary<string, string> nameToValueCollection, NodeType forNodeType)
        {
            if (mainDocumentPart == null)
            {
                throw new ArgumentNullException("mainDocumentPart");
            }

            if (string.IsNullOrEmpty(rootElementName))
            {
                throw new ArgumentNullException("rootElementName");
            }

            if (string.IsNullOrEmpty(childElementName))
            {
                throw new ArgumentNullException("childElementName");
            }

            if (nameToValueCollection == null)
            {
                throw new ArgumentNullException("nameToValueCollection");
            }

            var rootElementXName = XName.Get(rootElementName, this.CustomXmlPartCore.NamespaceUri.ToString());
            var childElementXName = XName.Get(childElementName, this.CustomXmlPartCore.NamespaceUri.ToString());
            var rootElement = new XElement(rootElementXName);
            XElement childElement = null;
            var customXmlPart = this.CustomXmlPartCore.GetCustomXmlPart(mainDocumentPart);

            if (customXmlPart != null)
            {
                // Root element shall never be null if Custom Xml part is present
                rootElement = this.CustomXmlPartCore.GetFirstElementFromCustomXmlPart(customXmlPart, rootElementName);

                childElement = (from e in rootElement.Descendants(childElementXName)
                                select e).FirstOrDefault();

                if (childElement != null)
                {
                    foreach (var idToValue in nameToValueCollection)
                    {
                        switch (forNodeType)
                        {
                            case NodeType.Attribute:
                                AddOrUpdateAttribute(childElement, idToValue.Key, idToValue.Value);
                                break;
                            case NodeType.Element:
                                this.AddOrUpdateChildElement(childElement, idToValue.Key, idToValue.Value);
                                break;
                        }
                    }

                    CustomXmlPartCore.WriteElementToCustomXmlPart(customXmlPart, rootElement);
                }
                else
                {
                    childElement = this.GetElementFromNameToValueCollectionForType(nameToValueCollection, childElementXName, forNodeType);
                    rootElement.Add(childElement);
                }
            }
            else
            {
                customXmlPart = this.CustomXmlPartCore.AddCustomXmlPart(mainDocumentPart, rootElementName);
                childElement = this.GetElementFromNameToValueCollectionForType(nameToValueCollection, childElementXName, forNodeType);
                rootElement.Add(childElement);
            }

            CustomXmlPartCore.WriteElementToCustomXmlPart(customXmlPart, rootElement);
        }

        /// <summary>
        /// Gets the type of the name to value collection from element for.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="forNodeType">Type of for node.</param>
        /// <returns>Returns the type of the name to value collection from element for</returns>
        public Dictionary<string, string> GetNameToValueCollectionFromElementForType(MainDocumentPart mainDocumentPart, string elementName, NodeType forNodeType)
        {
            var nameToValueCollection = new Dictionary<string, string>();
            var customXmlPart = this.CustomXmlPartCore.GetCustomXmlPart(mainDocumentPart);

            if (customXmlPart != null)
            {
                var element = this.CustomXmlPartCore.GetFirstElementFromCustomXmlPart(customXmlPart, elementName);

                if (element != null)
                {
                    switch (forNodeType)
                    {
                        case NodeType.Element:
                            foreach (var elem in element.Elements())
                            {
                                var firstOrDefault = elem.Nodes().FirstOrDefault(node => node.NodeType == XmlNodeType.Element);
                                if (firstOrDefault != null)
                                {
                                    nameToValueCollection.Add(elem.Name.LocalName, firstOrDefault.ToString());
                                }
                            }

                            break;
                        case NodeType.Attribute:
                            foreach (var attr in element.Attributes())
                            {
                                nameToValueCollection.Add(attr.Name.LocalName, attr.Value);
                            }

                            break;
                    }
                }
            }

            return nameToValueCollection;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the or update attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="attributeValue">The attribute value.</param>
        private static void AddOrUpdateAttribute(XElement element, string attributeName, string attributeValue)
        {
            var attrToUpdate =
                element.Attributes().FirstOrDefault(attr => attr.Name.LocalName.Equals(attributeName));

            if (attrToUpdate != null)
            {
                attrToUpdate.Value = attributeValue;
            }
            else
            {
                var attr = new XAttribute(attributeName, attributeValue);
                element.Add(attr);
            }
        }

        /// <summary>
        /// Gets the type of the element from name to value collection for.
        /// </summary>
        /// <param name="nameToValueCollection">The name to value collection.</param>
        /// <param name="elementXName">Name of the element X.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <returns>Returns the XElement of the element </returns>
        private XElement GetElementFromNameToValueCollectionForType(Dictionary<string, string> nameToValueCollection, XName elementXName, NodeType nodeType)
        {
            var element = new XElement(elementXName);

            foreach (var idToValue in nameToValueCollection)
            {
                switch (nodeType)
                {
                    case NodeType.Element:
                        this.AddOrUpdateChildElement(element, idToValue.Key, idToValue.Value);
                        break;
                    case NodeType.Attribute:
                        AddOrUpdateAttribute(element, idToValue.Key, idToValue.Value);
                        break;
                }
            }

            return element;
        }

        /// <summary>
        /// Adds the or update child element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="childElementName">Name of the child element.</param>
        /// <param name="childElementValue">The child element value.</param>
        private void AddOrUpdateChildElement(XElement element, string childElementName, string childElementValue)
        {
            var childElement =
                element.Elements().FirstOrDefault(elem => elem.Name.LocalName.Equals(childElementName));
            var newChildElement = new XElement(XName.Get(childElementName, this.CustomXmlPartCore.NamespaceUri.ToString()));
            newChildElement.Add(XElement.Parse(childElementValue));

            if (childElement != null)
            {
                childElement.ReplaceWith(newChildElement);
            }
            else
            {
                element.Add(newChildElement);
            }
        }

        #endregion
    }
}
