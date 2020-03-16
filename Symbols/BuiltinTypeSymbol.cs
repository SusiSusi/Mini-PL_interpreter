class BuiltinTypeSymbol : Symbol
{
  public BuiltinTypeSymbol(string name) : base(name) { }

  public override string ToString()
  {
    return name;
  }
}