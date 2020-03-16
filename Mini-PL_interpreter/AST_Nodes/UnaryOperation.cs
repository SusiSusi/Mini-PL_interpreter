namespace MiniPLInterpreter
{
  class UnaryOperation : AST
  {
    public Token operat { get; set; }
    public AST expr { get; set; }
    public UnaryOperation(Token operat, AST expr)
    {
      this.operat = operat;
      this.expr = expr;
    }
  }
}