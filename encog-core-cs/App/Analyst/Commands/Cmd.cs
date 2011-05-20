using System;
using System.Text;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Prop;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// Base class for Encog Analyst commands. This class defines the properties sent
    /// to a command.
    /// </summary>
    ///
    public abstract class Cmd
    {
        /// <summary>
        /// The analyst object that this command belongs to.
        /// </summary>
        ///
        private readonly EncogAnalyst analyst;

        /// <summary>
        /// The properties to use with this command.
        /// </summary>
        ///
        private readonly ScriptProperties properties;

        /// <summary>
        /// The script object that this command belongs to.
        /// </summary>
        ///
        private readonly AnalystScript script;

        /// <summary>
        /// Construct this command.
        /// </summary>
        ///
        /// <param name="theAnalyst">The analyst that this command belongs to.</param>
        public Cmd(EncogAnalyst theAnalyst)
        {
            analyst = theAnalyst;
            script = analyst.Script;
            properties = script.Properties;
        }


        /// <value>The analyst used with this command.</value>
        public EncogAnalyst Analyst
        {
            get { return analyst; }
        }


        /// <value>The name of this command.</value>
        public abstract String Name { get; }


        /// <value>The properties used with this command.</value>
        public ScriptProperties Prop
        {
            get { return properties; }
        }


        /// <value>The script used with this command.</value>
        public AnalystScript Script
        {
            get { return script; }
        }

        /// <summary>
        /// Execute this command.
        /// </summary>
        ///
        /// <param name="args">The arguments for this command.</param>
        /// <returns>True if processing should stop after this command.</returns>
        public abstract bool ExecuteCommand(String args);


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(Name);
            result.Append("]");
            return result.ToString();
        }
    }
}