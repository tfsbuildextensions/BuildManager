//-----------------------------------------------------------------------
// <copyright file="Extensions.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    internal static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            // Order by and put into a list.
            List<T> sorted = collection.OrderBy(x => x).ToList();

            // Loop the list and exchange items in the collection.
            for (int i = sorted.Count() - 1; i >= 0; i--)
            {
                collection.Insert(0, sorted[i]);
                collection.RemoveAt(collection.Count - 1);
            }
        }
    }
}
