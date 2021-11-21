using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CS_Html
{
    public enum TagTypeEnum
    {
        Text = 1,
        Font = 2,
        Emphasis = 2,
        Strong = 3,
    }

    public class Tag
    {
        TagTypeEnum tagType;
        int StartTagBegin = -1;
        int StartTagEnd = -1;
        int EndTagBegin = -1;
        int EndTagEnd = -1;

        string Text = String.Empty;
        string Color = String.Empty;
        string FontName = string.Empty;
        string FontSize = string.Empty;
        bool Bold = false;
        bool Italic = false;
    }

    public enum SnippetTypeEnum
    {
        Text = 0,
        StartTag = 1,
        EndTag = 2
    }

    public class Snippet
    {
        public SnippetTypeEnum SnippetType = SnippetTypeEnum.Text;
        public int FirstChar = -1;
        public int LastChar = -1;
        public int Length { get { return (FirstChar != -1 && LastChar != -1) ? LastChar - FirstChar + 1 : 0;  } }
    } // class

    public class SnippetParser
    {
        public string Text;
        public List<Snippet> SnippetList;
        public string ErrorMessage;

        public bool Parse(string Text)
        {
            this.Text = Text;
            SnippetList = new List<Snippet>();
            ErrorMessage = string.Empty;
            int idx = 0;
            while (idx < Text.Length)
            {
                Snippet snippet = ParseSnippet(ref idx);
                if (snippet == null)
                {
                    return false;
                }
                SnippetList.Add(snippet);
            }
            return true;
        }

        private Snippet ParseSnippet(ref int idx)
        {
            char ch = Text[idx];
            Snippet snippet = new Snippet();
            snippet.FirstChar = idx;
            if (ch == '<')
            {
                if (++idx >= Text.Length)
                {
                    ErrorMessage = "Error: Unterminated start tag: Failed to find '>'.";
                    return null;
                }
                if (Text[idx] == '/')
                {
                    snippet.SnippetType = SnippetTypeEnum.EndTag;
                    if (++idx >= Text.Length)
                    {
                        ErrorMessage = "Error: Unterminated end tag: Failed to find '>'.";
                        return null;
                    }
                }
                else
                {
                    snippet.SnippetType = SnippetTypeEnum.StartTag;
                }
                int pos = Text.IndexOf('>', idx);
                if (pos < 0)
                {
                    ErrorMessage = "Error: Unterminated " + (snippet.SnippetType == SnippetTypeEnum.StartTag ? "start" : "end") + " tag: Failed to find '>'.";
                    return null;
                }
                snippet.LastChar = pos;
            }
            else
            {
                snippet.SnippetType = SnippetTypeEnum.Text;
                int pos = Text.IndexOf('<', idx);
                if (pos < 0)
                {
                    snippet.LastChar = Text.Length - 1;
                }
                else
                {
                    snippet.LastChar = pos - 1;
                }
            }
            idx = snippet.LastChar + 1;
            return snippet;
        }
    } // class

    class Program
    {
        static void Main(string[] args)
        {
            const string indent = "    ";
            SnippetParser parser = new SnippetParser();
            string[] tests =
            {
                "An example with<b>a tag and a <i>subtag</i> inside it</b> and some text after",
                "<b>a tag and a <i>subtag</i> inside it</b>",
                "<b>a tag and a <i>subtag</i> inside it</b",
                "<b>a tag and a <isubtag</i> inside it</b>",
            };

            foreach (string test in tests)
            {
                Debug.WriteLine("Parsing:");
                Debug.WriteLine(indent + '"' + test + '"');
                Debug.WriteLine("Result:");
                bool result = parser.Parse(test);5555555555555a
                if (result)
                {
                    foreach (Snippet snippet in parser.SnippetList)
                    {
                        string substring = parser.Text.Substring(snippet.FirstChar, snippet.Length);
                        Debug.WriteLine(indent + '"' + substring + '"');
                    }
                }
                else
                {
                    Debug.WriteLine(indent + parser.ErrorMessage);
                }
            }
        }
    }
}
