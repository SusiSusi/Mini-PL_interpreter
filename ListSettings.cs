using System.Collections.Generic;

class ListSettings
{
  public Dictionary<string, Token> reservedKeywords { get; set; }
  public Dictionary<char, char> characterEscapes { get; set; }
  public Dictionary<char, TokenType> tokenTypes { get; set; }

  public ListSettings() {
    this.reservedKeywords = new Dictionary<string, Token>();
    this.characterEscapes = new Dictionary<char, char>();
    this.tokenTypes = new Dictionary<char, TokenType>();
    SetReserverdKeywords();
    SetCharacterEscapes();
    SetTokenTypes();
  }

  private void SetReserverdKeywords()
  {
    this.reservedKeywords.Add("var", new Token(TokenType.VAR, "var"));
    this.reservedKeywords.Add("for", new Token(TokenType.FOR, "for"));
    this.reservedKeywords.Add("end", new Token(TokenType.END, "end"));
    this.reservedKeywords.Add("in", new Token(TokenType.IN, "in"));
    this.reservedKeywords.Add("do", new Token(TokenType.DO, "do"));
    this.reservedKeywords.Add("read", new Token(TokenType.READ, "read"));
    this.reservedKeywords.Add("print", new Token(TokenType.PRINT, "print"));
    this.reservedKeywords.Add("int", new Token(TokenType.INT, "int"));
    this.reservedKeywords.Add("string", new Token(TokenType.STRING, "string"));
    this.reservedKeywords.Add("bool", new Token(TokenType.BOOL, "bool"));
    this.reservedKeywords.Add("assert", new Token(TokenType.ASSERT, "assert"));
  }

  private void SetCharacterEscapes()
  {
    this.characterEscapes.Add('\\', '\\'); // backslash
    this.characterEscapes.Add('a', '\a'); // alert
    this.characterEscapes.Add('b', '\b'); // backspace
    this.characterEscapes.Add('f', '\f'); // form feed
    this.characterEscapes.Add('n', '\n'); // new line
    this.characterEscapes.Add('r', '\r'); // carriage return
    this.characterEscapes.Add('t', '\t'); // horizontal tab
    this.characterEscapes.Add('v', '\v'); // vertical quote
  }

  private void SetTokenTypes()
  {
    this.tokenTypes.Add('-', TokenType.MINUS);
    this.tokenTypes.Add('+', TokenType.PLUS);
    this.tokenTypes.Add('*', TokenType.MUL);
    this.tokenTypes.Add('/', TokenType.DIV);
    this.tokenTypes.Add('=', TokenType.EQUAL);
    this.tokenTypes.Add('<', TokenType.LESS);
    this.tokenTypes.Add('&', TokenType.AND);
    this.tokenTypes.Add('!', TokenType.NOT);
    this.tokenTypes.Add('(', TokenType.LEFTBRACKET);
    this.tokenTypes.Add(')', TokenType.RIGHTBRACKET);
    this.tokenTypes.Add(';', TokenType.SEMI);
    this.tokenTypes.Add(':', TokenType.COLON);
    this.tokenTypes.Add('.', TokenType.DOT);
  }
}