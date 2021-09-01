using ControlsXL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace ControlsXL
{
    public enum FilteredDataGridType
    {
        TextBox,
        ComboBox
    }

    public class FilteredDataGridColumnHeader : DataGridColumnHeader
    {
        static FilteredDataGridColumnHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilteredDataGridColumnHeader), new FrameworkPropertyMetadata(typeof(FilteredDataGridColumnHeader)));
            StylesXL.StyleManager.Initialize();
        }
    }

    public class FilteredDataGrid : DataGrid
    {
        // Key is datacontext of the property bound to the column, value is the filter string
        private Dictionary<string, string> _FilterCache = new Dictionary<string, string>();

        // Key is property name, value is property info
        private Dictionary<string, PropertyInfo> _PropertyCache = new Dictionary<string, PropertyInfo>();

        static FilteredDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilteredDataGrid), new FrameworkPropertyMetadata(typeof(FilteredDataGrid)));
            StylesXL.StyleManager.Initialize();
        }
        /// <summary>
        /// Register for all text changed events
        /// </summary>
        public FilteredDataGrid() : base()
        {
            AddHandler(SearchTextBox.TextChangedEvent, new TextChangedEventHandler(FilterChanged), true);

            DataContextChanged += FilteredDataGridDataContextChanged;
        }

        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            base.OnAutoGeneratingColumn(e);

            try
            {
                // TODO: Implement different types - check binding of comboboxcolumn etc.

                // Store a reference to type of column that is to be generated
                var datagridColumnType = e.Column.GetType();

                // Loop through all possible types of auto generated columns and replace them with filterable columns
                if (datagridColumnType == typeof(DataGridTextColumn))
                {
                    FilteredDataGridTextColumn filteredDataGridTextColumn = new FilteredDataGridTextColumn();

                    filteredDataGridTextColumn.Binding = new Binding(e.PropertyName);
                    filteredDataGridTextColumn.Filter = false;
                    filteredDataGridTextColumn.Header = e.Column.Header;

                    e.Column = filteredDataGridTextColumn;
                }
                else if (datagridColumnType == typeof(DataGridTemplateColumn))
                {
                    //GridViewRowPresenter gvrp = this.GetVisualDescendants<GridViewRowPresenter>().FirstOrDefault();
                    //TextBlock tb = gvrp.GetVisualDescendants<TextBlock>().FirstOrDefault();
                    //string path = BindingOperations.GetBinding(tb, TextBlock.TextProperty).Path.Path;
                    //Console.WriteLine(path);
                }
                else if (datagridColumnType == typeof(DataGridComboBoxColumn))
                {
                }
                else if (datagridColumnType == typeof(DataGridCheckBoxColumn))
                {

                }
                else if (datagridColumnType == typeof(DataGridHyperlinkColumn))
                {

                }
            }
            catch (Exception)
            {
                // TODO: Error handling
            }
        }
        
        private void FilteredDataGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // If the data context of the data grid is changed, the property cache becomes unvalid
            _PropertyCache.Clear();
        }

        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)e.OriginalSource;

            DataGridColumnHeader columnHeader = TryFindParent<DataGridColumnHeader>(textbox);

            if(columnHeader != null)
            {
                UpdateFilter(textbox, columnHeader);
                ApplyFilters();
            }

        }

        private void UpdateFilter(TextBox textbox, DataGridColumnHeader columnHeader)
        {
            string propertyContext;

            if(columnHeader != null)
            {
                // Get column bound property data context name
                propertyContext = columnHeader.DataContext.ToString();
            }
            else
            {
                propertyContext = string.Empty;
            }

            if (!string.IsNullOrEmpty(propertyContext))
                _FilterCache[propertyContext] = textbox.Text;
        }

        private void ApplyFilters()
        {
            //CommitEdit();
            //CommitEdit();

            // Get the default view for the data grid data source
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(ItemsSource);

            if(collectionView != null)
            {
                collectionView.Filter = delegate (object item)
                {
                    bool show = true;

                    foreach(var filter in _FilterCache)
                    {
                        object property = GetPropertyValue(item, filter.Key);

                        if(property != null)
                        {

                            bool isMatching = property.ToString().Like(filter.Value);

                            if(!isMatching)
                            {
                                show = false;
                                break;
                            }
                        }
                    }

                    return show;
                };
            }
        }

        /// <summary>
        /// Get the value of a property
        /// </summary>
        /// <param name="item"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private object GetPropertyValue(object item, string propertyName)
        {
            // Get property  from cache
            PropertyInfo propertyInfo = null;

            if (_PropertyCache.ContainsKey(propertyName))
            {
                // Get the property from the cache by name
                propertyInfo = _PropertyCache[propertyName];
            }
            else
            {
                // Get the property info of the item
                propertyInfo = item.GetType().GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    // Add the property into the cache
                    _PropertyCache.Add(propertyName, propertyInfo);
                }

                //propertyInfo = item.GetType().GetProperty(propertyName);

                //if (propertyInfo != null)
                //{
                //    _PropertyCache.Add(propertyName, propertyInfo);
                //}
                //else
                //{
                //    object propertyValue = GetPropertyValueRecursive(item, propertyName);

                //    if (propertyValue != null)
                //    {
                //        return propertyValue;
                //    }
                //    else
                //    {
                //        string[] path = propertyName.Split('.');
                //        string propertyException = path[path.Count() - 1];


                //        throw new Exception($"Unable to resolve the filter for property {propertyException} to table {item.GetType().Name} from the path {propertyName}.");
                //    }
                //}

            }

            // Return the value from the property info
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(item, null);
            }

            Debug.Print($"[{nameof(FilteredDataGrid)}.{nameof(GetPropertyValue)}] NULL");

            return null;
        }

        public object GetPropertyValueRecursive(object item, string propertyPath)
        {
            foreach (var property in propertyPath.Split('.').Select(s => item.GetType().GetProperty(s)))
            {
                item = property.GetValue(item, null);
            }

            return item;
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null reference is being returned.</returns>
        public static T TryFindParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindParent<T>(parentObject);
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Do note, that for content element,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise null.</returns>
        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) return null;
            ContentElement contentElement = child as ContentElement;

            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            // If it's not a ContentElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }
    }

    public interface IFilteredDataGridColumn
    {
        BindingBase Binding { get; set; }
        bool Filter { get; set; }
    }


    public class FilteredDataGridTextColumn : DataGridTextColumn, IFilteredDataGridColumn
    {
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(FilteredDataGridType), typeof(FilteredDataGridTextColumn), new UIPropertyMetadata(FilteredDataGridType.TextBox));

        public FilteredDataGridType FilterType
        {
            get { return (FilteredDataGridType)GetValue(FilterTypeProperty); }
            set { SetValue(FilterTypeProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(bool), typeof(FilteredDataGridTextColumn), new UIPropertyMetadata(true));

        public bool Filter
        {
            get { return (bool)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
    }

    public class FilteredDataDataGridCheckBoxColumn : DataGridCheckBoxColumn, IFilteredDataGridColumn
    {
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(bool), typeof(FilteredDataDataGridCheckBoxColumn), new UIPropertyMetadata(true));

        public bool Filter
        {
            get { return (bool)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
    }

    public class FilteredDataGridTemplateColumn : DataGridTemplateColumn
    {
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(bool), typeof(FilteredDataGridTemplateColumn), new UIPropertyMetadata(false));

        public bool Filter
        {
            get { return (bool)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
    }

    public class FilteredDataGridComboBoxColumn : DataGridComboBoxColumn
    {
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(bool), typeof(FilteredDataGridComboBoxColumn), new UIPropertyMetadata(true));

        public bool Filter
        {
            get { return (bool)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
    }

    public class FilteredDataGridHyperlinkColumn : DataGridHyperlinkColumn, IFilteredDataGridColumn
    {
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(bool), typeof(FilteredDataGridHyperlinkColumn), new UIPropertyMetadata(true));

        public bool Filter
        {
            get { return (bool)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
    }

    public static class FilteredDataGridRowHelper
    {
        public static readonly DependencyProperty SelectionUnitProperty = DependencyProperty.RegisterAttached("SelectionUnit", typeof(DataGridSelectionUnit), typeof(FilteredDataGridRowHelper), new FrameworkPropertyMetadata(DataGridSelectionUnit.FullRow));

        /// <summary>
        /// Gets the value to define the DataGridRow selection behavior.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(DataGridRow))]
        public static DataGridSelectionUnit GetSelectionUnit(UIElement element)
        {
            return (DataGridSelectionUnit)element.GetValue(SelectionUnitProperty);
        }

        /// <summary>
        /// Sets the value to define the DataGridRow selection behavior.
        /// </summary>
        public static void SetSelectionUnit(UIElement element, DataGridSelectionUnit value)
        {
            element.SetValue(SelectionUnitProperty, value);
        }
    }

    public class FilteredHeaderConverter : IMultiValueConverter
    {
        /// <summary>
        /// Create a special header for a filtered datagrid column by addin the search criteria & filter symbol.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Get the values from the parameter object
            string filter = values[0] as string;
            string headerText = values[1] as string;

            // String to store the xaml generation
            string xaml;

            // String parameters for easy & clear xaml generation            
            string open = "<";
            string xamlTextBlockHeader = @"TextBlock xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ";
            string controlsNamespace = @"xmlns:xl='clr-namespace:ControlsXL;assembly=ControlsXL'";
            string runText = "<Run Text='";
            string runTextNormal = "<Run FontWeight='normal' Text='";
            string runTextLight = "<Run FontWeight='light' Text='";
            string runTextNormalItalic = "<Run FontWeight='normal' FontStyle='italic' Text='";
            string columnHeader = headerText;
            string filterText = filter.Replace(" ", "...") + "...";
            string vectorButton = "<xl:VectorButton Width='16' HorizontalAlignment = 'Center' VerticalAlignment = 'Bottom' Vector = 'F1 M 34.8333,61.75L 34.8333,42.75L 19,20.5833L 57,20.5833L 41.1667,42.75L 41.1667,58.5833L 34.8333,61.75 Z'/>";
            string headerSpacing = "  ";
            string filterCaption = "FILTER ";
            string filterSeparator = "= ";
            string closingTextBlock = "</TextBlock>";
            string openingBracket = "(";
            string closingBracket = ")";
            string closingTag = @"'/>";
            string close = ">";

            // Start generating the filtered column header xaml
            // <TextBlock xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:xl='clr-namespace:RecipeManager.Controls;assembly=RecipeManagerControls'>
            xaml = open;
            xaml += xamlTextBlockHeader;
            xaml += controlsNamespace;
            xaml += close;

            // <Run Text='[HEADER NAME]'/>
            xaml += runText + columnHeader + closingTag;

            if (!String.IsNullOrEmpty(filter))
            {
                // <Run FontWeight='normal' Text='  '/>
                xaml += runTextNormal + headerSpacing + closingTag;

                // <Run FontWeight='light' Text='['/>
                xaml += runTextNormal + openingBracket + closingTag;

                // <xl:VectorButton Width='16' HorizontalAlignment = 'Center' VerticalAlignment = 'Bottom' Vector = 'F1 M 34.8333,61.75L 34.8333,42.75L Z'/>
                //xaml += vectorButton;

                // <Run FontWeight='light' Text='= '/>
                //xaml += runTextLight + filterSeparator + closingTag;

                // <Run FontStyle='italic' Text='[Filter*Criteria]'/>
                xaml += runTextNormalItalic + filterText + closingTag;

                // <Run FontWeight='light' Text=' ]'/>
                xaml += runTextNormal + closingBracket + closingTag;
            }

            // </TextBlock>
            xaml += closingTextBlock;

            // Convert the generated text to a memory stream
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xaml));

            // Convert the memory stream to an actual textblock object
            TextBlock block = (TextBlock)System.Windows.Markup.XamlReader.Load(stream);

            return block;
        }
        //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    // Get values
        //    string filter = values[0] as string;
        //    string headerText = values[1] as string;

        //    // Generate header text
        //    string text = "{0}{3}" + headerText + " {4}";
        //    if (!String.IsNullOrEmpty(filter))
        //        text += "(Filter: {2}" + values[0] + "{4})";
        //    text += "{1}";

        //    // Escape special XML characters like <>&'
        //    text = new System.Xml.Linq.XText(text).ToString();

        //    // Format the text
        //    text = String.Format(text,
        //     @"<TextBlock xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>",
        //     "</TextBlock>", "<Run FontWeight='bold' Text='",
        //     "<Run Text='", @"'/>");

        //    // Convert to stream
        //    MemoryStream stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(text));

        //    // Convert to object
        //    TextBlock block = (TextBlock)System.Windows.Markup.XamlReader.Load(stream);
        //    return block;
        //}

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

   
}
