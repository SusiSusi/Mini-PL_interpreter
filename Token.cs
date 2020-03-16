enum TokenType
{
  // Operators
  MINUS,
  PLUS,
  MUL,
  DIV,
  EQUAL,
  LESS,
  AND,
  NOT,

  // Keywords
  VAR,
  FOR,
  END,
  IN,
  DO,
  READ,
  PRINT,
  INT,
  STRING,
  BOOL,
  ASSERT,

  // Marks
  LEFTBRACKET,
  RIGHTBRACKET,
  ASSIGN,
  SEMI,
  COLON,
  DOT,

  // Other
  INTEGER,
  EOF,
  ID
}

class Token
{
    public TokenType type { get; set; }
    public object value { get; set; }
    int? lineNo;
    int? columnNo;

    public Token(TokenType type, object value, int? lineNo = null, int? columnNo = null)
    {
      this.type = type;
      this.value = value;
      this.lineNo = lineNo;
      this.columnNo = columnNo;
    }

    public override string ToString() 
    {
      return "Token(" + this.type + ", " + this.value + ", position=" + this.lineNo 
      + ":" + this.columnNo + ")";
    }
}