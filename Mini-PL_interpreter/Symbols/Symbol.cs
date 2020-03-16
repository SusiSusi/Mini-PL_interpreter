namespace MiniPLInterpreter
{
  abstract class Symbol
  {
    public string name { get; set; }
    public object type { get; set; }

    public Symbol(string name, object type=null)
    {
      this.name = name;
      this.type = type;
    }
  }
}