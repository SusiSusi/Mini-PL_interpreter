class For : AST
{
  public StatementList list { get; set; }
  public Variable variable { get; set; }
  public AST expr1 { get; set; }
  public AST expr2 { get; set; }

  public For(StatementList list, Variable variable, AST expr1, AST expr2)
  {
    this.list = list;
    this.variable = variable; 
    this.expr1 = expr1;
    this.expr2 = expr2;
  }

}