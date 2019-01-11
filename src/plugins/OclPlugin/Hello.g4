grammar Hello;

oclFile : ( 'package' packageName
oclExpressions
'endpackage'
)+;
packageName : pathName;
oclExpressions : ( constraint )*;
constraint : contextDeclaration ( Stereotype '@' NUMBER NAME? ':' oclExpression)+;
contextDeclaration : 'context' ( operationContext | classifierContext );
classifierContext : ( NAME ':' NAME ) | NAME;
operationContext : NAME '::' operationName '(' formalParameterList ')' ( ':' returnType )?;
Stereotype : ( 'pre' | 'post' | 'inv' );
operationName : NAME | '=' | '+' | '-' | '<' | '<=' | '>=' | '>' | '/' | '*' | '<>' | 'implies' | 'not' | 'or' | 'xor' | 'and';
formalParameterList : ( NAME ':' typeSpecifier (',' NAME ':' typeSpecifier )*)?;
typeSpecifier : simpleTypeSpecifier | collectionType;
collectionType : collectionKind '(' simpleTypeSpecifier ')';
oclExpression : ( letExpression )* expression;
returnType : typeSpecifier;
expression : implExpression;
letExpression : 'let' NAME ( '(' formalParameterList ')' )? ( ':' typeSpecifier )? '=' expression ';';
ifExpression : 'if' expression 'then' expression 'else' expression 'endif';
implExpression : orExpression ( 'implies' orExpression)*;
orExpression : andExpression ( ('or' | 'xor') andExpression)*;
andExpression : relationalExpression ('and' relationalExpression)*;
relationalExpression : additiveExpression (relationalOperator additiveExpression)?;
additiveExpression : multiplicativeExpression ( addOperator multiplicativeExpression)*;
multiplicativeExpression : unaryExpression ( multiplyOperator unaryExpression)*;
unaryExpression : ( unaryOperator postfixExpression) | postfixExpression;
postfixExpression : primaryExpression ( ('.' | '->')propertyCall )*;
primaryExpression : literalCollection | literal | propertyCall | '(' expression ')' | ifExpression;
propertyCallParameters : '(' ( declarator )? ( actualParameterList )? ')';
literal : NUMBER | enumLiteral | stringLiteral | booleanLiteral;
stringLiteral : '"' NAME '"' ;
enumLiteral : NAME '::' NAME ( '::' NAME )*;
simpleTypeSpecifier : pathName;
literalCollection : collectionKind '{' ( collectionItem (',' collectionItem )*)? '}';
collectionItem : expression ('..' expression )?;
propertyCall : pathName ('@' NUMBER)? ( timeExpression )? ( qualifiers )? ( propertyCallParameters )?;
qualifiers : '[' actualParameterList ']';
declarator : NAME ( ',' NAME )* ( ':' simpleTypeSpecifier )? ( ';' NAME ':' typeSpecifier '=' expression )? '|';
pathName : NAME ( '::' NAME )*;
timeExpression : '@' 'pre';
actualParameterList : expression (',' expression)*;
logicalOperator : 'and' | 'or' | 'xor' | 'implies';
collectionKind : 'Set' | 'Bag' | 'Sequence' | 'Collection';
relationalOperator : '=' | '>' | '<' | '>=' | '<=' | '<>';
addOperator : '+' | '-';
multiplyOperator : '*' | '/';
unaryOperator : '-' | 'not';
booleanLiteral : 'true' | 'false';
fragment LOWERCASE  : 'a'..'z' ;
fragment UPPERCASE  : 'A'..'Z' ;
fragment DIGITS  : '0'..'9' ;
NAME : (LOWERCASE | UPPERCASE | '_') ( LOWERCASE | UPPERCASE | DIGITS | '_' )* ;
NUMBER : DIGITS (DIGITS)* ( '.' DIGITS (DIGITS)* )?;
WS : [ \t\r\n]+ -> skip ;