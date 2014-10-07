//-----------------------------------------------------------------------
// <copyright file="HighlightTextBlock.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class HighlightTextBlock : TextBlock
    {
        public static new readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HighlightTextBlock), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(UpdateHighlighting)));

        public static readonly DependencyProperty HighlightPhraseProperty = DependencyProperty.Register("HighlightPhrase", typeof(string), typeof(HighlightTextBlock), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(UpdateHighlighting)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(HighlightTextBlock), new FrameworkPropertyMetadata(Brushes.Yellow, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(UpdateHighlighting)));

        public static readonly DependencyProperty IsCaseSensitiveProperty = DependencyProperty.Register("IsCaseSensitive", typeof(bool), typeof(HighlightTextBlock), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(UpdateHighlighting)));

        public string HighlightPhrase
        {
            get 
            { 
                return (string)this.GetValue(HighlightPhraseProperty); 
            }
            
            set 
            { 
                this.SetValue(HighlightPhraseProperty, value); 
            }
        }

        public new string Text
        {
            get 
            {
                return (string)this.GetValue(TextProperty); 
            }
            
            set 
            { 
                this.SetValue(TextProperty, value); 
            }
        }

        public Brush HighlightBrush
        {
            get 
            {
                return (Brush)this.GetValue(HighlightBrushProperty); 
            }
            
            set 
            {
                this.SetValue(HighlightBrushProperty, value); 
            }
        }

        public bool IsCaseSensitive
        {
            get 
            {
                return (bool)this.GetValue(IsCaseSensitiveProperty); 
            }
            
            set 
            {
                this.SetValue(IsCaseSensitiveProperty, value); 
            }
        }

        private static void UpdateHighlighting(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplyHighlight(d as HighlightTextBlock);
        }

        private static void ApplyHighlight(HighlightTextBlock textBlock)
        {
            string highlightPhrase = textBlock.HighlightPhrase;
            string text = textBlock.Text;

            if (string.IsNullOrEmpty(highlightPhrase))
            {
                textBlock.Inlines.Clear();

                textBlock.Inlines.Add(text);
            }
            else
            {
                int index = text.IndexOf(highlightPhrase, textBlock.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

                textBlock.Inlines.Clear();

                if (index < 0) 
                {
                    textBlock.Inlines.Add(text); 
                }
                else
                {
                    if (index > 0)
                    {
                        textBlock.Inlines.Add(text.Substring(0, index)); 
                    }
                     
                    textBlock.Inlines.Add(new Run(text.Substring(index, highlightPhrase.Length))
                    {
                        Background = textBlock.HighlightBrush
                    });

                    index += highlightPhrase.Length;
                    if (index < text.Length)
                    {
                        textBlock.Inlines.Add(text.Substring(index)); 
                    }                        
                }
            }
        }
    }
}
