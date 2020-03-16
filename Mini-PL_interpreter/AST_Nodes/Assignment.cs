namespace MiniPLInterpreter
{
  class Assignment : AST
  {
    public Variable left { get; set; }
    public Token operat { get; set; }
    public AST right { get; set; }

    public Assignment(Variable left, Token operat, AST right)
    {
      this.left = left;
      this.operat = operat;
      this.right = right;
    }
  }
}