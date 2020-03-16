class VariableSymbol : Symbol
{
  public VariableSymbol(string name, Symbol type) : base(name, type) { }

  public override string ToString()
  {
    return "<" + name + ":" + type + ">";
  }
}