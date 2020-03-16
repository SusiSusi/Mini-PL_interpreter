class Numeric : AST
{
  public Token token { get; set; }
  public int tokenValue { get; set; }
  public Numeric(Token token)
  {
    this.token = token;
    this.tokenValue = (int)this.token.value;
  }
}