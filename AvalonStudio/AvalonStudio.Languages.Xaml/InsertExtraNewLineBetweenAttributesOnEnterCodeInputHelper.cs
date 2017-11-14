﻿using Avalonia.Ide.CompletionEngine;
using Avalonia.Input;
using AvalonStudio.Editor;
using AvalonStudio.Extensibility.Documents;
using System;

namespace AvalonStudio.Languages.Xaml
{
    class InsertExtraNewLineBetweenAttributesOnEnterCodeInputHelper : ICodeEditorInputHelper
    {
        public void AfterTextInput(ILanguageService languageServivce, IEditor editor, TextInputEventArgs args)
        {

        }

        public void BeforeTextInput(ILanguageService languageService, IEditor editor, TextInputEventArgs args)
        {
            if (args.Text == "\n")
            {
                //Check if we are not inside a tag
                var textBefore = editor.Document.GetText(0, Math.Max(0, editor.Offset));

                var state = XmlParser.Parse(textBefore);
                if (state.State == XmlParser.ParserState.None)
                {
                    //Find latest tag end
                    var idx = textBefore.LastIndexOf('>');

                    if (idx != -1)
                    {
                        state = XmlParser.Parse(editor.Document.GetText(0, Math.Max(0, idx)));

                        if ((state.State == XmlParser.ParserState.StartElement || state.State == XmlParser.ParserState.AfterAttributeValue) && editor.Document.Text[editor.Offset] == '<')
                        {
                            var newline = "\n";

                            bool indentEnd = false;

                            if (editor.Line < editor.Document.LineCount)
                            {
                                indentEnd = true;
                                editor.Document.TrimTrailingWhiteSpace(editor.Line + 1);
                            }

                            editor.Document.Insert(editor.Offset, newline);

                            editor.Document.TrimTrailingWhiteSpace(editor.Line);

                            editor.Offset -= newline.Length;

                            editor.IndentLine(editor.Line);

                            if (indentEnd)
                            {
                                editor.IndentLine(editor.Line + 1);
                            }
                        }
                    }
                }
            }
        }
    }
}
