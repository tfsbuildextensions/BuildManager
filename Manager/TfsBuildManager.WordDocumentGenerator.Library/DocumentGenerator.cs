//-----------------------------------------------------------------------
// <copyright file="DocumentGenerator.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;

    /// <summary>
    /// Base class for document generation
    /// </summary>
    public abstract class DocumentGenerator
    {
        #region Constants

        /// <summary>
        /// Root Node of CustomXML Part
        /// </summary>
        protected const string DocumentRootNode = "DocumentRootNode";

        /// <summary>
        /// Document Node
        /// </summary>
        protected const string DocumentNode = "Document";

        /// <summary>
        /// Document Container PlaceHolders Node
        /// </summary>
        protected const string DocumentContainerPlaceholdersNode = "DocumentContainerPlaceHolders";

        /// <summary>
        /// Data bound controls data store Node
        /// </summary>
        protected const string DataBoundControlsDataStoreNode = "DataBoundControlsDataStore";

        /// <summary>
        /// Data node in Data bound controls data store
        /// </summary>
        protected const string DataNode = "Data";

        /// <summary>
        /// Document Type Attribute
        /// </summary>
        protected const string DocumentTypeNodeName = "DocumentType";

        /// <summary>
        /// Document Version Attribute
        /// </summary>
        protected const string DocumentVersionNodeName = "Version";

        #endregion

        #region Members

        /// <summary>
        /// Instance of Document generation info
        /// </summary>
        private readonly DocumentGenerationInfo generationInfo;
        private readonly List<string> exclusions;

        /// <summary>
        /// Instance of CustomXml Part Helper
        /// </summary>
        private readonly CustomXmlPartHelper customXmlPartHelper = new CustomXmlPartHelper(DocumentGenerationInfo.NamespaceUri);

        #endregion

        #region Constructor

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Don't have an alternate approach just yet.")]
        protected DocumentGenerator(DocumentGenerationInfo generationInfo, List<string> exclusions)
        {
            this.generationInfo = generationInfo;
            this.exclusions = exclusions;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates the document.
        /// </summary>
        /// <returns>Returns the generated document </returns>
        public byte[] GenerateDocument()
        {
            this.generationInfo.PlaceholderTagToTypeCollection = this.GetPlaceholderTagToTypeCollection();
            return this.SetContentInPlaceholders();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the full tag value.
        /// </summary>
        /// <param name="templateTagPart">The template tag part.</param>
        /// <param name="tagGuidPart">The tag GUID part.</param>
        /// <returns>Gets the full tag value</returns>
        protected static string GetFullTagValue(string templateTagPart, string tagGuidPart)
        {
            return templateTagPart + ":" + tagGuidPart;
        }

        /// <summary>
        /// Sets the tag value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="fullTagValue">The full tag value.</param>
        protected static void SetTagValue(SdtElement element, string fullTagValue)
        {
            // Set the tag for the content control
            if (!string.IsNullOrEmpty(fullTagValue))
            {
                OpenXmlHelper.SetTagValue(element, fullTagValue);
            }
        }

        /// <summary>
        /// Sets the content of content control.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="content">The content.</param>
        protected static void SetContentOfContentControl(SdtElement element, string content)
        {
            // Set text without data binding
            OpenXmlHelper.SetContentOfContentControl(element, content);
        }

        /// <summary>
        /// Determines whether [is template tag equal] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="placeholderName">Name of the placeholder.</param>
        /// <returns>
        ///   <c>true</c> if [is template tag equal] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        protected static bool IsTemplateTagEqual(SdtElement element, string placeholderName)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (placeholderName == null)
            {
                throw new ArgumentNullException("placeholderName");
            }

            string templateTagPart;
            string tagGuidPart;
            GetTagValue(element, out templateTagPart, out tagGuidPart);
            return placeholderName.Equals(templateTagPart);
        }

        /// <summary>
        /// Gets the tag value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="templateTagPart">The template tag part.</param>
        /// <param name="tagGuidPart">The tag GUID part.</param>
        /// <returns>Get the tag value of the tag</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Output parameter handled safely"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "The outparameter is used safely"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "The outparameter is used safely"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "The out parameter is handled safely")]
        protected static string GetTagValue(SdtElement element, out string templateTagPart, out string tagGuidPart)
        {
            templateTagPart = string.Empty;
            tagGuidPart = string.Empty;
            Tag tag = OpenXmlHelper.GetTag(element);

            string fullTag = (tag == null || (tag.Val.HasValue == false)) ? string.Empty : tag.Val.Value;

            if (!string.IsNullOrEmpty(fullTag))
            {
                string[] tagParts = fullTag.Split(':');

                if (tagParts.Length == 2)
                {
                    templateTagPart = tagParts[0];
                    tagGuidPart = tagParts[1];
                }
                else if (tagParts.Length == 1)
                {
                    templateTagPart = tagParts[0];
                }
            }

            return fullTag;
        }

        /// <summary>
        /// Gets the place holder tag to type collection.
        /// </summary>
        /// <returns>Returns the place holder tag to type collection dictionary</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Don't have a fix for this just now")]
        protected abstract Dictionary<string, PlaceholderType> GetPlaceholderTagToTypeCollection();

        /// <summary>
        /// Ignore placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        protected abstract void IgnorePlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext);

        /// <summary>
        /// Non recursive placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        protected abstract void NonRecursivePlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext);

        /// <summary>
        /// Recursive placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        protected abstract void RecursivePlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext);

        /// <summary>
        /// Container placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        protected abstract void ContainerPlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext);

        /// <summary>
        /// Image placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The image placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        protected abstract void PictureContainerPlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext);

        /// <summary>
        /// Refreshes the charts.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        protected virtual void RefreshCharts(MainDocumentPart mainDocumentPart)
        {
        }

        /// <summary>
        /// Gets the serialized data context.
        /// </summary>
        /// <returns>Returns the serialized data context to xml string</returns>
        protected virtual string SerializeDataContextToXml()
        {
            StringBuilder sb = new StringBuilder();

            if (this.generationInfo != null && this.generationInfo.DataContext != null)
            {
                XmlSerializer serializer = new XmlSerializer(this.generationInfo.DataContext.GetType());
                XmlWriterSettings writerSettings = new XmlWriterSettings { OmitXmlDeclaration = true };
                using (XmlWriter writer = XmlWriter.Create(sb, writerSettings))
                {
                    serializer.Serialize(writer, this.generationInfo.DataContext);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the parent container.
        /// </summary>
        /// <param name="parentContainer">The parent container.</param>
        /// <param name="placeholder">The place holder.</param>
        /// <returns>Returns whether a parent container is present </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Don't have a fix for this just now")]
        protected bool GetParentContainer(ref SdtElement parentContainer, string placeholder)
        {
            MainDocumentPart mainDocumentPart = parentContainer.Ancestors<Document>().First().MainDocumentPart;
            KeyValuePair<string, string> nameToValue = this.customXmlPartHelper.GetNameToValueCollectionFromElementForType(mainDocumentPart, DocumentContainerPlaceholdersNode, NodeType.Element).FirstOrDefault(f => f.Key.Equals(placeholder));
            bool isRefresh = !string.IsNullOrEmpty(nameToValue.Value);

            if (isRefresh)
            {
                SdtElement parentElementFromCustomXmlPart = new SdtBlock(nameToValue.Value);
                parentContainer.Parent.ReplaceChild(parentElementFromCustomXmlPart, parentContainer);
                parentContainer = parentElementFromCustomXmlPart;
            }
            else
            {
                Dictionary<string, string> nameToValueCollection = new Dictionary<string, string> { { placeholder, parentContainer.OuterXml } };
                this.customXmlPartHelper.SetElementFromNameToValueCollectionForType(mainDocumentPart, DocumentRootNode, DocumentContainerPlaceholdersNode, nameToValueCollection, NodeType.Element);
            }

            return isRefresh;
        }
        
        /// <summary>
        /// Saves the data content to data bound controls data store.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        protected void SaveDataToDataBoundControlsDataStore(MainDocumentPart mainDocumentPart)
        {
            string dataContextAsXml = this.SerializeDataContextToXml();
            Dictionary<string, string> nameToValueCollection = new Dictionary<string, string> { { DataNode, dataContextAsXml } };
            this.customXmlPartHelper.SetElementFromNameToValueCollectionForType(mainDocumentPart, DocumentRootNode, DataBoundControlsDataStoreNode, nameToValueCollection, NodeType.Element);
        }

        /// <summary>
        /// Sets the data binding.
        /// </summary>
        /// <param name="xpath">The x path.</param>
        /// <param name="element">The element.</param>
        protected void SetDataBinding(string xpath, SdtElement element)
        {
            element.SdtProperties.RemoveAllChildren<DataBinding>();
            DataBinding dataBinding = new DataBinding { XPath = xpath, StoreItemId = new StringValue(this.customXmlPartHelper.CustomXmlPartCore.GetStoreItemId(element.Ancestors<Document>().First().MainDocumentPart)) };
            element.SdtProperties.Append(dataBinding);
        }

        /// <summary>
        /// Sets the content in placeholders.
        /// </summary>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        protected void SetContentInPlaceholders(OpenXmlElementDataContext openXmlElementDataContext)
        {
            if (IsContentControl(openXmlElementDataContext))
            {
                string templateTagPart;
                string tagGuidPart;
                SdtElement element = openXmlElementDataContext.Element as SdtElement;
                GetTagValue(element, out templateTagPart, out tagGuidPart);

                if (this.generationInfo.PlaceholderTagToTypeCollection.ContainsKey(templateTagPart))
                {
                    this.OnPlaceHolderFound(openXmlElementDataContext);
                }
                else
                {
                    this.PopulateOtherOpenXmlElements(openXmlElementDataContext);
                }
            }
            else
            {
                this.PopulateOtherOpenXmlElements(openXmlElementDataContext);
            }
        }

        /// <summary>
        /// Clones the element and set content in placeholders.
        /// </summary>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        /// <returns>Clones the element and returns the cloned element</returns>
        protected SdtElement CloneElementAndSetContentInPlaceholders(OpenXmlElementDataContext openXmlElementDataContext)
        {
            if (openXmlElementDataContext == null)
            {
                throw new ArgumentNullException("openXmlElementDataContext");
            }

            if (openXmlElementDataContext.Element == null)
            {
                throw new ArgumentNullException("openXmlElementDataContext");
            }

            SdtElement clonedSdtElement;
            if (openXmlElementDataContext.Element.Parent is Paragraph)
            {
                Paragraph clonedPara = openXmlElementDataContext.Element.Parent.InsertBeforeSelf(openXmlElementDataContext.Element.Parent.CloneNode(true) as Paragraph);
                clonedSdtElement = clonedPara.Descendants<SdtElement>().First();
            }
            else
            {
                clonedSdtElement = openXmlElementDataContext.Element.InsertBeforeSelf(openXmlElementDataContext.Element.CloneNode(true) as SdtElement);
            }

            foreach (var v in clonedSdtElement.Elements())
            {
                this.SetContentInPlaceholders(new OpenXmlElementDataContext { Element = v, DataContext = openXmlElementDataContext.DataContext });
            }

            return clonedSdtElement;
        }

        /// <summary>
        /// Sets the document properties.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        /// <param name="docProperties">The doc properties.</param>
        protected void SetDocumentProperties(MainDocumentPart mainDocumentPart, DocumentMetadata docProperties)
        {
            if (mainDocumentPart == null)
            {
                throw new ArgumentNullException("mainDocumentPart");
            }

            if (docProperties == null)
            {
                throw new ArgumentNullException("docProperties");
            }

            Dictionary<string, string> idtoValues = new Dictionary<string, string> { { DocumentTypeNodeName, string.IsNullOrEmpty(docProperties.DocumentType) ? string.Empty : docProperties.DocumentType }, { DocumentVersionNodeName, string.IsNullOrEmpty(docProperties.DocumentVersion) ? string.Empty : docProperties.DocumentVersion } };
            this.customXmlPartHelper.SetElementFromNameToValueCollectionForType(mainDocumentPart, DocumentRootNode, DocumentNode, idtoValues, NodeType.Attribute);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether [is content control] [the specified open XML element data context].
        /// </summary>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        /// <returns>
        ///   <c>true</c> if [is content control] [the specified open XML element data context]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsContentControl(OpenXmlElementDataContext openXmlElementDataContext)
        {
            if (openXmlElementDataContext == null || openXmlElementDataContext.Element == null)
            {
                return false;
            }

            return openXmlElementDataContext.Element is SdtBlock || openXmlElementDataContext.Element is SdtRun || openXmlElementDataContext.Element is SdtRow || openXmlElementDataContext.Element is SdtCell;
        }

        /// <summary>
        /// Sets the content in placeholders.
        /// </summary>
        /// <returns>This method returns the stream of bytes after setting the contents in the placeholder</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Don't have a fix for this just now")]
        private byte[] SetContentInPlaceholders()
        {
            byte[] output;

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(this.generationInfo.TemplateData, 0, this.generationInfo.TemplateData.Length);

                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(ms, true))
                {
                    wordDocument.ChangeDocumentType(WordprocessingDocumentType.Document);
                    MainDocumentPart mainDocumentPart = wordDocument.MainDocumentPart;
                    Document document = mainDocumentPart.Document;

                    // The sections that the user decided to remove from the build notes
                    foreach (var exclusion in this.exclusions)
                    {
                        var section =
                            document.Body.Descendants<SdtElement>().FirstOrDefault(
                                r => r.SdtProperties.GetFirstChild<Tag>().Val.HasValue &&
                                     r.SdtProperties.GetFirstChild<Tag>().Val.Value ==
                                     string.Format("{0}ContentControlRow", exclusion));
                        mainDocumentPart.Document.Body.RemoveChild(section);
                    }

                    if (this.generationInfo.Metadata != null)
                    {
                        this.SetDocumentProperties(mainDocumentPart, this.generationInfo.Metadata);
                    }

                    if (this.generationInfo.IsDataBoundControls)
                    {
                        this.SaveDataToDataBoundControlsDataStore(mainDocumentPart);
                    }

                    foreach (HeaderPart part in mainDocumentPart.HeaderParts)
                    {
                        this.SetContentInPlaceholders(new OpenXmlElementDataContext { Element = part.Header, DataContext = this.generationInfo.DataContext });
                        part.Header.Save();
                    }

                    foreach (FooterPart part in mainDocumentPart.FooterParts)
                    {
                        this.SetContentInPlaceholders(new OpenXmlElementDataContext { Element = part.Footer, DataContext = this.generationInfo.DataContext });
                        part.Footer.Save();
                    }

                    this.SetContentInPlaceholders(new OpenXmlElementDataContext { Element = document, DataContext = this.generationInfo.DataContext });

                    OpenXmlHelper.EnsureUniqueContentControlIdsForMainDocumentPart(mainDocumentPart);

                    document.Save();
                }

                ms.Position = 0;
                output = new byte[ms.Length];
                ms.Read(output, 0, output.Length);
            }

            return output;
        }

        /// <summary>
        /// Populates the other open XML elements.
        /// </summary>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        private void PopulateOtherOpenXmlElements(OpenXmlElementDataContext openXmlElementDataContext)
        {
            if (openXmlElementDataContext.Element is OpenXmlCompositeElement && openXmlElementDataContext.Element.HasChildren)
            {
                List<OpenXmlElement> elements = openXmlElementDataContext.Element.Elements().ToList();

                foreach (var element in elements)
                {
                    if (element is OpenXmlCompositeElement)
                    {
                        this.SetContentInPlaceholders(new OpenXmlElementDataContext { Element = element, DataContext = openXmlElementDataContext.DataContext });
                    }
                }
            }
        }

        /// <summary>
        /// Called when [place holder found].
        /// </summary>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        private void OnPlaceHolderFound(OpenXmlElementDataContext openXmlElementDataContext)
        {
            string templateTagPart;
            string tagGuidPart;
            SdtElement element = openXmlElementDataContext.Element as SdtElement;
            GetTagValue(element, out templateTagPart, out tagGuidPart);

            if (this.generationInfo.PlaceholderTagToTypeCollection.ContainsKey(templateTagPart))
            {
                switch (this.generationInfo.PlaceholderTagToTypeCollection[templateTagPart])
                {
                    case PlaceholderType.None:
                        break;
                    case PlaceholderType.NonRecursive:
                        this.NonRecursivePlaceholderFound(templateTagPart, openXmlElementDataContext);
                        break;
                    case PlaceholderType.Recursive:
                        this.RecursivePlaceholderFound(templateTagPart, openXmlElementDataContext);
                        break;
                    case PlaceholderType.Ignore:
                        this.IgnorePlaceholderFound(templateTagPart, openXmlElementDataContext);
                        break;
                    case PlaceholderType.Container:
                        this.ContainerPlaceholderFound(templateTagPart, openXmlElementDataContext);
                        break;
                    case PlaceholderType.PictureContainer:
                        this.PictureContainerPlaceholderFound(templateTagPart, openXmlElementDataContext);
                        break;
                }
            }
        }

        #endregion
    }
}
