using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Data
{
    /// <summary>
    /// An Encog object that can hold text data. This object can be stored in an
    /// Encog persisted file.
    /// </summary>
    [Serializable]
    public class TextData : IEncogPersistedObject
    {
        /// <summary>
        /// The text data that is stored.
        /// </summary>
        private String text;

        /// <summary>
        /// The name of this object.
        /// </summary>
        private String name;

        /// <summary>
        /// The description of this object.
        /// </summary>
        private String description;

        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private readonly ILog logger = LogManager.GetLogger(typeof(TextData));

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A cloned version of this object.</returns>
        public Object Clone()
        {
            TextData result = new TextData();
            result.Name = this.Name;
            result.Description = this.Description;
            result.Text = this.Text;
            return result;
        }



        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// The description for this object.
        /// </summary>
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// The text that this object holds.
        /// </summary>
        public String Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// Create a persistor to store this object.
        /// </summary>
        /// <returns>A persistor.</returns>
        public IPersistor CreatePersistor()
        {
            return new TextDataPersistor();
        }

    }

}
