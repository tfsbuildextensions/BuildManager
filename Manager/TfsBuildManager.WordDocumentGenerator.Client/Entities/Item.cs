//-----------------------------------------------------------------------
// <copyright file="Item.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    using System;

    public class Item
    {
        private Guid id = Guid.Empty;
        private string name = string.Empty;

        public Guid Id
        {
            get
            {
                return this.id;
            }

            set 
            {
                this.id = value;
            }
        }
        
        public string Name
        {
            get 
            {
                return this.name;
            }

            set 
            {
                this.name = value;
            }
        }
    }
}