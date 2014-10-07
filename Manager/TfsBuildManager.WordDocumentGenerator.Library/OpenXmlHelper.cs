//-----------------------------------------------------------------------
// <copyright file="OpenXmlHelper.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;

    /// <summary>
    /// Helper class for OpenXml operations for document generation
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Don't have a fix for this just now")]
    public class OpenXmlHelper
    {
        #region Memebers

        private readonly Uri namespaceUri;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenXmlHelper"/> class.
        /// </summary>
        /// <param name="namespaceUri">The namespace URI.</param>
        public OpenXmlHelper(Uri namespaceUri)
        {
            this.namespaceUri = namespaceUri;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the text from content control.
        /// </summary>
        /// <param name="contentControl">The content control.</param>
        /// <returns>Returns the text from the content controls</returns>
        public static string GetTextFromContentControl(SdtElement contentControl)
        {
            string result = null;

            if (contentControl != null)
            {
                if (contentControl is SdtRun)
                {
                    if (IsContentControlMultiline(contentControl))
                    {
                        var runs = contentControl.Descendants<SdtContentRun>().First().Elements()
                           .Where(elem => elem is Run || elem is InsertedRun);

                        var runTexts = new List<string>();

                        foreach (var run in runs)
                        {
                            foreach (var runChild in run.Elements())
                            {
                                var runText = runChild as Text;
                                var runBreak = runChild as Break;

                                if (runText != null)
                                {
                                    runTexts.Add(runText.Text);
                                }
                                else if (runBreak != null)
                                {
                                    runTexts.Add(Environment.NewLine);
                                }
                            }
                        }

                        var stringBuilder = new StringBuilder();

                        foreach (var item in runTexts)
                        {
                            stringBuilder.Append(item);
                        }

                        result = stringBuilder.ToString();
                    }
                    else
                    {
                        result = contentControl.InnerText;
                    }
                }
                else
                {
                    result = contentControl.InnerText;
                }
            }

            return result;
        }

        /// <summary>
        /// Generates the paragraph.
        /// </summary>
        /// <returns>
        /// Returns new Paragraph with empty run
        /// </returns>
        public static Paragraph GenerateParagraph()
        {
            var paragraph = new Paragraph();
            var run = new Run();
            paragraph.Append(run);
            return paragraph;
        }

        /// <summary>
        /// Gets the SDT content of content control.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Returns the sdt content of content control</returns>
        public static OpenXmlCompositeElement GetSdtContentOfContentControl(SdtElement element)
        {
            var sdtRunELement = element as SdtRun;
            var sdtBlockElement = element as SdtBlock;
            var sdtCellElement = element as SdtCell;
            var sdtRowElement = element as SdtRow;

            if (sdtRunELement != null)
            {
                return sdtRunELement.SdtContentRun;
            }

            if (sdtBlockElement != null)
            {
                return sdtBlockElement.SdtContentBlock;
            }

            if (sdtCellElement != null)
            {
                return sdtCellElement.SdtContentCell;
            }

            if (sdtRowElement != null)
            {
                return sdtRowElement.SdtContentRow;
            }

            return null;
        }

        /// <summary>
        /// Unprotects the document.
        /// </summary>
        /// <param name="wordprocessingDocument">The word processing document.</param>
        public static void UnprotectDocument(WordprocessingDocument wordprocessingDocument)
        {
            if (wordprocessingDocument == null)
            {
                throw new ArgumentNullException("wordprocessingDocument");
            }

            var documentSettingsPart = wordprocessingDocument.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>().FirstOrDefault();

            if (documentSettingsPart != null)
            {
                var documentProtection = documentSettingsPart.Settings.Elements<DocumentProtection>().FirstOrDefault();

                if (documentProtection != null)
                {
                    documentProtection.Remove();
                }
            }

            var permElements = new List<OpenXmlLeafElement>();

            foreach (var permStart in wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<PermStart>())
            {
                if (!permElements.Contains(permStart))
                {
                    permElements.Add(permStart);
                }
            }

            foreach (var permEnd in wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<PermEnd>())
            {
                if (!permElements.Contains(permEnd))
                {
                    permElements.Add(permEnd);
                }
            }

            foreach (var permElem in permElements)
            {
                if (permElem.Parent != null)
                {
                    permElem.Remove();
                }
            }

            wordprocessingDocument.MainDocumentPart.Document.Save();
        }

        /// <summary>
        /// Protects the document.
        /// </summary>
        /// <param name="wordprocessingDocument">The word processing document.</param>
        public static void ProtectDocument(WordprocessingDocument wordprocessingDocument)
        {
            if (wordprocessingDocument == null)
            {
                throw new ArgumentNullException("wordprocessingDocument");
            }

            var documentSettingsPart = wordprocessingDocument.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>().FirstOrDefault();

            if (documentSettingsPart != null)
            {
                var documentProtection = documentSettingsPart.Settings.Elements<DocumentProtection>().FirstOrDefault();

                if (documentProtection != null)
                {
                    documentProtection.Enforcement = true;
                }
                else
                {
                    documentProtection = new DocumentProtection()
                    {
                        Edit = DocumentProtectionValues.Comments,
                        Enforcement = false, 
                        CryptographicProviderType = CryptProviderValues.RsaFull,
                        CryptographicAlgorithmClass = CryptAlgorithmClassValues.Hash,
                        CryptographicAlgorithmType = CryptAlgorithmValues.TypeAny,
                        CryptographicAlgorithmSid = 4,
                        CryptographicSpinCount = 100000U,
                        Hash = "2krUoz1qWd0WBeXqVrOq81l8xpk=",
                        Salt = "9kIgmDDYtt2r5U2idCOwMA=="
                    };
                    documentSettingsPart.Settings.Append(documentProtection);
                }
            }

            wordprocessingDocument.MainDocumentPart.Document.Save();
        }

        /// <summary>
        /// Sets the unique content control ids.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="existingIds">The existing ids.</param>
        public static void SetUniquecontentControlIds(OpenXmlCompositeElement element, IList<int> existingIds)
        {
            var randomizer = new Random();

            foreach (var sdtId in element.Descendants<SdtId>())
            {
                if (existingIds.Contains(sdtId.Val))
                {
                    var randomId = randomizer.Next(int.MaxValue);

                    while (existingIds.Contains(randomId))
                    {
                        randomizer.Next(int.MaxValue);
                    }

                    sdtId.Val.Value = randomId;
                }
                else
                {
                    existingIds.Add(sdtId.Val);
                }
            }
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="element">The SDT element.</param>
        /// <returns>
        /// Returns Tag of content control
        /// </returns>
        public static Tag GetTag(SdtElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return element.SdtProperties.Elements<Tag>().FirstOrDefault();
        }

        /// <summary>
        /// Ensures the unique content control ids for main document part.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        public static void EnsureUniqueContentControlIdsForMainDocumentPart(MainDocumentPart mainDocumentPart)
        {
            var contentControlIds = new List<int>();

            if (mainDocumentPart == null)
            {
                return;
            }

            foreach (var part in mainDocumentPart.HeaderParts)
            {
                SetUniquecontentControlIds(part.Header, contentControlIds);
                part.Header.Save();
            }

            foreach (var part in mainDocumentPart.FooterParts)
            {
                SetUniquecontentControlIds(part.Footer, contentControlIds);
                part.Footer.Save();
            }

            SetUniquecontentControlIds(mainDocumentPart.Document.Body, contentControlIds);
            mainDocumentPart.Document.Save();
        }

        /// <summary>
        /// Sets the tag value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="tagValue">The tag value.</param>
        public static void SetTagValue(SdtElement element, string tagValue)
        {
            var tag = GetTag(element);
            tag.Val.Value = tagValue;
        }

        /// <summary>
        /// Sets the content of content control.
        /// </summary>
        /// <param name="contentControl">The content control.</param>
        /// <param name="content">The content.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to break this down to a wrapper, multiple function format. Will do soon.")]
        public static void SetContentOfContentControl(SdtElement contentControl, string content)
        {
            if (contentControl == null)
            {
                throw new ArgumentNullException("contentControl");
            }

            content = string.IsNullOrEmpty(content) ? string.Empty : content;
            var isCombobox = contentControl.SdtProperties.Descendants<SdtContentDropDownList>().FirstOrDefault() != null;
            var isImage = contentControl.SdtProperties.Descendants<SdtContentPicture>().FirstOrDefault() != null;
            var prop = contentControl.Elements<SdtProperties>().FirstOrDefault();

            if (isCombobox)
            {
                var openXmlCompositeElement = GetSdtContentOfContentControl(contentControl);
                var run = CreateRun(openXmlCompositeElement, content);
                SetSdtContentKeepingPermissionElements(openXmlCompositeElement, run);
            }

            if (isImage)
            {
                string embed = null;
                Drawing dr = contentControl.Descendants<Drawing>().FirstOrDefault();
                if (dr != null)
                {
                    DocumentFormat.OpenXml.Drawing.Blip blip = dr.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().FirstOrDefault();
                    if (blip != null)
                    {
                        embed = blip.Embed;
                    }
                }

                if (embed != null)
                {
                    var document = (Document)prop.Ancestors<Body>().FirstOrDefault().Parent;
                    IdPartPair idpp = document.MainDocumentPart.Parts.Where(pa => pa.RelationshipId == embed).FirstOrDefault();
                    if (idpp != null)
                    {
                        ImagePart ip = (ImagePart)idpp.OpenXmlPart;
                        DirectoryInfo di = new DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        using (FileStream fileStream = File.Open(Path.Combine(di.Parent.FullName, content), FileMode.Open))
                        {
                            ip.FeedData(fileStream);
                        }
                    }
                }
            }
            else
            {
                var openXmlCompositeElement = GetSdtContentOfContentControl(contentControl);
                contentControl.SdtProperties.RemoveAllChildren<ShowingPlaceholder>();
                var runs = new List<Run>();

                if (IsContentControlMultiline(contentControl))
                {
                    var textSplitted = content.Split(Environment.NewLine.ToCharArray()).ToList();
                    var addBreak = false;

                    foreach (var textSplit in textSplitted)
                    {
                        var run = CreateRun(openXmlCompositeElement, textSplit);

                        if (addBreak)
                        {
                            run.AppendChild(new Break());
                        }

                        if (!addBreak)
                        {
                            addBreak = true;
                        }

                        runs.Add(run);
                    }
                }
                else
                {
                    runs.Add(CreateRun(openXmlCompositeElement, content));
                }

                SdtContentCell aopenXmlCompositeElement = openXmlCompositeElement as SdtContentCell;
                if (aopenXmlCompositeElement != null)
                {
                    AddRunsToSdtContentCell(aopenXmlCompositeElement, runs);
                }
                else if (openXmlCompositeElement is SdtContentBlock)
                {
                    var para = CreateParagraph(openXmlCompositeElement, runs);
                    SetSdtContentKeepingPermissionElements(openXmlCompositeElement, para);
                }
                else
                {
                    SetSdtContentKeepingPermissionElements(openXmlCompositeElement, runs);
                }
            }
        }

        /// <summary>
        /// Appends the documents to primary document.
        /// </summary>
        /// <param name="primaryDocument">The primary document.</param>
        /// <param name="documentstoAppend">The documents to append.</param>
        /// <returns>Returns the stream of bytes after the documents have been appended</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Don't have a fix for this just now")]
        public byte[] AppendDocumentsToPrimaryDocument(byte[] primaryDocument, IList<byte[]> documentstoAppend)
        {
            if (documentstoAppend == null)
            {
                throw new ArgumentNullException("documentstoAppend");
            }

            if (primaryDocument == null)
            {
                throw new ArgumentNullException("primaryDocument");
            }

            byte[] output = null;

            using (var finalDocumentStream = new MemoryStream())
            {
                finalDocumentStream.Write(primaryDocument, 0, primaryDocument.Length);

                using (var finalDocument = WordprocessingDocument.Open(finalDocumentStream, true))
                {
                    SectionProperties finalDocSectionProperties = null;
                    UnprotectDocument(finalDocument);

                    var tempSectionProperties =
                        finalDocument.MainDocumentPart.Document.Descendants<SectionProperties>().LastOrDefault();

                    if (tempSectionProperties != null)
                    {
                        finalDocSectionProperties = tempSectionProperties.CloneNode(true) as SectionProperties;
                    }

                    this.RemoveContentControlsAndKeepContents(finalDocument.MainDocumentPart.Document);

                    foreach (byte[] documentToAppend in documentstoAppend)
                    {
                        var subReportPart =
                            finalDocument.MainDocumentPart.AddAlternativeFormatImportPart(
                                AlternativeFormatImportPartType.WordprocessingML);
                        SectionProperties secProperties = null;

                        using (var docToAppendStream = new MemoryStream())
                        {
                            docToAppendStream.Write(documentToAppend, 0, documentToAppend.Length);

                            using (var docToAppend = WordprocessingDocument.Open(docToAppendStream, true))
                            {
                                UnprotectDocument(docToAppend);

                                tempSectionProperties =
                                    docToAppend.MainDocumentPart.Document.Descendants<SectionProperties>().LastOrDefault();

                                if (tempSectionProperties != null)
                                {
                                    secProperties = tempSectionProperties.CloneNode(true) as SectionProperties;
                                }

                                this.RemoveContentControlsAndKeepContents(docToAppend.MainDocumentPart.Document);
                                docToAppend.MainDocumentPart.Document.Save();
                            }

                            docToAppendStream.Position = 0;
                            subReportPart.FeedData(docToAppendStream);
                        }

                        if (documentstoAppend.ElementAtOrDefault(0).Equals(documentToAppend))
                        {
                            AssignSectionProperties(finalDocument.MainDocumentPart.Document, finalDocSectionProperties);
                        }

                        var altChunk = new AltChunk();
                        altChunk.Id = finalDocument.MainDocumentPart.GetIdOfPart(subReportPart);
                        finalDocument.MainDocumentPart.Document.AppendChild(altChunk);

                        if (!documentstoAppend.ElementAtOrDefault(documentstoAppend.Count - 1).Equals(documentToAppend))
                        {
                            AssignSectionProperties(finalDocument.MainDocumentPart.Document, secProperties);
                        }

                        finalDocument.MainDocumentPart.Document.Save();
                    }

                    finalDocument.MainDocumentPart.Document.Save();
                }

                finalDocumentStream.Position = 0;
                output = new byte[finalDocumentStream.Length];
                finalDocumentStream.Read(output, 0, output.Length);
            }

            return output;
        }

        /// <summary>
        /// Removes the content controls and keep contents.
        /// </summary>
        /// <param name="document">The document.</param>
        public void RemoveContentControlsAndKeepContents(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var customXmlPartCore = new CustomXmlPartCore(this.namespaceUri);
            var customXmlPart = customXmlPartCore.GetCustomXmlPart(document.MainDocumentPart);
            var customPartDoc = new XmlDocument();

            if (customXmlPart != null)
            {
                using (var reader = XmlReader.Create(customXmlPart.GetStream(FileMode.Open, FileAccess.Read)))
                {
                    customPartDoc.Load(reader);
                }
            }

            this.RemoveContentControlsAndKeepContents(document.Body, customPartDoc.DocumentElement);
            document.Save();
        }

        /// <summary>
        /// Removes the content controls and keep contents.
        /// </summary>
        /// <param name="compositeElement">The composite element.</param>
        /// <param name="customXmlPartDocElement">The custom XML part doc element.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Don't have a fix for this just now")]
        public void RemoveContentControlsAndKeepContents(OpenXmlCompositeElement compositeElement, XmlElement customXmlPartDocElement)
        {
            if (compositeElement == null)
            {
                throw new ArgumentNullException("compositeElement");
            }

            SdtElement acompositeElement = compositeElement as SdtElement;
            if (acompositeElement != null)
            {
                var elementsList = this.RemoveContentControlAndKeepContents(acompositeElement, customXmlPartDocElement);

                foreach (var innerCompositeElement in elementsList)
                {
                    this.RemoveContentControlsAndKeepContents(innerCompositeElement, customXmlPartDocElement);
                }
            }
            else
            {
                var childCompositeElements = compositeElement.Elements<OpenXmlCompositeElement>().ToList();

                foreach (var childCompositeElement in childCompositeElements)
                {
                    this.RemoveContentControlsAndKeepContents(childCompositeElement, customXmlPartDocElement);
                }
            }
        }

        /// <summary>
        /// Assigns the content from custom XML part for databound control.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="customPartDocElement">The custom part doc element.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Don't have a fix for this just now")]
        public void AssignContentFromCustomXmlPartForDataboundControl(SdtElement element, XmlElement customPartDocElement)
        {
            // This fix is applied only for data bound content controls. It was found MergeDocuments method was not picking up data from CustomXmlPart. Thus
            // default text of the content control was there in the Final report instead of the XPath value.
            // This method copies the text from the CustomXmlPart using XPath specified while creating the Binding element and assigns that to the
            // content control
            var binding = element.SdtProperties.GetFirstChild<DataBinding>();

            if (binding == null)
            {
                return;
            }

            if (binding.XPath.HasValue)
            {
                var path = binding.XPath.Value;

                if (customPartDocElement != null)
                {
                    var mgr = new XmlNamespaceManager(new NameTable());
                    mgr.AddNamespace("ns0", this.namespaceUri.ToString());
                    var node = customPartDocElement.SelectSingleNode(path, mgr);

                    if (node != null)
                    {
                        SetContentOfContentControl(element, node.InnerText);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether [is content control multiline] [the specified content control].
        /// </summary>
        /// <param name="contentControl">The content control.</param>
        /// <returns>
        ///   <c>true</c> if [is content control multiline] [the specified content control]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsContentControlMultiline(SdtElement contentControl)
        {
            var contentText = contentControl.SdtProperties.Elements<SdtContentText>().FirstOrDefault();

            var isMultiline = false;

            if (contentText != null && contentText.MultiLine != null)
            {
                isMultiline = contentText.MultiLine.Value;
            }

            return isMultiline;
        }

        /// <summary>
        /// Sets the SDT content keeping permission elements.
        /// </summary>
        /// <param name="openXmlCompositeElement">The open XML composite element.</param>
        /// <param name="newChild">The new child.</param>
        private static void SetSdtContentKeepingPermissionElements(OpenXmlCompositeElement openXmlCompositeElement, OpenXmlElement newChild)
        {
            var start = openXmlCompositeElement.Descendants<PermStart>().FirstOrDefault();
            var end = openXmlCompositeElement.Descendants<PermEnd>().FirstOrDefault();
            openXmlCompositeElement.RemoveAllChildren();

            if (start != null)
            {
                openXmlCompositeElement.AppendChild(start);
            }

            openXmlCompositeElement.AppendChild(newChild);

            if (end != null)
            {
                openXmlCompositeElement.AppendChild(end);
            }
        }

        /// <summary>
        /// Sets the SDT content keeping permission elements.
        /// </summary>
        /// <param name="openXmlCompositeElement">The open XML composite element.</param>
        /// <param name="newChildren">The new children.</param>
        private static void SetSdtContentKeepingPermissionElements(OpenXmlCompositeElement openXmlCompositeElement, IEnumerable<Run> newChildren)
        {
            var start = openXmlCompositeElement.Descendants<PermStart>().FirstOrDefault();
            var end = openXmlCompositeElement.Descendants<PermEnd>().FirstOrDefault();
            openXmlCompositeElement.RemoveAllChildren();

            if (start != null)
            {
                openXmlCompositeElement.AppendChild(start);
            }

            foreach (var newChild in newChildren)
            {
                openXmlCompositeElement.AppendChild(newChild);
            }

            if (end != null)
            {
                openXmlCompositeElement.AppendChild(end);
            }
        }

        /// <summary>
        /// Adds to list if composite element.
        /// </summary>
        /// <param name="elementsList">The elements list.</param>
        /// <param name="newElement">The new element.</param>
        private static void AddToListIfCompositeElement(IList<OpenXmlCompositeElement> elementsList, IEnumerable<OpenXmlElement> newElement)
        {
            var compositeElement = newElement as OpenXmlCompositeElement;

            if (elementsList == null)
            {
                throw new ArgumentNullException("elementsList");
            }

            if (compositeElement != null)
            {
                elementsList.Add(compositeElement);
            }
        }

        /// <summary>
        /// Assigns the section properties.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="secProperties">The sec properties.</param>
        private static void AssignSectionProperties(OpenXmlCompositeElement document, OpenXmlElement secProperties)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            if (secProperties == null)
            {
                return;
            }

            var pageSize = secProperties.Descendants<PageSize>().FirstOrDefault();

            if (pageSize != null)
            {
                pageSize.Remove();
            }

            var pageMargin = secProperties.Descendants<PageMargin>().FirstOrDefault();

            if (pageMargin != null)
            {
                pageMargin.Remove();
            }

            document.AppendChild(new Paragraph(new ParagraphProperties(new SectionProperties(pageSize, pageMargin))));
        }

        /// <summary>
        /// Creates the paragraph.
        /// </summary>
        /// <param name="openXmlCompositeElement">The open XML composite element.</param>
        /// <param name="runs">The runs.</param>
        /// <returns>Returns the paragraph element</returns>
        private static Paragraph CreateParagraph(OpenXmlCompositeElement openXmlCompositeElement, List<Run> runs)
        {
            var paragraphProperties = openXmlCompositeElement.Descendants<ParagraphProperties>().FirstOrDefault();
            Paragraph para;

            if (paragraphProperties != null)
            {
                para = new Paragraph(paragraphProperties.CloneNode(true));
                foreach (var run in runs)
                {
                    para.AppendChild(run);
                }
            }
            else
            {
                para = new Paragraph();
                foreach (var run in runs)
                {
                    para.AppendChild(run);
                }
            }

            return para;
        }

        /// <summary>
        /// Creates the run.
        /// </summary>
        /// <param name="openXmlCompositeElement">The open XML composite element.</param>
        /// <param name="content">The content.</param>
        /// <returns>Returns a run object</returns>
        private static Run CreateRun(OpenXmlCompositeElement openXmlCompositeElement, string content)
        {
            var runProperties = openXmlCompositeElement.Descendants<RunProperties>().FirstOrDefault();

            var run = runProperties != null
                          ? new Run(runProperties.CloneNode(true), new Text(content))
                          : new Run(new Text(content));

            return run;
        }

        /// <summary>
        /// Adds the runs to SDT content cell.
        /// </summary>
        /// <param name="sdtContentCell">The SDT content cell.</param>
        /// <param name="runs">The runs.</param>
        private static void AddRunsToSdtContentCell(OpenXmlCompositeElement sdtContentCell, IEnumerable<Run> runs)
        {
            var cell = new TableCell();
            var para = new Paragraph();
            para.RemoveAllChildren();

            foreach (var run in runs)
            {
                para.AppendChild(run);
            }

            cell.AppendChild(para);
            SetSdtContentKeepingPermissionElements(sdtContentCell, cell);
        }

        /// <summary>
        /// Removes the content control and keep contents.
        /// </summary>
        /// <param name="contentControl">The content control.</param>
        /// <param name="customXmlPartDocElement">The custom XML part doc element.</param>
        /// <returns>Returns the custom xml part document element list</returns>
        private IEnumerable<OpenXmlCompositeElement> RemoveContentControlAndKeepContents(SdtElement contentControl, XmlElement customXmlPartDocElement)
        {
            IList<OpenXmlCompositeElement> elementsList = new List<OpenXmlCompositeElement>();

            this.AssignContentFromCustomXmlPartForDataboundControl(contentControl, customXmlPartDocElement);

            foreach (var elem in GetSdtContentOfContentControl(contentControl).Elements())
            {
                var newElement = contentControl.Parent.InsertBefore(elem.CloneNode(true), contentControl);
                AddToListIfCompositeElement(elementsList, newElement);
            }

            contentControl.Remove();
            return elementsList;
        }

        #endregion
    }
}