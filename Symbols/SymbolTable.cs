using System.Collections.Generic;
using System;
using System.Linq;

class SymbolTable
{
  public Dictionary<object, Symbol> symbols { get; set; }

  public SymbolTable() 
  {
    this.symbols = new Dictionary<object, Symbol>();
    InitBuiltins();
  }

  private void InitBuiltins()
  {
    Define(new BuiltinTypeSymbol("int"));
    Define(new BuiltinTypeSymbol("string"));
    Define(new BuiltinTypeSymbol("bool"));
  }

  public void Define(Symbol symbol)
  {
    this.symbols[symbol.name] = symbol;
  }

  public Symbol Lookup(object name)
  {
    if (this.symbols.ContainsKey(name))
    {
      return this.symbols[name];
    }
    return null;
  }

  public override string ToString()
  {
    return string.Join(Environment.NewLine, this.symbols.Select(kvp => kvp.Value.ToString()));
  }
}