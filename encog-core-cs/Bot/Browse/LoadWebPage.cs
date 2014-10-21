//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.IO;
using System.Text;
using Encog.Bot.Browse.Range;
using Encog.Bot.DataUnits;
using Encog.Parse.Tags;
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
        private WebPage _page;

        /// <summary>
        /// The base URL for the page being loaded.
        /// </summary>
        private readonly Uri _baseURL;

        /// <summary>
        /// The last form that was processed.
        /// </summary>
        private Form _lastForm;

        /// <summary>
        /// The last hierarchy element that was processed.
        /// </summary>
        private DocumentRange _lastHierarchyElement;

        /// <summary>
        /// Construct a web page loader with the specified base URL.
        /// </summary>
        /// <param name="baseURL">The base URL to use when loading.</param>
        public LoadWebPage(Uri baseURL)
        {
            _baseURL = baseURL;
        }

        /// <summary>
        /// Add the specified hierarchy element.
        /// </summary>
        /// <param name="element">The hierarchy element to add.</param>
        private void AddHierarchyElement(DocumentRange element)
        {
            if (_lastHierarchyElement == null)
            {
                _page.AddContent(element);
            }
            else
            {
                _lastHierarchyElement.AddElement(element);
            }
            _lastHierarchyElement = element;
        }

        /// <summary>
        /// Create a dataunit to hode the code HTML tag.
        /// </summary>
        /// <param name="str">The code to create the data unit with.</param>
        private void CreateCodeDataUnit(String str)
        {
            if (str.Trim().Length > 0)
            {
                var d = new CodeDataUnit {Code = str};
                _page.AddDataUnit(d);
            }
        }

        /// <summary>
        /// Create a tag data unit.
        /// </summary>
        /// <param name="tag">The tag name to create the data unit for.</param>
        private void CreateTagDataUnit(Tag tag)
        {
            var d = new TagDataUnit {Tag = (Tag) tag.Clone()};

            _page.AddDataUnit(d);
        }

        /// <summary>
        /// Create a text data unit.
        /// </summary>
        /// <param name="str">The text.</param>
        private void CreateTextDataUnit(String str)
        {
            if (str.Trim().Length > 0)
            {
                var d = new TextDataUnit {Text = str};
                _page.AddDataUnit(d);
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

            while (count < _page.getDataSize())
            {
                DataUnit du = _page.GetDataUnit(count);

                if (du is TagDataUnit)
                {
                    Tag nextTag = ((TagDataUnit) du).Tag;
                    if (String.Compare(tag.Name, nextTag.Name, true) == 0)
                    {
                        if (nextTag.TagType == Tag.Type.End)
                        {
                            if (depth == 0)
                            {
                                return count;
                            }
                            depth--;
                        }
                        else if (nextTag.TagType == Tag.Type.Begin)
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
            _page = new WebPage();

            LoadDataUnits(istream);
            LoadContents();

            return _page;
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
                byte[] array = Encoding.UTF8.GetBytes(str);
                Stream bis = new MemoryStream(array);

                WebPage result = Load(bis);
                bis.Close();
                return result;
            }
            catch (IOException e)
            {
#if logging
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Exception", e);
                }
#endif
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
            for (int index = 0; index < _page.getDataSize(); index++)
            {
                DataUnit du = _page.GetDataUnit(index);
                if (du is TagDataUnit)
                {
                    Tag tag = ((TagDataUnit) du).Tag;

                    if (tag.TagType != Tag.Type.End)
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

                    if (tag.TagType == Tag.Type.Begin)
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

                    if (tag.TagType == Tag.Type.End)
                    {
                        if (string.Compare(tag.Name, "div") == 0)
                        {
                            if (_lastHierarchyElement != null)
                            {
                                _lastHierarchyElement =
                                    _lastHierarchyElement.Parent;
                            }
                        }
                        else if (String.Compare(tag.Name, "span", true) == 0)
                        {
                            if (_lastHierarchyElement != null)
                            {
                                _lastHierarchyElement =
                                    _lastHierarchyElement.Parent;
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
            var text = new StringBuilder();
            int ch;
            var parse = new ReadHTML(istream);
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
                    text.Append((char) ch);
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
            var div = new Div(_page);
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

            var form = new Form(_page);
            form.Begin = index;
            form.End = FindEndTag(index + 1, tag);

            if ((method == null) || string.Compare(method, "GET", true) == 0)
            {
                form.Method = Form.FormMethod.Get;
            }
            else
            {
                form.Method = Form.FormMethod.Post;
            }

            if (action == null)
            {
                form.Action = new Address(_baseURL);
            }
            else
            {
                form.Action = new Address(_baseURL, action);
            }

            _page.AddContent(form);
            _lastForm = form;
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

            var input = new Input(_page);
            input.Type = type;
            input.Name = name;
            input.Value = value;

            if (_lastForm != null)
            {
                _lastForm.AddElement(input);
            }
            else
            {
                _page.AddContent(input);
            }
        }

        /// <summary>
        /// Called by loadContents to load a link on the page.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadLink(int index, Tag tag)
        {
            var link = new Link(_page);
            String href = tag.GetAttributeValue("href");

            if (href != null)
            {
                link.Target = new Address(_baseURL, href);
                link.Begin = index;
                link.End = FindEndTag(index + 1, tag);
                _page.AddContent(link);
            }
        }

        /// <summary>
        /// Called by loadContents to load a span.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        private void LoadSpan(int index, Tag tag)
        {
            var span = new Span(_page);
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
            var title = new DocumentRange(_page);
            title.Begin = index;
            title.End = FindEndTag(index + 1, tag);
            _page.Title = title;
        }
    }
}
