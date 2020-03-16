using System.Collections.Generic;

namespace MiniPLInterpreter
{
  class StatementList : AST
  {
    public List<AST> children { get; set; }
    public StatementList()
    {
      this.children = new List<AST>();
    }
  }
}