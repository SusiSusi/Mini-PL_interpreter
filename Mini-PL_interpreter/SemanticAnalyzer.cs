namespace MiniPLInterpreter
{
  class SemanticAnalyzer : NodeVisitor
  {
    public Parser parser { get; set; }
    public AST tree { get; set; }
    public SymbolTable symbolTable { get; set; }

    public SemanticAnalyzer(Parser parser)
    {
      this.parser = parser;
      this.symbolTable = new SymbolTable();
    }

    private void Error(string errorMessage, Token token)
    {
      string message = errorMessage + " -> " + token.ToString(); 
      throw new SemanticError(message);
    }

    public void VisitBinaryOperation(BinaryOperation node)
    {
      Visit(node.left);
      Visit(node.right);
    }

    public void VisitNumeric(Numeric node) { }

    public void VisitUnaryOperation(UnaryOperation node)
    {
      Visit(node.expr);
    }

    public void VisitStatementList(StatementList node)
    {
      foreach (AST child in node.children)
      {
        Visit(child);
      }
    }

    public void VisitNoOperation(NoOperation node) {}

    public void VisitVariableDeclaration(VariableDeclaration node)
    {
      object typeName = node.typeNode.value;
      Symbol typeSymbol = this.symbolTable.Lookup(typeName);
      string variableName = (string)node.variableNode.value;

      if (node.value != null)
      {
        Visit(node.value);
      }
      Symbol variableSymbol = new VariableSymbol(variableName, typeSymbol);

      if (this.symbolTable.Lookup(variableName) != null)
      {
        Error("Duplicate identifier found", node.variableNode.token);
      }
      this.symbolTable.Define(variableSymbol);
    }

    public void VisitAssignment(Assignment node)
    {
      object variableName = node.left.value;
      Symbol variableSymbol = this.symbolTable.Lookup(variableName);
      if (variableSymbol == null)
      {
        Error("Variable name not found", node.left.token);
      }
      Visit(node.right);
    }

    public void VisitVariable(Variable node)
    {
      object variableName = node.value;
      Symbol variableSymbol = this.symbolTable.Lookup(variableName);
      if (variableSymbol == null)
      {
        Error("Identifier not found", node.token);
      }
    }

    public void VisitStringAST(StringAST node) { }

    public void VisitBooleanAST(BooleanAST node) { }

    public void VisitFor(For node)
    {
      object variable = Visit(node.variable);
      object expr1 = Visit(node.expr1);
      object expr2 = Visit(node.expr2);

      Visit(node.list);
    }

    public void VisitAssert(Assert node)
    {
      object expr = Visit(node.expr);
    }

    public void VisitPrint(Print node) { }

    public void VisitRead(Read node)
    {
      object variableName = node.variable.value;
      Visit(node.variable);
    }

    public void Analyze()
    {
      this.tree = this.parser.Parse();
      if (this.tree == null)
      {
        throw new SemanticError("Parser tree is null!");
      }
      Visit(tree);
    }
  }
}