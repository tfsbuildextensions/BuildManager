//-----------------------------------------------------------------------
// <copyright file="BuildImport.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    public class BuildImport
    {
        public string JsonFile { get; set; }

        public string Status { get; set; }

        public string StatusImage { get; set; }

        public string Message { get; set; }
    }
}
