using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Next.Dtos;

namespace NextTests.Helpers
{
    public class CreatePropertyGrid
    {
        /// <summary>
        /// Generates the Xaml for showing all properties of a class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bindingPopertyName"></param>
        [TestCase(typeof(InstrumentMatch),"Instrument")]
        public void CreateGridTest(Type type,string bindingPopertyName)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            var grid = new XElement("Grid");
            grid.Add(new XAttribute("DataContext",string.Format( @"{{Binding {0}}}",bindingPopertyName)));
            var rows = new XElement("Grid.RowDefinitions");
            foreach (var propertyInfo in propertyInfos)
            {
                var rowDef = new XElement("RowDefinition");
                rowDef.Add(new XAttribute("Height",@"Auto"));
                rows.Add(rowDef);
            }
            grid.Add(rows);
            var columns = new XElement("Grid.ColumnDefinitions");
            var col1 = new XElement("ColumnDefinition");
            col1.Add(new XAttribute("Width", "Auto"));
            var col2 = new XElement("ColumnDefinition");
            columns.Add(col1);
            columns.Add(col2);
            grid.Add(columns);
            for (int index = 0; index < propertyInfos.Length; index++)
            {
                var propertyInfo = propertyInfos[index];
                var label = new XElement("Label",propertyInfo.Name);
                label.Add(new XAttribute("Grid.Row", index.ToString()));
                label.Add(new XAttribute("Grid.Column", 0.ToString()));
                grid.Add(label);
                var textBox = new XElement("TextBox");
                textBox.Add(new XAttribute("Grid.Row", index.ToString()));
                textBox.Add(new XAttribute("Grid.Column", 1.ToString()));
                textBox.Add(new XAttribute("Text",string.Format( @"{{Binding {0}}}",propertyInfo.Name)));
                grid.Add(textBox);
            }
            Console.Write(grid.ToString());
        }
    }
}
