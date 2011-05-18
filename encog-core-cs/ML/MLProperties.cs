using System;
using System.Collections.Generic;

namespace Encog.ML
{
    /// <summary>
    /// Defines a Machine Learning Method that holds properties.
    /// </summary>
    ///
    public interface MLProperties : MLMethod
    {
        /// <value>A map of all properties.</value>
        IDictionary<String, String> Properties { /// <returns>A map of all properties.</returns>
            get; }


        /// <summary>
        /// Get the specified property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The property as a double.</returns>
        double GetPropertyDouble(String name);

        /// <summary>
        /// Get the specified property as a long.
        /// </summary>
        ///
        /// <param name="name">The name of the specified property.</param>
        /// <returns>The value of the specified property.</returns>
        long GetPropertyLong(String name);

        /// <summary>
        /// Get the specified property as a string.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        String GetPropertyString(String name);

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="d">The value of the property.</param>
        void SetProperty(String name, double d);

        /// <summary>
        /// Set a property as a long.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="l">The value of the property.</param>
        void SetProperty(String name, long l);

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="value_ren">The value of the property.</param>
        void SetProperty(String name, String value_ren);

        /// <summary>
        /// Update any objeccts when a property changes.
        /// </summary>
        ///
        void UpdateProperties();
    }
}