using System;
using System.Collections.Generic;

namespace MiniPLInterpreter
{
  class Interpreter : NodeVisitor
  {
    public SemanticAnalyzer semanticAnalyzer { get; set; }

    public AST tree { get; set; }
    public Dictionary<object, object> globalMemory;
    public Interpreter(SemanticAnalyzer semanticAnalyzer)
    {
      this.semanticAnalyzer = semanticAnalyzer;
      this.tree = semanticAnalyzer.tree;
      this.globalMemory = new Dictionary<object, object>();
    }

    public object VisitBinaryOperation(BinaryOperation node)
    {
      TokenType opType = node.operat.type;
      object nodeLeft = Visit(node.left);
      object nodeRight = Visit(node.right);

      if (nodeLeft is int && nodeRight is int)
      {
        return IntBinaryOperations(opType, (int)nodeLeft, (int)nodeRight);
      }
      else if (nodeLeft is string && nodeRight is string)
      {
        return StringBinaryOperations(opType, (string)nodeLeft, (string)nodeRight);
      }
      else if (nodeLeft is bool && nodeRight is bool)
      {
        return BoolBinaryOperations(opType, (bool)nodeLeft, (bool)nodeRight);
      }
      throw new InterpreterError("VisitBinaryOperation error");
    }

    public int VisitNumeric(Numeric node)
    {
      return node.tokenValue;
    }

    public string VisitStringAST(StringAST node)
    {
      return node.tokenValue;
    }

    public bool VisitBooleanAST(BooleanAST node)
    {
      return node.tokenValue;
    }

    public object VisitUnaryOperation(UnaryOperation node)
    {
      TokenType opType = node.operat.type;
      if (opType == TokenType.PLUS)
      {
        return +(int)Visit(node.expr);
      }
      else if (opType == TokenType.MINUS)
      {
        return -(int)Visit(node.expr);
      }
      throw new InterpreterError("VisitUnaryOperation error");
    }

    public void VisitFor(For node)
    {
      int start = (int)Visit(node.expr1);
      int end = (int)Visit(node.expr2);

      for (int i = start; i <= end; i++)
      {
        this.globalMemory[node.variable.value] = i;
        Visit(node.list);
      }
    }

    public void VisitAssert(Assert node)
    {
      object response = Visit(node.expr);
      if (!(bool)response)
      {
        Console.WriteLine("Assertion fails: " + response.ToString().ToUpper());
      }
      else
      {
        Console.WriteLine(response.ToString().ToUpper());
      }
    }

    public void VisitStatementList(StatementList node)
    {
      foreach (AST child in node.children)
      {
        Visit(child);
      }
    }

    public void VisitNoOperation(NoOperation node) { }

    public void VisitAssignment(Assignment node)
    {
      object variableName = node.left.value;
      object variableValue = Visit(node.right);
      this.globalMemory[variableName] = variableValue;
    }
    
    public object VisitVariable(Variable node)
    {
      object variableName = node.value;
      if (this.globalMemory.ContainsKey(variableName))
      {
        return this.globalMemory[variableName];
      }
      throw new InterpreterError("VisitVariable error");
    }

    public void VisitVariableDeclaration(VariableDeclaration node) 
    { 
      object variableName = node.variableNode.value;
      object typeName = node.typeNode.value;
      if (node.value == null)
      {
        if ((string)typeName == TokenType.INT.ToString().ToLower())
        {
          this.globalMemory[variableName] = 0;
        }
        else if ((string)typeName == TokenType.STRING.ToString().ToLower())
        {
          this.globalMemory[variableName] = "";
        }
        else
        {
          this.globalMemory[variableName] = false;
        }
      }
      else
      {
        object variableValue = Visit(node.value);
        this.globalMemory[variableName] = variableValue;
      }
    }

    public void VisitType(Type node) { }

    public void VisitPrint(Print node)
    {
      Console.Write(Visit(node.expr));
    }

    public void VisitRead(Read node)
    {
      object variableName = node.variable.token.value;
      Symbol symbol = this.semanticAnalyzer.symbolTable.symbols[variableName];
      string variableType = symbol.type.ToString();

      string input = Console.ReadLine();

      if (variableType == TokenType.INT.ToString().ToLower())
      {
        try
        {
          int number = int.Parse(input);
          this.globalMemory[node.variable.value] = number;
        }
        catch (System.Exception)
        {
          throw new InterpreterError("Invalid input. Expected input value is a number.");
        } 
      }
      else
      {
        this.globalMemory[node.variable.value] = input;
      }
    }

    public void Interpret()
    {
      if (this.tree == null)
      {
        throw new InterpreterError("Can not interpret because the tree is null");
      }
      Visit(this.tree);
    }
    
    private object IntBinaryOperations(TokenType type, int nodeLeft, int nodeRight)
    {
      if (type == TokenType.PLUS)
      {
        return nodeLeft + nodeRight;
      }
      else if (type == TokenType.MINUS)
      {
        return nodeLeft - nodeRight;
      }
      else if (type == TokenType.MUL)
      {
        return nodeLeft * nodeRight;
      }
      else if (type == TokenType.DIV)
      {
        return nodeLeft / nodeRight;
      }
      else if (type == TokenType.EQUAL)
      {
        return nodeLeft == nodeRight;
      }
      else if (type == TokenType.LESS)
      {
        return nodeLeft < nodeRight;
      }
      throw new InterpreterError("IntBinaryOperations error");
    }

    private object StringBinaryOperations(TokenType type, string nodeLeft, string nodeRight)
    {
      if (type == TokenType.PLUS)
      {
        return nodeLeft + nodeRight;
      }
      else if (type == TokenType.EQUAL)
      {
        return nodeLeft.Equals(nodeRight);
      }
      else if (type == TokenType.LESS)
      {
        if (nodeLeft.CompareTo(nodeRight) == -1)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      throw new InterpreterError("StringBinaryOperations error");
    }

    private object BoolBinaryOperations(TokenType type, bool nodeLeft, bool nodeRight)
    {
      if (type == TokenType.AND)
      {
        return nodeLeft & nodeRight;
      }
      else if (type == TokenType.NOT)
      {
        return nodeLeft != nodeRight;
      }
      else if (type == TokenType.EQUAL)
      {
        return nodeLeft == nodeRight;
      }
      else if (type == TokenType.LESS)
      {
        if (nodeLeft.CompareTo(nodeRight) == -1)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      throw new InterpreterError("BoolBinaryOperations error"); 
    }
  }
}