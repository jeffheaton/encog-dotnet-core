using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Units;
using Encog.Parse.Recognize;
using log4net;
using Encog.Persist.Location;
using System.IO;
using Encog.Persist;

namespace Encog.Parse
{
    /// <summary>
    /// Encog's generic parser, used by the NLP package.
    /// </summary>
    public class Parse
    {
        private static UnitManager unitManager;
        private ParseTemplate template;
        public const String RESOURCE_NAME = "org/encog/data/template.eg";
        public const String RESOURCE_ITEM_NAME = "parse-native";

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FilePersistence));

        public Signal.Signal ParseFile(String name)
        {
            try
            {
                TextReader file = new StreamReader("date.txt");

                String contents = "";
                String line = "";

                while ((line = file.ReadLine()) != null)
                    contents += line + "\r\n";

                file.Close();
                return ParseString(contents);
            }
            catch (IOException e)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
                }
                throw new ParseError(e);
            }
        }

        public static UnitManager UnitManager
        {
            get
            {
                return Parse.unitManager;
            }
            set
            {
                unitManager = value;
            }
        }

        private bool ParseIteration(Signal.Signal input)
        {
            bool changed = false;

            if (this.template == null)
            {
                throw new ParseError("Must load a template before calling the Parse object.");
            }

            foreach (Recognize.Recognize recognize in template.Recognizers)
            {
                if (recognize.PerformRecognize(input))
                    changed = true;
            }
            return changed;
        }

        public Signal.Signal ParseString(String input)
        {
            Signal.Signal result = new Signal.Signal(input + " ");
            result.ResetDelta();

            do
            {
                result.ResetDelta();
                ParseIteration(result);
            }
            while (result.Delta);
            return result;
        }

        /// <summary>
        /// The template to use for parsing.
        /// </summary>
        public ParseTemplate Template
        {
            get
            {
                return template;
            }
        }

        public void Load()
        {

            ResourcePersistence location = new ResourcePersistence(Parse.RESOURCE_NAME);
            EncogPersistedCollection encog = new EncogPersistedCollection(location, FileMode.Open);
            this.template = (ParseTemplate)encog.Find(Parse.RESOURCE_ITEM_NAME);
        }

    }
}
