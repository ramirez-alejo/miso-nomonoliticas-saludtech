using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Infraestructura.MessageBroker
{
    /// <summary>
    /// Utility class for converting models with Guid[] properties to models with string[] properties
    /// to work around Pulsar schema limitations.
    /// </summary>
    public static class GuidArrayConverter
    {
        /// <summary>
        /// Converts a model with Guid[] properties to a model with string[] properties
        /// </summary>
        /// <typeparam name="T">The type of the model</typeparam>
        /// <param name="model">The model to convert</param>
        /// <returns>A new model with Guid[] properties converted to string[]</returns>
        public static T ConvertGuidArraysToStringArrays<T>(T model) where T : class, new()
        {
            if (model == null)
            {
                return null;
            }
            
            // Create a new instance of the model
            var result = new T();
            
            // Get all properties of the model
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                
                if (value == null)
                {
                    continue;
                }
                
                // Check if the property is a Guid[]
                if (property.PropertyType == typeof(Guid[]))
                {
                    // Convert Guid[] to string[]
                    var guidArray = (Guid[])value;
                    var stringArray = guidArray.Select(g => g.ToString()).ToArray();
                    
                    // Create a dynamic property with the same name but of type string[]
                    var dynamicProperty = typeof(T).GetProperty(property.Name + "AsStrings");
                    if (dynamicProperty != null && dynamicProperty.PropertyType == typeof(string[]))
                    {
                        dynamicProperty.SetValue(result, stringArray);
                    }
                }
                else
                {
                    // Copy the property value as is
                    property.SetValue(result, value);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Converts a model with string[] properties back to a model with Guid[] properties
        /// </summary>
        /// <typeparam name="T">The type of the model</typeparam>
        /// <param name="model">The model to convert</param>
        /// <returns>A new model with string[] properties converted back to Guid[]</returns>
        public static T ConvertStringArraysToGuidArrays<T>(T model) where T : class, new()
        {
            if (model == null)
            {
                return null;
            }
            
            // Create a new instance of the model
            var result = new T();
            
            // Get all properties of the model
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                
                if (value == null)
                {
                    continue;
                }
                
                // Check if the property is a string[] and ends with "AsStrings"
                if (property.PropertyType == typeof(string[]) && property.Name.EndsWith("AsStrings"))
                {
                    // Convert string[] to Guid[]
                    var stringArray = (string[])value;
                    var guidArray = stringArray.Select(s => Guid.Parse(s)).ToArray();
                    
                    // Get the original property name (without "AsStrings")
                    var originalPropertyName = property.Name.Substring(0, property.Name.Length - 9);
                    var originalProperty = typeof(T).GetProperty(originalPropertyName);
                    
                    if (originalProperty != null && originalProperty.PropertyType == typeof(Guid[]))
                    {
                        originalProperty.SetValue(result, guidArray);
                    }
                }
                else
                {
                    // Copy the property value as is
                    property.SetValue(result, value);
                }
            }
            
            return result;
        }
    }
}
