using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using log4net;
using Encog.Persist.Location;
using Encog.Persist.Persistors;

namespace Encog.Parse.Recognize
{
    /// <summary>
    /// Allows templates to be specified for the parser.
    /// </summary>
    public class ParseTemplate : IEncogPersistedObject
    {

        private ICollection<Recognize> recognizers = new List<Recognize>();
        private String name;
        private String description;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FilePersistence));

        public void AddRecognizer(Recognize recognize)
        {
            recognizers.Add(recognize);
        }

        public Recognize CreateRecognizer(String name)
        {
            Recognize result = new Recognize(name);
            AddRecognizer(result);
            return result;
        }

        /// <summary>
        /// The recognizers to use.
        /// </summary>
        public ICollection<Recognize> Recognizers
        {
            get
            {
                return recognizers;
            }
        }

        public IPersistor CreatePersistor()
        {
            return new ParseTemplatePersistor();
        }

        /// <summary>
        /// The name.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }


        /// <summary>
        /// The description.
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


        public Object Clone()
        {
            ParseTemplate result = new ParseTemplate();
            result.Name = this.Name;
            result.Description = this.Description;
            return result;
        }
    }
}
