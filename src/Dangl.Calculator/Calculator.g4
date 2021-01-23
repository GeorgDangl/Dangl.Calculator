// **************************************
// *                                    *
// *  Grammar for calculating           *
// *  formulas.                         *
// *  � 2015 Georg Dangl                *
// *  info@georgdangl.de                *
// *                                    *
// **************************************    
// *                                    *
// *  Grammar written for ANTLR4        *
// *  www.antlr.org                     *
// **************************************
    
grammar Calculator;

@parser::members
{
    protected const int EOF = Eof;
}

@lexer::members
{
    protected const int EOF = Eof;
    protected const int HIDDEN = Hidden;
}

@header {
#pragma warning disable 0419
#pragma warning disable 3021
#pragma warning disable 1574
#pragma warning disable 1591
#pragma warning disable 1658
#pragma warning disable 1584
#pragma warning disable 0108
}

/*
 * Parser Rules
 */

// Main entry for the calculator
calculator : expression '='? trailingComment? compileUnit;

// Possible expression types
expression    :    SUB expression                              #Unary          // Unary minus sign (negative numbers)
              |    ADD expression                              #UnaryPlus      // Unary plus sign (positive numbers)
              |    FLOOR expression                            #Floor          // Round down to zero accuracy
              |    CEIL expression                             #Ceil           // Round up to zero accuracy
              |    ABS expression                              #Abs            // Absolute value
              |    ROUNDK '(' expression ';' expression ')'    #Roundk         // Round expr_1 with expr_2 accuracy
              |    ROUND expression                            #Round          // Round with zero accuracy
              |    TRUNC expression                            #Trunc          // Trim decimal digits
              |    SIN expression                              #Sin            // Sinus
              |    COS expression                              #Cos            // Cosinus
              |    TAN expression                              #Tan            // Tangens
              |    COT expression                              #Cot            // Cotangens    
              |    SINH expression                             #Sinh           // Sinus Hypererbolicus
              |    COSH expression                             #Cosh           // Cosinus Hyperbolicus
              |    TANH expression                             #Tanh           // Tangens Hyperbolicus
              |    ARCSIN expression                           #Arcsin         // Inverse Sinus
              |    ARCCOS expression                           #Arccos         // Inverse Cosinus
              |    ARCTAN expression                           #Arctan         // Inverse Tangens
              |    ARCTAN2 '(' expression ';' expression ')'   #Arctan2        // Atan2
              |    ARCCOT expression                           #Arccot         // Inverse Cotangens
              |    EXP expression                              #Exp            // e ^ expr
              |    LN expression                               #Ln             // Logarithm to e
              |    EEX expression                              #Eex            // 10 ^ expr
              |    LOG expression                              #Log            // Logarithm to 10
              |    RAD expression                              #Rad            // Angle to radians (360� base)
              |    DEG expression                              #Deg            // Radians to angle (360� base)
              |    SQRT expression                             #Sqrt           // Square root
              |    SQR expression                              #Sqr            // Square product
              |    expression EXPONENT expression              #Exponent       // Exponent, e.g. 10e+43
              |    expression NEGEXPONENT expression           #NegExponent    // Inverted Exponent, e.g. 10e-43
              |    expression op=('^'|'**') expression         #Pow            // expr_1 to the expr_2 th power
              |    expression (MOD | '%' ) expression          #Mod            // Modulo
              |    expression WHOLE expression                 #Whole          // Whole part of division rest
              |    expression op=('~'|'//') expression         #SqRoot         // expr_1 nth root of expr_2
              |    expression op=('*'|'/') expression          #MulDiv         // Multiplication or division
              |    '(' expression ')'                          #Parenthesis    // Expression within parentheses
              |    expression '(' expression ')'               #Mult           // Multiplication without sign
              |    '(' expression ')' expression               #Mult           // Multiplication without sign
              |    expression op=(ADD|SUB) expression          #AddSub         // Addition or subtraction
              |    NUMBER                                      #Number         // Single integer or float number
              |    PI '()'?                                    #Pi             // Mathematical constant pi = 3,141593
              |    EULER                                       #Euler          // Mathematical constant e = 2,718282
              |    SUBSTITUTION                                #Substitution
              ;

// End of file
trailingComment: SEMICOLON .*? ;
compileUnit    : EOF ;

/*
 * Lexer Rules
 */

NUMBER      : FLOAT
            | DIGIT+
            ;
FLOAT       : DIGIT+ (','|'.') DIGIT*
            | (','|'.') DIGIT+
            ;
DIGIT       : [0-9]                           ;
MOD         : [Mm][Oo][Dd]                    ;
WHOLE       : [Dd][Ii][Vv]                    ;
MUL         : '*'                             ;
DIV         : '/'                             ;
ADD         : '+'                             ;
SUB         : '-'                             ;
PI          : [Pp][Ii]                        ;
EXPONENT    : [Ee] '+'                        ;
NEGEXPONENT : [Ee] '-'                        ;
EULER       : [Ee]                            ;
SQRT        : [Ss][Qq][Rr][Tt]                ;
SQR         : [Ss][Qq][Rr]                    ;
FLOOR       : [Ff][Ll][Oo][Oo][Rr]            ;
CEIL        : [Cc][Ee][Ii][Ll]                ;
ABS         : [Aa][Bb][Ss]                    ;
ROUNDK      : [Rr][Oo][Uu][Nn][Dd][Kk]        ;
ROUND       : [Rr][Oo][Uu][Nn][Dd]            ;
TRUNC       : [Tt][Rr][Uu][Nn][Cc]            ;
SIN         : [Ss][Ii][Nn]                    ;
COS         : [Cc][Oo][Ss]                    ;
TAN         : [Tt][Aa][Nn]                    ;
COT         : [Cc][Oo][Tt]                    ;
SINH        : [Ss][Ii][Nn][Hh]                ;
COSH        : [Cc][Oo][Ss][Hh]                ;
TANH        : [Tt][Aa][Nn][Hh]                ;
ARCSIN      : [Aa][Rr][Cc][Ss][Ii][Nn]        ;
ARCCOS      : [Aa][Rr][Cc][Cc][Oo][Ss]        ;
ARCTAN      : [Aa][Rr][Cc][Tt][Aa][Nn]        ;
ARCTAN2     : [Aa][Rr][Cc][Tt][Aa][Nn][2]     ;
ARCCOT      : [Aa][Rr][Cc][Cc][Oo][Tt]        ;
EXP         : [Ee][Xx][Pp]                    ;
LN          : [Ll][Nn]                        ;
EEX         : [Ee][Ee][Xx]                    ;
LOG         : [Ll][Oo][Gg]                    ;
RAD         : [Rr][Aa][Dd]                    ;
DEG         : [Dd][Ee][Gg]                    ;
WS          : (' '|'\t'|'\r'|'\n') -> skip    ;
COM         : COMMENT              -> skip    ;
SUBSTITUTION: '#' ([a-z] | [A-Z] | [äÄöÖüÜ] | [0-9])+ ;
SEMICOLON   : ';'                             ;
INVALID     : .                               ;

fragment COMMENT    : '/*' .*? '*/'
                    | '\'' .*? '\''
                    | '"'.*?'"'
                    ;
