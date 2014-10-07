//-----------------------------------------------------------------------
// <copyright file="DocumentDetail.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    public class DocumentDetail
    {
        public string Title { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }
    }
}