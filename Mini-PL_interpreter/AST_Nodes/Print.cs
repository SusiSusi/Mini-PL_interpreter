namespace MiniPLInterpreter
{
  class Print : AST
  {
    public AST expr { get; set; } 

    public Print(AST expr)
    {
      this.expr = expr;
    }
  }
}