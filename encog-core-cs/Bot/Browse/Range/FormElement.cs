using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Range
{
    /**
 * A document range that represents one individual component to a form.
 * 
 * @author jheaton
 * 
 */
    public abstract class FormElement : DocumentRange
    {

        /**
         * The name of this form element.
         */
        private String name;

        /**
         * The value held by this form element.
         */
        private String value;

        /**
         * The owner of this form element.
         */
        private Form owner;



        /**
         * Construct a form element from the specified web page.
         * @param source The page that holds this form element.
         */
        public FormElement(WebPage source)
            : base(source)
        {
        }

        /**
         * @return The name of this form.
         */
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

        /**
         * @return The owner of this form element.
         */
        public Form Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        /**
         * @return The value of this form element.
         */
        public String Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }

        }

        /**
         * @return True if this is autosend, which means that the type is 
         * NOT submit.  This prevents a form that has multiple submit buttons
         * from sending ALL of them in a single post.
         */
        public abstract bool AutoSend
        {
            get;
        }

    }

}
