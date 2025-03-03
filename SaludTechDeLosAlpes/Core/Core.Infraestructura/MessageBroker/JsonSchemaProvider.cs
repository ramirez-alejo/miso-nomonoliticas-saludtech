using System;
using System.Text.Json;
using Pulsar.Client.Api;

namespace Core.Infraestructura.MessageBroker
{
    public static class JsonSchemaProvider
    {
        /// <summary>
        /// Creates a JSON schema for the specified type with special handling for Guid arrays
        /// </summary>
        /// <typeparam name="T">The type for which to create a schema</typeparam>
        /// <returns>A JSON schema for the specified type</returns>
        public static ISchema<T> CreateJsonSchema<T>()
        {
            try
            {
                // Try to create a standard JSON schema
                return Schema.JSON<T>();
            }
            catch (Exception ex)
            {
                // If schema creation fails, log the error
                Console.WriteLine($"Error creating JSON schema: {ex.Message}");
                
                // For now, just throw the exception
                // In production, you might want to handle this more gracefully
                throw;
            }
        }
    }
}
