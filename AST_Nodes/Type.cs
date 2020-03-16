class Type : AST
{
  public Token token { get; set; }
  public object value { get; set; }

  public Type(Token token) 
  {
    this.token = token;
    this.value = token.value;
  }
}