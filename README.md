# ControlsXL
WPF Custom Controls Library
###### <i> This is currently a work in progress, styles are gonna be bound to the [StylesXL](https://github.com/X-Lars/StylesXL) for styling which should also support custom skin colors.</i>

## EnumeratedText
Plain text bound to an property of the type enum and displays the enum names.
Use the mouse wheel to modify the value or click the control to set keyboard focus and use the predefined keys. (+, -, HOME, END, PGUP, PGDN).
If I, in the below example, did not added the type converter, the values _164T, _164, _132T, etc. would show. When you don't hover the control with the mouse, the control is shown as plain text.

Examples | Inside a treeview
----|-------
![Alt text](/Images/EnumeratedTextToolTip.jpg)|![Alt text](/Images/EnumeratedTextUse.jpg)
![Alt text](/Images/EnumeratedText.jpg)

In XAML actually only the minimal binding to the `Value` is required.
```
<xl:EnumeratedText FontSize="20"
                   HorizontalContentAlignment="Center"
                   Suffix="Note"
                   Value="{Binding Notes}"/>
```
The converter I use can be found [here](https://github.com/X-Lars/ControlsXL/blob/master/ControlsXL/Common/DescriptionConverter.cs), but if your enum has meaningful names there is not use for a converter. The values are ordered by the integer value of the enum but don't have the be continuous.
```
[TypeConverter(typeof(DescriptionConverter))]
public enum NoteEnum : int
{
    [Description("\U0001D163\U00002083")] _164T = 0x00,
    [Description("\U0001D163")]           _164  = 0x01,
    [Description("\U0001D162\U00002083")] _132T = 0x02,
    [Description("\U0001D162")]           _132  = 0x03,
    [Description("\U0001D161\U00002083")] _116T = 0x04,
    [Description("\U0001D162\U0001D16D")] _132D = 0x05
}

public NoteEnum Notes
{
    get => _Note;
    set => _Note = value;
}
```
## NumericTextBox
Text box limited to min and max values, with optional value prefix or suffix and supports binding to a `List<string>`.
Optionally the increment and decrement buttons can be hidden because the control can be operated with the mouse wheel and predefined keys.

Key | Action
----|-------
'+', 'UP ARROW', 'MOUSEWHEEL UP'| Increment by Interval
'+' + 'CTRL', 'UP' + 'CTRL' | Increment by Interval * 10
'-', 'DOWN ARROW', 'MOUSEWHEEL DOWN'| Decrement by Interval
'-' + 'CTRL', 'DOWN' + 'CTRL' | Decrement by Interval * 10
'PAGEUP'| Increment to Max
'PAGEDOWN'| Decrement to Min

#### Examples
###### <i>Simple setup with prefix</i>
![Alt text](/Images/NumericTextBox-Horizontal-Prefix.jpg)

Creates a `NumericTextbox` with default horizontal `Orientation` and `Interval` of 1.
```
<xl:NumericTextBox 
    Min="-15" Max="15" 
    Prefix="Freq"
    Value="10"/>
```
###### <i>Vertical orientation and inverted values</i>
![Alt text](/Images/NumericTextBox-Vertical-Suffix.jpg)

Creates a `NumericTextbox` with vertical `Orientation` and `Interval` of 12. In this case the `Value` can only be -12, -24 or -36. Important note, because the `Min` and `Max`
are reversed, i.e. -36 is smaller than -12, the `Index` is reversed too, index 0 is bound to -12 and index 2 is bound to -36.
```
<xl:NumericTextBox
    Min="-12" Max="-36"
    Index="1"
    Interval="12"
    Orientation="Vertical"
    Suffix="Hz"/>
```
###### <i>Hidden buttons and precision</i>
![Alt text](/Images/NumericTextBox-NoButtons-Interval.jpg)

Creates a `NumericTextbox` where the buttons are hidden, use the mouse wheel or keys to modify the `Value`. The display precision is automatically adapted
from the `Interval`.
```
<xl:NumericTextBox
  Min="0" Max="127"
  Interval="0.5"
  Orientation="Vertical"
  Suffix="Hz"
  ShowButtons="False"
  Value="10"/>
```
###### <i>List binding</i>
![Alt text](/Images/NumericTextBox-Horizontal-List.jpg)

Creates a `NumericTextbox` bound to a list, use the `ValueProvider` property for binding, the `Index` property will contain the selected list index.
Incrementing and decrementing will cycle through the list values.
```
// Example property to bind
public List<string> Values
{
  get 
    {
      List<string> list = new List<string>();
      list.Add("A");
      list.Add("B");
      list.Add("C");
      return list;
    }
}
```

```
<xl:NumericTextBox
    Index="0"
    ValueProvider="{Binding Values}"/>
```
