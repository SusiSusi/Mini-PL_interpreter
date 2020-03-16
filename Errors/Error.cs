using System;

abstract class Error : Exception
{
  public string message { get; set; }

  public Error(string message)
  {
    this.message = message;
  }

  public override string ToString()
  {
    return "\n" + this.message;
  }
}