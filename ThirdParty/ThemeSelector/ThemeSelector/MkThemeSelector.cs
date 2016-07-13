using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace mkLibrary.ThemeSelector
{
    /// <summary>
    /// Inherits ResourceDictionary, used to identify a theme resource dictionary in the merged dictionaries collection
    /// </summary>
    public class ThemeResourceDictionary : ResourceDictionary
    {
    }

    public class MkThemeSelector : DependencyObject
    {

        public static readonly DependencyProperty CurrentThemeDictionaryProperty =
            DependencyProperty.RegisterAttached("CurrentThemeDictionary", typeof(Uri),
            typeof(MkThemeSelector),
            new UIPropertyMetadata(null, CurrentThemeDictionaryChanged));

        public static readonly DependencyProperty IsGlobalThemeProperty =
            DependencyProperty.RegisterAttached("IsGlobalTheme", typeof(bool),
            typeof(MkThemeSelector),
            null);

        public static Uri GetCurrentThemeDictionary(DependencyObject obj)
        {
            return (Uri)obj.GetValue(CurrentThemeDictionaryProperty);
        }

        public static void SetCurrentThemeDictionary(DependencyObject obj, Uri value)
        {
            obj.SetValue(CurrentThemeDictionaryProperty, value);
        }

        public static bool GetIsGlobalTheme(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsGlobalThemeProperty);
        }

        public static void SetIsGlobalTheme(DependencyObject obj, bool value)
        {
            obj.SetValue(IsGlobalThemeProperty, value);
        }

        private static void CurrentThemeDictionaryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(GetIsGlobalTheme(obj))
            {
                ApplyGlobalTheme(GetCurrentThemeDictionary(obj));
            }
            else if (obj is FrameworkElement) // works only on FrameworkElement objects
            {
                ApplyTheme(obj as FrameworkElement, GetCurrentThemeDictionary(obj));
            }
        }

        private static void ApplyGlobalTheme(Uri dictionaryUri)
        {
            if (Application.Current == null
                || Application.Current.Resources == null)
            {
                return;
            }

            ApplyTheme(Application.Current.Resources.MergedDictionaries, dictionaryUri);
        }

        private static void ApplyTheme(FrameworkElement targetElement, Uri dictionaryUri)
        {
            if (targetElement == null
                || targetElement.Resources == null)
            {
                return;
            }

            ApplyTheme(targetElement.Resources.MergedDictionaries, dictionaryUri);
        }

        private static void ApplyTheme(Collection<ResourceDictionary> mergedDictionaries, Uri dictionaryUri)
        {
            if (mergedDictionaries == null) return;

            try
            {
                ThemeResourceDictionary themeDictionary = null;
                if (dictionaryUri != null)
                {
                    themeDictionary = new ThemeResourceDictionary();
                    themeDictionary.Source = dictionaryUri;

                }

                // Find if the target element already has a theme applied
                // we always want to remove them in case 'None' was selected.
                var existingDictionaries = mergedDictionaries
                    .OfType<ThemeResourceDictionary>()
                    .ToList();

                // Remove the existing dictionaries 
                foreach (ThemeResourceDictionary thDictionary in existingDictionaries)
                {
                    mergedDictionaries.Remove(thDictionary);
                }

                if (themeDictionary != null)
                {
                    // Add the new dictionary to the collection of merged dictionaries of the target object
                    mergedDictionaries.Insert(0, themeDictionary);
                }
            }
            finally { }
        }
    }
}
