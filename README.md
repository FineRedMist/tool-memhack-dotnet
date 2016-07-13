From http://svetoslavsavov.blogspot.ca/2009/07/switching-wpf-interface-themes-at.html (converted to markdown).

# Switching WPF interface themes at runtime

WPF introduced the use of dynamic resource references. They're very helpful in rich application development. The dynamic resources are loaded at runtime, and can be changed at any time. Dynamic resources could be styles, brushes, etc. WPF however does not provide a convenient way of changing the resource references. That's why I've created a simple class that allows the developers to switch different resources very easy.

## Using dynamic resources

Creating and applying a dynamic resource is easy in WPF. First, we need to define the resource:

![Image1](Documentation/Resources/screen1.png "Image 1")

Then, we have to reference the resource like this:

![Image2](Documentation/Resources/screen2.png "Image 2")

And that's it. WPF searches for a resource with the given key and applies it when it's found.

## Changing the dynamic resources at runtime

As I said, dynamic resources are applied at runtime, that means they can be changed. Let's assume that we have defined several resources like brushes and styles, and we want them to be in two variants - for example a red theme and a blue theme for user interfaces. We put all of the resources for each theme in a different resource dictionary file. For this example, I use the WPF Themes which can be found here: http://wpf.codeplex.com/Wiki/View.aspx?title=WPF%20Themes. So, assume that we have two resource dictionaries: 

* ShinyBlue.xaml
* ShinyRed.xaml

Without these themes, a WPF window should look something like this:

![Image4](Documentation/Resources/screen4.png "Image 4")

Using the themes is pretty easy, all we need to do is merge one of the resource dictionary to the resources of root control or window:

![Image3](Documentation/Resources/screen3.png "Image 3")

And now, the window looks like that:

![Image5](Documentation/Resources/screen5.png "Image 5")

Pretty neat, huh? OK, that's good, but what if we want to change the theme to the red one? We have to go in the XAML code, change the source of the merged resource dictionary and recompile. No way! We should be able to come up with something nicer.

Well, there's a solution. See, everytime when the resources of a framework element are changed (added or removed resources), WPF goes through all elements with dynamic resource references and updates them accordingly. So the solution should be obvious - when we need to change the theme, we can simply remove the old merged dictionary, and add the new one. I searched a bit in the internet, and the most common solution is to clear all merged dictionaries from the collection and then add the desired one. Yes, allright, that would work. But what if the developer has added more than one resource dictionary, not only the one with the theme resources? Everything goes away, and bang, the software is not working. So there should be a proper way of detecting which resource dictionary contains the theme resources, and leave the other dictionaries alone.

It sounds a bit complicated, right? First, search for the right resource dictionary, then remove it from the list of merged dictionaries, then load the new one, and apply it. Yes but what if it could be done jyst by setting one single value to one single property, and all is OK?

## The ThemeSelector class

So there is it. The solution. I created a class which has an attachable property - the URI path to the desired theme dictionary. Now, let's think about finding the right dictionary to be removed when themes are being switched. Kinda obvious solution is a new class that inherits ResourceDictionary. Then, we search all merged dictionaries and remove those which are of this new type. Pretty simple, right? Here's the class: 

```
public class ThemeResourceDictionary : ResourceDictionary { }
```

So it's time to see the real deal. 

```
public class MkThemeSelector : DependencyObject 
{ 
    public static readonly DependencyProperty CurrentThemeDictionaryProperty =
        DependencyProperty.RegisterAttached("CurrentThemeDictionary", 
                                            typeof(Uri), 
                                            typeof(MkThemeSelector), 
                                            new UIPropertyMetadata(null, CurrentThemeDictionaryChanged)); 
    
    public static Uri GetCurrentThemeDictionary(DependencyObject obj) 
    {
        return (Uri)obj.GetValue(CurrentThemeDictionaryProperty); 
    }
     
    public static void SetCurrentThemeDictionary(DependencyObject obj, Uri value)
    {
        obj.SetValue(CurrentThemeDictionaryProperty, value); 
    } 
    
    private static void CurrentThemeDictionaryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) 
    {
        if (obj is FrameworkElement) // works only on FrameworkElement objects 
        {
            ApplyTheme(obj as FrameworkElement, GetCurrentThemeDictionary(obj));
        }
    }
    
    private static void ApplyTheme(FrameworkElement targetElement, Uri dictionaryUri) 
    {
        if (targetElement == null)
            return;
            
        try
        {
            ThemeResourceDictionary themeDictionary = null;
            if (dictionaryUri != null)
            {
                themeDictionary = new ThemeResourceDictionary();
                themeDictionary.Source = dictionaryUri; // add the new dictionary to the collection of merged dictionaries of the target object 
                targetElement.Resources.MergedDictionaries.Insert(0, themeDictionary); 
            }
            
            // find if the target element already has a theme applied 
            List existingDictionaries = (from dictionary in targetElement.Resources.MergedDictionaries.OfType() select dictionary).ToList();
            
            // remove the existing dictionaries
            foreach (ThemeResourceDictionary thDictionary in existingDictionaries) 
            {
                if (themeDictionary == thDictionary)
                    continue; // don't remove the newly added dictionary 
                targetElement.Resources.MergedDictionaries.Remove(thDictionary); 
            }
        } 
        finally 
        {
        }
    }
}
```

As I said, the class as one dependency property and an callback method to handle the event of changing the value of this property. There everything is straight-forward. First, the new theme dictionary is loaded, and then the old one is removed. That's it.

## Using the ThemeSelector class

Here comes the nice part. The usage of the class is as simple as changing the value of one single property. 

```
private void ChangeToRedTheme()
{
    MkThemeSelector.SetCurrentThemeDictionary(this, new Uri("/ThemeSelector;component/Themes/ShinyRed.xaml", UriKind.Relative));
}
```

When this method is called, the theme changes:

![Image8](Documentation/Resources/screen8.png "Image 8")

## The ThemeSelector class and WPF data binding

We can even use databinding to change the themes dynamically. Let's assume that we have a combo box which have two items - the red theme, and the blue theme:

![Image6](Documentation/Resources/screen6.png "Image 6")

Now, on the element to which we want to apply the theme, for example the root grid in the window, we set the following binding expression: 

```
local:MkThemeSelector.CurrentThemeDictionary="{Binding ElementName=cmbThemes, Path=SelectedItem.Tag}"
```

So it looks like this:

![Image7](Documentation/Resources/screen7.png "Image 7")

And that's all. When we run the application, we have this combo box, allowing us to select the theme in runtime.

![Image9](Documentation/Resources/screen9.png "Image 9")
