using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Bot.Browse.Range;
using Encog.Parse.Tags;
using System.IO;
using Encog.Bot.DataUnits;
using Encog.Parse.Tags.Read;

namespace Encog.Bot.Browse
{
    /// <summary>
    /// Called to actually load a web page. This will read the HTML on a web page and
    /// generate the DocumentRange classes.
    /// </summary>
    public class LoadWebPage
    {

        /// <summary>
        /// The loaded webpage.
        /// </summary>
        private WebPage page;

        /// <summary>
        /// The base URL for the page being loaded.
        /// </summary>
        private Uri baseURL;

        /// <summary>
        /// The last form that was processed.
        /// </summary>
        private Form lastForm;

        /// <summary>
        /// The last hierarchy element that was processed.
        /// </summary>
        private DocumentRange lastHierarchyElement;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(LoadWebPage));

        /// <summary>
        /// Construct a web page loader with the specified base URL.
        /// </summary>
        /// <param name="baseURL">The base URL to use when loading.</param>
        public LoadWebPage(Uri baseURL)
        {
            this.baseURL = baseURL;
        }

        /// <summary>
        /// Add the specified hierarchy element.
        /// </summary>
        /// <param name="element">The hierarchy element to add.</param>
        private void AddHierarchyElement(DocumentRange element)
        {
            if (this.lastHierarchyElement == null)
            {
                this.page.AddContent(element);
            }
            else
            {
                this.lastHierarchyElement.AddElement(element);
            }
            this.lastHierarchyElement = element;
        }

        /// <summary>
        /// Create a dataunit to hode the code HTML tag.
        /// </summary>
        /// <param name="str">The code to create the data unit with.</param>
        private void CreateCodeDataUnit(String str)
        {
            if (str.Trim().Length > 0)
            {
                CodeDataUnit d = new CodeDataUnit();
                d.Code = str;
                this.page.AddDataUnit(d);
            }
        }

        /// <summary>
        /// Create a tag data unit.
        /// </summary>
        /// <param name="tag">The tag name to create the data unit for.</param>
        private void CreateTagDataUnit(Tag tag)
        {
            TagDataUnit d = new TagDataUnit();
            d.Tag = tag.Clone();

            this.page.AddDataUnit(d);
        }

        /// <summary>
        /// Create a text data unit.
        /// </summary>
        /// <param name="str">The text.</param>
        private void CreateTextDataUnit(String str)
        {
            if (str.Trim().Length > 0)
            {
                TextDataUnit d = new TextDataUnit();
                d.Text = str;
                this.page.AddDataUnit(d);
            }
        }

        /// <summary>
        ///  Find the end tag that lines up to the beginning tag.
        /// </summary>
        /// <param name="index">The index to start the search on. This specifies
        /// the starting data unit.</param>
        /// <param name="tag">The beginning tag that we are seeking the end tag 
        /// for.</param>
        /// <returns>The index that the ending tag was found at. Returns -1
        /// if not found.</returns>
        protected int FindEndTag(int index, Tag tag)
        {
            int depth = 0;
            int count = index;

            while (count < this.page.getDataSize())
            {
                DataUnit du = this.page.GetDataUnit(count);

                if (du is TagDataUnit)
                {
                    Tag nextTag = ((TagDataUnit)du).Tag;
                    if (String.Compare(tag.Name, nextTag.Name, true) == 0)
                    {
                        if (nextTag.TagType == Tag.Type.END)
                        {
                            if (depth == 0)
                            {
                                return count;
                            }
                            else
                            {
                                depth--;
                            }
                        }
                        else if (nextTag.TagType == Tag.Type.BEGIN)
                        {
                            depth++;
                        }
                    }
                }
                count++;
            }
            return -1;

        }

        /// <summary>
        /// Load a web page from the specified stream.
        /// </summary>
        /// <param name="istream">The input stream to load from.</param>
        /// <returns>The loaded web page.</returns>
        public WebPage Load(Stream istream)
        {
            this.page = new WebPage();

            LoadDataUnits(istream);
            LoadContents();

            return this.page;
        }

        /// <summary>
        /// Load the web page from a string that contains HTML.
        /// </summary>
        /// <param name="str">A string containing HTML.</param>
        /// <returns>The loaded WebPage.</returns>
        public WebPage Load(String str)
        {
            try
            {
                byte[] array = System.Text.Encoding.UTF8.GetBytes(str);
                Stream bis = new MemoryStream(array);

                WebPage result = Load(bis);
                bis.Close();
                return result;
            }
            catch (IOException e)
            {
                if (this.logger.IsDebugEnabled)
                {
                    this.logger.Debug("Exception", e);
                }
                throw new BrowseError(e);
            }
        }

