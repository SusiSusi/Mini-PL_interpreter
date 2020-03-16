using System.Collections.Generic;

namespace MiniPLInterpreter
{
  class Parser
  {
    public Lexer lexer { get; set; }
    public Token currentToken { get; set; }

    public Parser(Lexer lexer)
    {
      this.lexer = lexer;
      this.currentToken = this.lexer.GetNextToken();
    }

    private void Error(string errorMessage, Token token) 
    {
      string message = errorMessage + " -> " + token.ToString();
      throw new ParserError(message);
    }

    // <program> ::= <statement_list>
    private AST Program()
    {
      List<AST> nodes = StatementList();
      StatementList statementList = new StatementList();
      foreach (AST node in nodes)
      {
        statementList.children.Add(node);
      }
      return statementList;
    }

    // <statement_list> ::= ( <statement> ";" )*
    private List<AST> StatementList()
    {
      List<AST> statements = new List<AST>();
      AST node = Statement();
      statements.Add(node);

      while (this.currentToken.type == TokenType.SEMI)
      {
        Eat(TokenType.SEMI);
        statements.Add(Statement());
      }
      
      if (this.currentToken.type == TokenType.ID)
      {
        Error("Token type can not be ID", this.currentToken);
      }
      return statements;
    }

    /* 
    <statement> ::= "var" "ID" ":" <type_spec> | "var" "ID" ":" <type_spec> ":=" <expr>
                  | "ID" ":=" <expr> 
                  | "for" "ID" "in" <expr> ".." <expr> "do" <statement_list> "end" "for"
                  | "read" "ID" | "print" <expr> | "assert" "(" <expr> ")" | e
    */
    private AST Statement()
    {
      if (this.currentToken.type == TokenType.VAR)
      {
        return VariableDeclaration();
      }
      else if (this.currentToken.type == TokenType.ID)
      {
        return AssigmentStatement();
      }
      else if (this.currentToken.type == TokenType.FOR)
      {
        return For();
      }
      else if (this.currentToken.type == TokenType.READ)
      {
        return Read();
      }
      else if (this.currentToken.type == TokenType.PRINT)
      {
        return Print();
      }
      else if (this.currentToken.type == TokenType.ASSERT)
      {
        return Assert();
      }
      return Empty();
    }

    // <statement> ::= "ID" ":=" <expr>
    private Assignment AssigmentStatement()
    {
      Variable left = Variable();
      Token token = this.currentToken;
      Eat(TokenType.ASSIGN);
      AST right = Expr();
      Assignment node = new Assignment(left, token, right);
      return node;
    }
    
    // Variable : ID
    private Variable Variable()
    { 
      Variable node = new Variable(this.currentToken);
      Eat(TokenType.ID);
      return node;
    }

    // <statement> ::= e 
    private NoOperation Empty()
    {
      return new NoOperation();
    }

    /* 
    <statement> ::= "var" "ID" ":" <type_spec> 
                |  "var" "ID" ":" <type_spec> ":=" <expr> 
    */
    private VariableDeclaration VariableDeclaration()
    {
      Eat(TokenType.VAR);
      Variable variable = new Variable(this.currentToken);
      Eat(TokenType.ID);
      Eat(TokenType.COLON);
      Type typeNode = TypeSpec();
      if (this.currentToken.type == TokenType.SEMI)
      {
        return new VariableDeclaration(variable, typeNode);
      }
      else
      {
        Eat(TokenType.ASSIGN);
        VariableDeclaration varDec = new VariableDeclaration(variable, typeNode, Expr());
        return varDec; 
      }
    }

    // <type_spec> ::= "int" | "string" | "bool"
    private Type TypeSpec()
    {
      Token token = this.currentToken;
      if (this.currentToken.type == TokenType.INT)
      {
        Eat(TokenType.INT);
      }
      else if (this.currentToken.type == TokenType.STRING)
      {
        Eat(TokenType.STRING);
      }
      else if (this.currentToken.type == TokenType.BOOL)
      {
        Eat(TokenType.BOOL);
      }
      else
      {
        Error("Invalid token type", this.currentToken);
      } 
      return new Type(token);
    }

    // <statement> ::= "for" "ID" "in" <expr> ".." <expr> "do" <statement_list> "end" "for"
    private For For()
    {
      Token token = this.currentToken;
      Eat(TokenType.FOR);
      Variable variable = Variable();
      Eat(TokenType.IN);
      AST expr1 = Expr();
      Eat(TokenType.DOT);
      Eat(TokenType.DOT);
      AST expr2 = Expr();
      Eat(TokenType.DO);

      StatementList list = new StatementList();
      foreach (AST node in StatementList())
      {
        list.children.Add(node);
      }
      Eat(TokenType.END);
      Eat(TokenType.FOR);
      
      return new For(list, variable, expr1, expr2);
    }

