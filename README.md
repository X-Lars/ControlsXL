# ControlsXL
WPF Custom Controls Library
###### <i> This is currently a work in progress, styles are gonna be bound to the [StylesXL](https://github.com/X-Lars/StylesXL) for styling which should also support custom skin colors.</i>

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
