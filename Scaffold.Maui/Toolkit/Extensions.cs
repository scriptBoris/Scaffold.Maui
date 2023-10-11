using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit
{
    public static class Extensions
    {
        public static string GetDisplayItemText(this object self, string? displayProperty)
        {
            object? currentObject = self;
            if (!string.IsNullOrWhiteSpace(displayProperty))
            {
                string[] propertyNames = displayProperty.Split('.');

                foreach (string propertyName in propertyNames)
                {
                    var propertyInfo = currentObject.GetType().GetProperty(propertyName);
                    if (propertyInfo == null)
                    {
                        currentObject = null;
                        break;
                    }

                    currentObject = propertyInfo.GetValue(currentObject);
                    if (currentObject == null)
                    {
                        currentObject = null;
                        break;
                    }
                }

                if (currentObject == null)
                    currentObject = self;
            }

            return currentObject.ToString() ?? self.GetType().Name;
        }
    }
}