    // <statement> ::= "assert" "(" <expr> ")"
    private Assert Assert()
    {
      Token token = this.currentToken;
      Eat(TokenType.ASSERT);
      Eat(TokenType.LEFTBRACKET);
      AST expr = Expr();
      Eat(TokenType.RIGHTBRACKET);
      return new Assert(expr);
    }

    // <statement> ::= "print" <expr>
    private Print Print()
    {
      Eat(TokenType.PRINT);
      return new Print(Expr());
    }

    // <statement> ::= "read" "ID"
    private Read Read()
    {
      Token token = this.currentToken;
      Eat(TokenType.READ);
      Variable variable = Variable();
      return new Read(variable);
    }

    // Compares the current token type and the given token type. If they match,
    // "eat" the current token and place the next token in the this.currentToken
    private void Eat(TokenType tokenType)
    {
      if (this.currentToken.type == tokenType) 
      {
        this.currentToken = this.lexer.GetNextToken();
      }
      else 
      {
        Error("Unexpected token", this.currentToken);
      }
    }

    /* 
    <factor> ::= ("PLUS" | "MINUS") <factor> | "INTEGER" | "STRING" 
              |  "BOOL" | "(" <expr> ")" | "ID"
    */
    private AST Factor()
    {
      Token token = this.currentToken;
      if (token.type == TokenType.PLUS)
      {
        Eat(TokenType.PLUS);
        return new UnaryOperation(token, Factor());
      }
      else if (token.type == TokenType.MINUS)
      {
        Eat(TokenType.MINUS);
        return new UnaryOperation(token, Factor());
      } 
      else if (token.type == TokenType.INTEGER)
      {
        Eat(TokenType.INTEGER);
        return new Numeric(token);
      }
      else if (token.type == TokenType.STRING)
      {
        Eat(TokenType.STRING);
        return new StringAST(token);
      }
      else if (token.type == TokenType.BOOL)
      {
        Eat(TokenType.BOOL);
        return new BooleanAST(token);
      }
      else if (token.type == TokenType.LEFTBRACKET)
      {
        Eat(TokenType.LEFTBRACKET);
        AST node = Expr();
        Eat(TokenType.RIGHTBRACKET);
        return node;
      }
      else
      {
        Variable node = Variable();
        return node;
      }
    }

    // <term> ::= <factor> ( ("MUL" | "DIV") <factor> )*
    private AST Term()
    {
      AST node = Factor();
      while (this.currentToken.type == TokenType.MUL ||
            this.currentToken.type == TokenType.DIV)
      {
        Token token = this.currentToken;
        if (token.type == TokenType.MUL)
        {
          Eat(TokenType.MUL);
        }
        else if (token.type == TokenType.DIV)
        {
          Eat(TokenType.DIV);
        }
        node = new BinaryOperation(node, token, Factor());
      }
      return node;
    }

    // <expr> ::= <term> ( ("PLUS" | "MINUS" | "EQUAL" | "LESS" | "AND" | "NOT") <term> )*
    private AST Expr()
    {
      AST node = Term();
      while (this.currentToken.type == TokenType.PLUS || 
            this.currentToken.type == TokenType.MINUS ||
            this.currentToken.type == TokenType.EQUAL ||
            this.currentToken.type == TokenType.LESS ||
            this.currentToken.type == TokenType.AND ||
            this.currentToken.type == TokenType.NOT)
      {
        Token token = this.currentToken;
        if (token.type == TokenType.PLUS)
        {
          Eat(TokenType.PLUS);
        }
        else if (token.type == TokenType.MINUS)
        {
          Eat(TokenType.MINUS);
        }
        else if (token.type == TokenType.EQUAL)
        {
          Eat(TokenType.EQUAL);
        }
        else if (token.type == TokenType.LESS)
        {
          Eat(TokenType.LESS);
        }
        else if (token.type == TokenType.AND)
        {
          Eat(TokenType.AND);
        }
        else if (token.type == TokenType.NOT)
        {
          Eat(TokenType.NOT);
        }
        node = new BinaryOperation(node, token, Term());
      }
      return node;
    }

    public AST Parse()
    {
      AST node = Program();
      if (this.currentToken.type != TokenType.EOF)
      {
        Error("Expected EOF token type but token type is", this.currentToken);
      } 
      return node;
    }
  }
}