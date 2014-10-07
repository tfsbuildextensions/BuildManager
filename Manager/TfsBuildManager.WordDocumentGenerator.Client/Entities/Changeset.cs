//-----------------------------------------------------------------------
// <copyright file="Changeset.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    using System;

    public class Changeset
    {
        public string ChangesetId { get; set; }

        public string ChangesetComment { get; set; }

        public string ChangesetCommittedBy { get; set; }

        public string ChangesetCommittedOn { get; set; }

        public Uri ChangesetUri { get; set; }
    }
}