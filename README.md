XAML Magic
==========
A fork of the Xaml Styler plugin found on https://github.com/NicoVermeir/XamlStyler. This fork has been created to accelerate development on the plugin originally created by Chris Chaochen (http://xamlstyler.codeplex.com) and further developed by NicoVermeir. This fork will only support Visual Studio 2015 and later. For Visual Studio 2012 support please install Chris's version. For other Visual Studio versions, consider installing Nico's version.

Features
==========
* Format XAML markup with the push of a button.

<img src="http://i59.tinypic.com/fehok3.jpg" alt="beautify option" />

* Sort attributes based on following rules:
  * x:Class 
  * XML Namespaces 
    * WPF built-in namespaces 
    *  User defined namespaces 
  * Key, Name or Title attributes 
    * x:Key 
    * x:Name 
    * Title 
  * Grid or Canvas related attached layout attributes 
    * Numeric layout attributes Width/MinWidth/MaxWidth 
    * Height/MinHeight/MaxHeight 
    * Margin 
  * Alignment related attributes HorizontalAlignment/ContentHorizontalAlignment 
    * VerticalAlignment/ContentVerticalAlignment 
    * Panel.ZIndex 
  * Other attributes 
* Short attributes tolerance. 
  * When an element contains 2 or less than 2 attributes, line break is not applied for * better readability. 
* Special characters(e.g., &) are preserved. 
* Respect "significant" whitespace situation. 
  * No new linefeed will be added to <Run/>, if it is immediatly following *another element to prevent the rendering of unexpected space. 

<table>
<tbody>
<tr>
<th width="350">Significant Whitespace between &lt;Run/&gt;<br>
</th>
<th>&nbsp;</th>
<th width="350">No Whitespace between &lt;Run/&gt;</th>
</tr>
<tr>
<td><img src="http://xamlstyler.codeplex.com/download?DownloadId=156790" alt="" width="125" height="72"></td>
<td>&nbsp;vs</td>
<td><img src="http://xamlstyler.codeplex.com/download?DownloadId=156789" alt="" width="84" height="78"></td>
</tr>
<tr>
<td>
<div>
<pre><span>&lt;</span><span>TextBlock</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>A<span>&lt;</span><span>Run</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>B<span>&lt;</span><span>Run</span><span>&gt;</span><br><span>&lt;/</span><span>TextBlock</span><span>&gt;</span><br></pre>
</div>
</td>
<td>&nbsp;vs</td>
<td>
<div>
<pre><span>&lt;</span><span>TextBlock</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>A<span>&lt;</span><span>Run</span><span>&gt;</span><span>&lt;</span><span>Run</span><span>&gt;</span>B<span>&lt;</span><span>Run</span><span>&gt;</span><br><span>&lt;/</span><span>TextBlock</span><span>&gt;</span><br></pre>
</div>
</td>
</tr>
</tbody>
</table>

* Indent XAML markup based on "Tab Size/Indent Size/Indent Charater" settings available in "Option/Text Editor/XAML/Tabs" page. 

<img src="http://i60.tinypic.com/106x5pi.jpg" alt="markup settings" />

* XAML Magic specific options. 
  * Define your own attribute ordering rules 
  * Define your own attribute line break rules 
  * Markup extension formatting 
  * Automatically reformat XAML file on saving 
  
<img src="http://i62.tinypic.com/11tbpqp.jpg" alt="xamlstyler options" /> 
  
* Import/Export XAML Magic settings. 

<img src="http://i59.tinypic.com/o8doon.jpg" alt="export settings" />

Contribute
==========
* Download the Visual Studio 2015 SDK from https://msdn.microsoft.com/en-us/library/bb166441.aspx
* Fork the XamlMagic project into your own GitHub account
* Develop some awesome features
* Create a pull request when ready
* Wait for us to merge your request
