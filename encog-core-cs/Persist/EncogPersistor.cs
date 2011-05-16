using System;
using System.IO;

namespace Encog.Persist
{
    /// <summary>
    /// This interface defines an Encog Persistor. An Encog persistor will write an
    /// Encog object to an EG file.
    /// </summary>
    ///
    public interface EncogPersistor
    {
        /// <value>Get the class string for the object.</value>
        String PersistClassString { /// <returns>Get the class string for the object.</returns>
            get; }

        /// <value>Get the file version used by this persistor.</value>
        int FileVersion { /// <returns>Get the file version used by this persistor.</returns>
            get; }


        /// <summary>
        /// Read the object from an input stream.
        /// </summary>
        ///
        /// <param name="is">The input stream.</param>
        /// <returns>The object.</returns>
        Object Read(Stream mask0);

        /// <summary>
        /// Save the object.
        /// </summary>
        ///
        /// <param name="os">The output stream to save to.</param>
        /// <param name="obj">The object to save.</param>
        void Save(Stream os, Object obj);
    }
}