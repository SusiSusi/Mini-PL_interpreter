using System.IO;

namespace MiniPLInterpreter
{
  class Program
  {
    static void Main(string[] args)
    {
      string codeExamples = File.ReadAllText("CodeExamples.txt");
      Lexer lexer = new Lexer(codeExamples);
      Parser parser = new Parser(lexer);
            
      SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(parser);
      semanticAnalyzer.Analyze();
      Interpreter interpreter = new Interpreter(semanticAnalyzer);
      interpreter.Interpret(); 
    }
  }
}
