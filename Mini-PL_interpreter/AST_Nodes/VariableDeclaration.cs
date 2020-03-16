namespace MiniPLInterpreter
{
  class VariableDeclaration : AST
  {
    public Variable variableNode { get; set; }
    public Type typeNode { get; set; }
    public AST value { get; set; }

    public VariableDeclaration(Variable variableNode, Type typeNode, AST value = null)
    {
      this.variableNode = variableNode;
      this.typeNode = typeNode;
      this.value = value;
    }
  }
}