        /// <summary>
        /// Using the data units, which should have already been loaded by this 
        /// time, load the contents of the web page.  This includes the title,
        /// any links and forms.  Div tags and spans are also processed.
        /// </summary>
        protected void LoadContents()
        {
            for (int index = 0; index < this.page.getDataSize(); index++)
            {
                DataUnit du = this.page.GetDataUnit(index);
                if (du is TagDataUnit)
                {
                    Tag tag = ((TagDataUnit)du).Tag;

                    if (tag.TagType != Tag.Type.END)
                    {
                        if (string.Compare(tag.Name, "a", true) == 0)
                        {
                            LoadLink(index, tag);
                        }
                        else if (string.Compare(tag.Name, "title", true) == 0)
                        {
                            LoadTitle(index, tag);
                        }
                        else if (string.Compare(tag.Name, "form", true) == 0)
                        {
                            LoadForm(index, tag);
                        }
                        else if (string.Compare(tag.Name, "input", true) == 0)
                        {
                            LoadInput(index, tag);
                        }

                    }

                    if (tag.TagType == Tag.Type.BEGIN)
                    {
                        if (String.Compare(tag.Name, "div", true) == 0)
                        {
                            LoadDiv(index, tag);
                        }
                        else if (String.Compare(tag.Name, "span", true) == 0)
                        {
                            LoadSpan(index, tag);
                        }
                    }

                    if (tag.TagType == Tag.Type.END)
                    {
                        if (string.Compare(tag.Name, "div") == 0)
                        {
                            if (this.lastHierarchyElement != null)
                            {
                                this.lastHierarchyElement =
                                    this.lastHierarchyElement.Parent;
                            }
                        }
                        else if (String.Compare(tag.Name, "span", true) == 0)
                        {
                            if (this.lastHierarchyElement != null)
                            {
                                this.lastHierarchyElement =
                                    this.lastHierarchyElement.Parent;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load the data units.  Once the lower level data units have been 
        /// loaded, the contents can be loaded.
        /// </summary>
        /// <param name="istream">The input stream that the data units are loaded from.</param>
        protected void LoadDataUnits(Stream istream)
        {
            StringBuilder text = new StringBuilder();
            int ch;
            ReadHTML parse = new ReadHTML(istream);
            bool style = false;
            bool script = false;

            while ((ch = parse.Read()) != -1)
            {
                if (ch == 0)
                {

                    if (style)
                    {
                        CreateCodeDataUnit(text.ToString());
                    }
                    else if (script)
                    {
                        CreateCodeDataUnit(text.ToString());
                    }
                    else
                    {
                        CreateTextDataUnit(text.ToString());
                    }
                    style = false;
                    script = false;

                    text.Length = 0;
                    CreateTagDataUnit(parse.LastTag);
                    if (String.Compare(parse.LastTag.Name, "style", true) == 0)
                    {
                        style = true;
                    }
                    else if (string.Compare(parse.LastTag.Name,
                            "script", true) == 0)
                    {
                        script = true;
                    }
                }
                else
                {
                    text.Append((char)ch);
                }
            }

            CreateTextDataUnit(text.ToString());
        }


        /// <summary>
        /// Called by loadContents to load a div tag.
        /// </summary>
        /// <param name="index">The index to begin at.</param>
        /// <param name="tag">The beginning div tag.</param>
        private void LoadDiv(int index, Tag tag)
        {
            Div div = new Div(this.page);
            String classAttribute = tag.GetAttributeValue("class");
            String idAttribute = tag.GetAttributeValue("id");

            div.IdAttribute = idAttribute;
            div.ClassAttribute = (classAttribute);
            div.Begin = index;
            div.End = FindEndTag(index + 1, tag);
            AddHierarchyElement(div);
        }

        /// <summary>
        /// Called by loadContents to load a form on the page.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadForm(int index, Tag tag)
        {
            String method = tag.GetAttributeValue("method");
            String action = tag.GetAttributeValue("action");

            Form form = new Form(this.page);
            form.Begin = index;
            form.End = FindEndTag(index + 1, tag);

            if ((method == null) || string.Compare(method, "GET", true) == 0)
            {
                form.Method = Form.FormMethod.GET;
            }
            else
            {
                form.Method = Form.FormMethod.POST;
            }

            if (action == null)
            {
                form.Action = new Address(this.baseURL);
            }
            else
            {
                form.Action = new Address(this.baseURL, action);
            }

            this.page.AddContent(form);
            this.lastForm = form;
        }

        /// <summary>
        /// Called by loadContents to load an input tag on the form.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadInput(int index, Tag tag)
        {
            String type = tag.GetAttributeValue("type");
            String name = tag.GetAttributeValue("name");
            String value = tag.GetAttributeValue("value");

            Input input = new Input(this.page);
            input.Type = type;
            input.Name = name;
            input.Value = value;

            if (this.lastForm != null)
            {
                this.lastForm.AddElement(input);
            }
            else
            {
                this.page.AddContent(input);
            }
        }

        /// <summary>
        /// Called by loadContents to load a link on the page.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadLink(int index, Tag tag)
        {
            Link link = new Link(this.page);
            String href = tag.GetAttributeValue("href");

            if (href != null)
            {
                link.setTarget(new Address(this.baseURL, href));
                link.Begin = index;
                link.End = FindEndTag(index + 1, tag);
                this.page.AddContent(link);
            }
        }

        /// <summary>
        /// Called by loadContents to load a span.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        private void LoadSpan(int index, Tag tag)
        {
            Span span = new Span(this.page);
            String classAttribute = tag.GetAttributeValue("class");
            String idAttribute = tag.GetAttributeValue("id");

            span.IdAttribute = idAttribute;
            span.ClassAttribute = classAttribute;
            span.Begin = index;
            span.End = FindEndTag(index + 1, tag);
            AddHierarchyElement(span);
        }

        /// <summary>
        /// Called by loadContents to load the title of the page.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadTitle(int index, Tag tag)
        {
            DocumentRange title = new DocumentRange(this.page);
            title.Begin = index;
            title.End = FindEndTag(index + 1, tag);
            this.page.Title = title;
        }

    }

}
