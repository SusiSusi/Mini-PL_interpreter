class BinaryOperation : AST
{
  public AST left { get; set; }
  public Token operat { get; set; }
  public AST right { get; set; }
  public BinaryOperation(AST left, Token operat, AST right)
  {
    this.left = left;
    this.operat = operat;
    this.right = right;
  }
}