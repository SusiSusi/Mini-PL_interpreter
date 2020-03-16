namespace MiniPLInterpreter
{
  class Variable : AST
  {
    public Token token { get; set; }
    public object value { get; set; }

    public Variable(Token token)
    {
      this.token = token;
      this.value = token.value;
    }
  }
}