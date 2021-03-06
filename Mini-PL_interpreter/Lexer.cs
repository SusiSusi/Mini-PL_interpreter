using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPLInterpreter
{
  class Lexer
  {
    string text;
    int pos;
    char currentChar;
    int lineNo;
    int columnNo;
    Dictionary<string, Token> reservedKeywords;
    Dictionary<char, char> characterEscapes;
    Dictionary<char, TokenType> tokenTypes;
    
    public Lexer(string text)
    {
      this.text = text; // string input
      this.pos = 0; // index into this.text
      this.currentChar = this.text[this.pos];
      // token line number and column number
      this.lineNo = 1;
      this.columnNo = 1;  

      ListSettings settings = new ListSettings();
      this.reservedKeywords = settings.reservedKeywords;
      this.characterEscapes = settings.characterEscapes;
      this.tokenTypes = settings.tokenTypes;
    }
    
    private void Error() 
    {
      string message = "Invalid character: " + this.currentChar + ", position=" + this.lineNo 
      + ":" + this.columnNo;
      throw new LexerError(message); 
    }

    // Takes care of identifiers and reserved keywords
    private Token SetId() 
    {
      string result = "";
      int line = 0;
      int column = 0;
      
      while (this.currentChar != '\0' && Char.IsLetterOrDigit(this.currentChar))
      {
        result += this.currentChar;
        line = this.lineNo;
        column = this.columnNo;
        ReadNextChar();
      }
      if (this.reservedKeywords.ContainsKey(result))
      {
        return this.reservedKeywords[result];
      }
      return new Token(TokenType.ID, result, line, column);
    }

    // Moves the this.pos pointer and sets the this.currentChar
    private void ReadNextChar()
    {
      if (this.currentChar == '\n')
      {
        this.lineNo += 1;
        this.columnNo = 0;
      }

      this.pos += 1;
      if (this.pos > this.text.Length - 1)
      {
        this.currentChar = '\0'; // end of input
      }
      else 
      {
        this.currentChar = this.text[this.pos];
        this.columnNo += 1;
      }
    }

    // Returns the next character from the this.text without adding this.pos value
    private char Peek()
    {
      int peekPos = this.pos + 1;
      if (peekPos > this.text.Length - 1)
      {
        return '\0';
      }
      else
      {
        return this.text[peekPos];
      }
    }
    private void SkipWhitespace()
    {
      while (this.currentChar != '\0' && Char.IsWhiteSpace(this.currentChar))
      {
        ReadNextChar();
      }
    }

    private void SkipCommentOneLine()
    {
      while (this.currentChar != '\0' && this.currentChar != '\n')
      {
        ReadNextChar();
      }
      ReadNextChar();
    }

    private void SkipCommentMultipleLines()
    {
      int nested = 1;
      while (this.currentChar != '\0' && nested > 0)
      {
        if (this.currentChar == '*' && Peek() == '/')
        {
          ReadNextChar();
          ReadNextChar();
          nested -= 1;
        }
        if (this.currentChar == '/' && Peek() == '*')
        {
          ReadNextChar();
          ReadNextChar();
          nested += 1;
        }
        ReadNextChar();
      }

      if (nested > 0)
      {
        throw new LexerError("Missing comment closure: [" + this.lineNo + ":" + this.columnNo + "]");
      }
    }

    // Returns a possible multi-digit integer from input, such as 88
    private int Integer()
    {
      string result = "";
      while (this.currentChar != '\0' && Char.IsDigit(this.currentChar))
      {
        result += this.currentChar;
        ReadNextChar();
      }
      return int.Parse(result);
    }

    // Returns a string literals from input
    private Token StringLiteralToken()
    {
      StringBuilder builder = new StringBuilder();
      while (this.currentChar != '"')
      {
        if (this.currentChar == '\0')
        {
          throw new LexerError("End of input while scanning a string literal");
        } 
        if (this.currentChar == '\\')
        {
          if (this.characterEscapes.ContainsKey(Peek()))
          {
            builder.Append(this.characterEscapes[Peek()]);
            ReadNextChar();
            ReadNextChar();
          } 
          else
          {
            Error();
          }
          continue;
        }
        builder.Append(this.currentChar);
        ReadNextChar();
      }
      ReadNextChar();
      return new Token(TokenType.STRING, builder.ToString(), this.lineNo, this.columnNo);
    }

    // Responsible for breaking a sentence into tokens, one token at a time
    public Token GetNextToken() 
    {
      while (this.currentChar != '\0')
      {
        if (Char.IsWhiteSpace(this.currentChar))
        {
          SkipWhitespace();
          continue;
        }

        if (this.currentChar == '/' && Peek() == '/')
        {
          ReadNextChar();
          ReadNextChar();
          SkipCommentOneLine();
          continue;
        }

        if (this.currentChar == '/' && Peek() == '*')
        {
          ReadNextChar();
          ReadNextChar();
          SkipCommentMultipleLines();
          continue;
        }

        if (this.currentChar == '"')
        {
          ReadNextChar();
          return StringLiteralToken();
        }

        if (this.currentChar == ':' && Peek() == '=')
        {
          Token token = new Token(TokenType.ASSIGN, ":=", this.lineNo, this.columnNo);
          ReadNextChar();
          ReadNextChar();
          return token;
        }

        if (Char.IsLetter(this.currentChar))
        {
          return SetId();
        }

        if (Char.IsDigit(this.currentChar))
        {
          return new Token(TokenType.INTEGER, Integer(), this.lineNo, this.columnNo);
        }

        // single-character token
        object tokenType = null;
        object tokenTypeValue = null;

        foreach (KeyValuePair<char, TokenType> kvp in this.tokenTypes)
        {
          if (kvp.Key == this.currentChar)
          {
            tokenType = kvp.Value;
            tokenTypeValue = kvp.Key;
            break;
          }
        }

        if (tokenType != null && tokenTypeValue != null)
        {
          Token token = new Token((TokenType)tokenType, tokenTypeValue, this.lineNo, this.columnNo);
          ReadNextChar();
          return token;
        }
        else
        {
          Error();
        }
      }
      // no more input left, the last token is EOF
      return new Token(TokenType.EOF, '\0', this.lineNo, this.columnNo);
    }
  }
}