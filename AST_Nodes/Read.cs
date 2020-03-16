class Read : AST
{
  public Variable variable { get; set; }

  public Read(Variable variable)
  {
    this.variable = variable;
  }
}