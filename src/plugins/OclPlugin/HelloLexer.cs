//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Hello.g4 by ANTLR 4.7.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
public partial class HelloLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, T__19=20, T__20=21, T__21=22, T__22=23, T__23=24, 
		T__24=25, T__25=26, T__26=27, T__27=28, T__28=29, T__29=30, T__30=31, 
		T__31=32, T__32=33, T__33=34, T__34=35, T__35=36, T__36=37, T__37=38, 
		T__38=39, T__39=40, T__40=41, T__41=42, T__42=43, T__43=44, Stereotype=45, 
		NAME=46, NUMBER=47, WS=48;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "T__15", "T__16", 
		"T__17", "T__18", "T__19", "T__20", "T__21", "T__22", "T__23", "T__24", 
		"T__25", "T__26", "T__27", "T__28", "T__29", "T__30", "T__31", "T__32", 
		"T__33", "T__34", "T__35", "T__36", "T__37", "T__38", "T__39", "T__40", 
		"T__41", "T__42", "T__43", "Stereotype", "LOWERCASE", "UPPERCASE", "DIGITS", 
		"NAME", "NUMBER", "WS"
	};


	public HelloLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public HelloLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'package'", "'endpackage'", "'@'", "':'", "'context'", "'::'", 
		"'('", "')'", "'='", "'+'", "'-'", "'<'", "'<='", "'>='", "'>'", "'/'", 
		"'*'", "'<>'", "'implies'", "'not'", "'or'", "'xor'", "'and'", "','", 
		"'let'", "';'", "'if'", "'then'", "'else'", "'endif'", "'.'", "'->'", 
		"'\"'", "'{'", "'}'", "'..'", "'['", "']'", "'|'", "'pre'", "'Set'", "'Bag'", 
		"'Sequence'", "'Collection'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, "Stereotype", "NAME", 
		"NUMBER", "WS"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "Hello.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static HelloLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x32', '\x144', '\b', '\x1', '\x4', '\x2', '\t', '\x2', 
		'\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', 
		'\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', 
		'\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', 
		'\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', '\x10', '\x4', 
		'\x11', '\t', '\x11', '\x4', '\x12', '\t', '\x12', '\x4', '\x13', '\t', 
		'\x13', '\x4', '\x14', '\t', '\x14', '\x4', '\x15', '\t', '\x15', '\x4', 
		'\x16', '\t', '\x16', '\x4', '\x17', '\t', '\x17', '\x4', '\x18', '\t', 
		'\x18', '\x4', '\x19', '\t', '\x19', '\x4', '\x1A', '\t', '\x1A', '\x4', 
		'\x1B', '\t', '\x1B', '\x4', '\x1C', '\t', '\x1C', '\x4', '\x1D', '\t', 
		'\x1D', '\x4', '\x1E', '\t', '\x1E', '\x4', '\x1F', '\t', '\x1F', '\x4', 
		' ', '\t', ' ', '\x4', '!', '\t', '!', '\x4', '\"', '\t', '\"', '\x4', 
		'#', '\t', '#', '\x4', '$', '\t', '$', '\x4', '%', '\t', '%', '\x4', '&', 
		'\t', '&', '\x4', '\'', '\t', '\'', '\x4', '(', '\t', '(', '\x4', ')', 
		'\t', ')', '\x4', '*', '\t', '*', '\x4', '+', '\t', '+', '\x4', ',', '\t', 
		',', '\x4', '-', '\t', '-', '\x4', '.', '\t', '.', '\x4', '/', '\t', '/', 
		'\x4', '\x30', '\t', '\x30', '\x4', '\x31', '\t', '\x31', '\x4', '\x32', 
		'\t', '\x32', '\x4', '\x33', '\t', '\x33', '\x4', '\x34', '\t', '\x34', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x5', '\x3', '\x5', '\x3', '\x6', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\b', '\x3', 
		'\b', '\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', '\x3', '\v', 
		'\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\r', '\x3', '\r', '\x3', 
		'\xE', '\x3', '\xE', '\x3', '\xE', '\x3', '\xF', '\x3', '\xF', '\x3', 
		'\xF', '\x3', '\x10', '\x3', '\x10', '\x3', '\x11', '\x3', '\x11', '\x3', 
		'\x12', '\x3', '\x12', '\x3', '\x13', '\x3', '\x13', '\x3', '\x13', '\x3', 
		'\x14', '\x3', '\x14', '\x3', '\x14', '\x3', '\x14', '\x3', '\x14', '\x3', 
		'\x14', '\x3', '\x14', '\x3', '\x14', '\x3', '\x15', '\x3', '\x15', '\x3', 
		'\x15', '\x3', '\x15', '\x3', '\x16', '\x3', '\x16', '\x3', '\x16', '\x3', 
		'\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x18', '\x3', 
		'\x18', '\x3', '\x18', '\x3', '\x18', '\x3', '\x19', '\x3', '\x19', '\x3', 
		'\x1A', '\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1B', '\x3', 
		'\x1B', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1D', '\x3', 
		'\x1D', '\x3', '\x1D', '\x3', '\x1D', '\x3', '\x1D', '\x3', '\x1E', '\x3', 
		'\x1E', '\x3', '\x1E', '\x3', '\x1E', '\x3', '\x1E', '\x3', '\x1F', '\x3', 
		'\x1F', '\x3', '\x1F', '\x3', '\x1F', '\x3', '\x1F', '\x3', '\x1F', '\x3', 
		' ', '\x3', ' ', '\x3', '!', '\x3', '!', '\x3', '!', '\x3', '\"', '\x3', 
		'\"', '\x3', '#', '\x3', '#', '\x3', '$', '\x3', '$', '\x3', '%', '\x3', 
		'%', '\x3', '%', '\x3', '&', '\x3', '&', '\x3', '\'', '\x3', '\'', '\x3', 
		'(', '\x3', '(', '\x3', ')', '\x3', ')', '\x3', ')', '\x3', ')', '\x3', 
		'*', '\x3', '*', '\x3', '*', '\x3', '*', '\x3', '+', '\x3', '+', '\x3', 
		'+', '\x3', '+', '\x3', ',', '\x3', ',', '\x3', ',', '\x3', ',', '\x3', 
		',', '\x3', ',', '\x3', ',', '\x3', ',', '\x3', ',', '\x3', '-', '\x3', 
		'-', '\x3', '-', '\x3', '-', '\x3', '-', '\x3', '-', '\x3', '-', '\x3', 
		'-', '\x3', '-', '\x3', '-', '\x3', '-', '\x3', '.', '\x3', '.', '\x3', 
		'.', '\x3', '.', '\x3', '.', '\x3', '.', '\x3', '.', '\x3', '.', '\x3', 
		'.', '\x3', '.', '\x5', '.', '\x117', '\n', '.', '\x3', '/', '\x3', '/', 
		'\x3', '\x30', '\x3', '\x30', '\x3', '\x31', '\x3', '\x31', '\x3', '\x32', 
		'\x3', '\x32', '\x3', '\x32', '\x5', '\x32', '\x122', '\n', '\x32', '\x3', 
		'\x32', '\x3', '\x32', '\x3', '\x32', '\x3', '\x32', '\a', '\x32', '\x128', 
		'\n', '\x32', '\f', '\x32', '\xE', '\x32', '\x12B', '\v', '\x32', '\x3', 
		'\x33', '\x3', '\x33', '\a', '\x33', '\x12F', '\n', '\x33', '\f', '\x33', 
		'\xE', '\x33', '\x132', '\v', '\x33', '\x3', '\x33', '\x3', '\x33', '\x3', 
		'\x33', '\a', '\x33', '\x137', '\n', '\x33', '\f', '\x33', '\xE', '\x33', 
		'\x13A', '\v', '\x33', '\x5', '\x33', '\x13C', '\n', '\x33', '\x3', '\x34', 
		'\x6', '\x34', '\x13F', '\n', '\x34', '\r', '\x34', '\xE', '\x34', '\x140', 
		'\x3', '\x34', '\x3', '\x34', '\x2', '\x2', '\x35', '\x3', '\x3', '\x5', 
		'\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', '\xF', '\t', 
		'\x11', '\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', '\xE', 
		'\x1B', '\xF', '\x1D', '\x10', '\x1F', '\x11', '!', '\x12', '#', '\x13', 
		'%', '\x14', '\'', '\x15', ')', '\x16', '+', '\x17', '-', '\x18', '/', 
		'\x19', '\x31', '\x1A', '\x33', '\x1B', '\x35', '\x1C', '\x37', '\x1D', 
		'\x39', '\x1E', ';', '\x1F', '=', ' ', '?', '!', '\x41', '\"', '\x43', 
		'#', '\x45', '$', 'G', '%', 'I', '&', 'K', '\'', 'M', '(', 'O', ')', 'Q', 
		'*', 'S', '+', 'U', ',', 'W', '-', 'Y', '.', '[', '/', ']', '\x2', '_', 
		'\x2', '\x61', '\x2', '\x63', '\x30', '\x65', '\x31', 'g', '\x32', '\x3', 
		'\x2', '\x3', '\x5', '\x2', '\v', '\f', '\xF', '\xF', '\"', '\"', '\x2', 
		'\x14C', '\x2', '\x3', '\x3', '\x2', '\x2', '\x2', '\x2', '\x5', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', '\x2', '\x2', '\x2', '\t', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\r', '\x3', '\x2', '\x2', '\x2', '\x2', '\xF', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x11', '\x3', '\x2', '\x2', '\x2', '\x2', '\x13', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x15', '\x3', '\x2', '\x2', '\x2', '\x2', '\x17', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x19', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1D', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x1F', '\x3', '\x2', '\x2', '\x2', '\x2', '!', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '#', '\x3', '\x2', '\x2', '\x2', '\x2', '%', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\'', '\x3', '\x2', '\x2', '\x2', '\x2', 
		')', '\x3', '\x2', '\x2', '\x2', '\x2', '+', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '-', '\x3', '\x2', '\x2', '\x2', '\x2', '/', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x31', '\x3', '\x2', '\x2', '\x2', '\x2', '\x33', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x35', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x37', '\x3', '\x2', '\x2', '\x2', '\x2', '\x39', '\x3', '\x2', '\x2', 
		'\x2', '\x2', ';', '\x3', '\x2', '\x2', '\x2', '\x2', '=', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '?', '\x3', '\x2', '\x2', '\x2', '\x2', '\x41', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x43', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x45', '\x3', '\x2', '\x2', '\x2', '\x2', 'G', '\x3', '\x2', '\x2', '\x2', 
		'\x2', 'I', '\x3', '\x2', '\x2', '\x2', '\x2', 'K', '\x3', '\x2', '\x2', 
		'\x2', '\x2', 'M', '\x3', '\x2', '\x2', '\x2', '\x2', 'O', '\x3', '\x2', 
		'\x2', '\x2', '\x2', 'Q', '\x3', '\x2', '\x2', '\x2', '\x2', 'S', '\x3', 
		'\x2', '\x2', '\x2', '\x2', 'U', '\x3', '\x2', '\x2', '\x2', '\x2', 'W', 
		'\x3', '\x2', '\x2', '\x2', '\x2', 'Y', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'[', '\x3', '\x2', '\x2', '\x2', '\x2', '\x63', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x65', '\x3', '\x2', '\x2', '\x2', '\x2', 'g', '\x3', '\x2', '\x2', 
		'\x2', '\x3', 'i', '\x3', '\x2', '\x2', '\x2', '\x5', 'q', '\x3', '\x2', 
		'\x2', '\x2', '\a', '|', '\x3', '\x2', '\x2', '\x2', '\t', '~', '\x3', 
		'\x2', '\x2', '\x2', '\v', '\x80', '\x3', '\x2', '\x2', '\x2', '\r', '\x88', 
		'\x3', '\x2', '\x2', '\x2', '\xF', '\x8B', '\x3', '\x2', '\x2', '\x2', 
		'\x11', '\x8D', '\x3', '\x2', '\x2', '\x2', '\x13', '\x8F', '\x3', '\x2', 
		'\x2', '\x2', '\x15', '\x91', '\x3', '\x2', '\x2', '\x2', '\x17', '\x93', 
		'\x3', '\x2', '\x2', '\x2', '\x19', '\x95', '\x3', '\x2', '\x2', '\x2', 
		'\x1B', '\x97', '\x3', '\x2', '\x2', '\x2', '\x1D', '\x9A', '\x3', '\x2', 
		'\x2', '\x2', '\x1F', '\x9D', '\x3', '\x2', '\x2', '\x2', '!', '\x9F', 
		'\x3', '\x2', '\x2', '\x2', '#', '\xA1', '\x3', '\x2', '\x2', '\x2', '%', 
		'\xA3', '\x3', '\x2', '\x2', '\x2', '\'', '\xA6', '\x3', '\x2', '\x2', 
		'\x2', ')', '\xAE', '\x3', '\x2', '\x2', '\x2', '+', '\xB2', '\x3', '\x2', 
		'\x2', '\x2', '-', '\xB5', '\x3', '\x2', '\x2', '\x2', '/', '\xB9', '\x3', 
		'\x2', '\x2', '\x2', '\x31', '\xBD', '\x3', '\x2', '\x2', '\x2', '\x33', 
		'\xBF', '\x3', '\x2', '\x2', '\x2', '\x35', '\xC3', '\x3', '\x2', '\x2', 
		'\x2', '\x37', '\xC5', '\x3', '\x2', '\x2', '\x2', '\x39', '\xC8', '\x3', 
		'\x2', '\x2', '\x2', ';', '\xCD', '\x3', '\x2', '\x2', '\x2', '=', '\xD2', 
		'\x3', '\x2', '\x2', '\x2', '?', '\xD8', '\x3', '\x2', '\x2', '\x2', '\x41', 
		'\xDA', '\x3', '\x2', '\x2', '\x2', '\x43', '\xDD', '\x3', '\x2', '\x2', 
		'\x2', '\x45', '\xDF', '\x3', '\x2', '\x2', '\x2', 'G', '\xE1', '\x3', 
		'\x2', '\x2', '\x2', 'I', '\xE3', '\x3', '\x2', '\x2', '\x2', 'K', '\xE6', 
		'\x3', '\x2', '\x2', '\x2', 'M', '\xE8', '\x3', '\x2', '\x2', '\x2', 'O', 
		'\xEA', '\x3', '\x2', '\x2', '\x2', 'Q', '\xEC', '\x3', '\x2', '\x2', 
		'\x2', 'S', '\xF0', '\x3', '\x2', '\x2', '\x2', 'U', '\xF4', '\x3', '\x2', 
		'\x2', '\x2', 'W', '\xF8', '\x3', '\x2', '\x2', '\x2', 'Y', '\x101', '\x3', 
		'\x2', '\x2', '\x2', '[', '\x116', '\x3', '\x2', '\x2', '\x2', ']', '\x118', 
		'\x3', '\x2', '\x2', '\x2', '_', '\x11A', '\x3', '\x2', '\x2', '\x2', 
		'\x61', '\x11C', '\x3', '\x2', '\x2', '\x2', '\x63', '\x121', '\x3', '\x2', 
		'\x2', '\x2', '\x65', '\x12C', '\x3', '\x2', '\x2', '\x2', 'g', '\x13E', 
		'\x3', '\x2', '\x2', '\x2', 'i', 'j', '\a', 'r', '\x2', '\x2', 'j', 'k', 
		'\a', '\x63', '\x2', '\x2', 'k', 'l', '\a', '\x65', '\x2', '\x2', 'l', 
		'm', '\a', 'm', '\x2', '\x2', 'm', 'n', '\a', '\x63', '\x2', '\x2', 'n', 
		'o', '\a', 'i', '\x2', '\x2', 'o', 'p', '\a', 'g', '\x2', '\x2', 'p', 
		'\x4', '\x3', '\x2', '\x2', '\x2', 'q', 'r', '\a', 'g', '\x2', '\x2', 
		'r', 's', '\a', 'p', '\x2', '\x2', 's', 't', '\a', '\x66', '\x2', '\x2', 
		't', 'u', '\a', 'r', '\x2', '\x2', 'u', 'v', '\a', '\x63', '\x2', '\x2', 
		'v', 'w', '\a', '\x65', '\x2', '\x2', 'w', 'x', '\a', 'm', '\x2', '\x2', 
		'x', 'y', '\a', '\x63', '\x2', '\x2', 'y', 'z', '\a', 'i', '\x2', '\x2', 
		'z', '{', '\a', 'g', '\x2', '\x2', '{', '\x6', '\x3', '\x2', '\x2', '\x2', 
		'|', '}', '\a', '\x42', '\x2', '\x2', '}', '\b', '\x3', '\x2', '\x2', 
		'\x2', '~', '\x7F', '\a', '<', '\x2', '\x2', '\x7F', '\n', '\x3', '\x2', 
		'\x2', '\x2', '\x80', '\x81', '\a', '\x65', '\x2', '\x2', '\x81', '\x82', 
		'\a', 'q', '\x2', '\x2', '\x82', '\x83', '\a', 'p', '\x2', '\x2', '\x83', 
		'\x84', '\a', 'v', '\x2', '\x2', '\x84', '\x85', '\a', 'g', '\x2', '\x2', 
		'\x85', '\x86', '\a', 'z', '\x2', '\x2', '\x86', '\x87', '\a', 'v', '\x2', 
		'\x2', '\x87', '\f', '\x3', '\x2', '\x2', '\x2', '\x88', '\x89', '\a', 
		'<', '\x2', '\x2', '\x89', '\x8A', '\a', '<', '\x2', '\x2', '\x8A', '\xE', 
		'\x3', '\x2', '\x2', '\x2', '\x8B', '\x8C', '\a', '*', '\x2', '\x2', '\x8C', 
		'\x10', '\x3', '\x2', '\x2', '\x2', '\x8D', '\x8E', '\a', '+', '\x2', 
		'\x2', '\x8E', '\x12', '\x3', '\x2', '\x2', '\x2', '\x8F', '\x90', '\a', 
		'?', '\x2', '\x2', '\x90', '\x14', '\x3', '\x2', '\x2', '\x2', '\x91', 
		'\x92', '\a', '-', '\x2', '\x2', '\x92', '\x16', '\x3', '\x2', '\x2', 
		'\x2', '\x93', '\x94', '\a', '/', '\x2', '\x2', '\x94', '\x18', '\x3', 
		'\x2', '\x2', '\x2', '\x95', '\x96', '\a', '>', '\x2', '\x2', '\x96', 
		'\x1A', '\x3', '\x2', '\x2', '\x2', '\x97', '\x98', '\a', '>', '\x2', 
		'\x2', '\x98', '\x99', '\a', '?', '\x2', '\x2', '\x99', '\x1C', '\x3', 
		'\x2', '\x2', '\x2', '\x9A', '\x9B', '\a', '@', '\x2', '\x2', '\x9B', 
		'\x9C', '\a', '?', '\x2', '\x2', '\x9C', '\x1E', '\x3', '\x2', '\x2', 
		'\x2', '\x9D', '\x9E', '\a', '@', '\x2', '\x2', '\x9E', ' ', '\x3', '\x2', 
		'\x2', '\x2', '\x9F', '\xA0', '\a', '\x31', '\x2', '\x2', '\xA0', '\"', 
		'\x3', '\x2', '\x2', '\x2', '\xA1', '\xA2', '\a', ',', '\x2', '\x2', '\xA2', 
		'$', '\x3', '\x2', '\x2', '\x2', '\xA3', '\xA4', '\a', '>', '\x2', '\x2', 
		'\xA4', '\xA5', '\a', '@', '\x2', '\x2', '\xA5', '&', '\x3', '\x2', '\x2', 
		'\x2', '\xA6', '\xA7', '\a', 'k', '\x2', '\x2', '\xA7', '\xA8', '\a', 
		'o', '\x2', '\x2', '\xA8', '\xA9', '\a', 'r', '\x2', '\x2', '\xA9', '\xAA', 
		'\a', 'n', '\x2', '\x2', '\xAA', '\xAB', '\a', 'k', '\x2', '\x2', '\xAB', 
		'\xAC', '\a', 'g', '\x2', '\x2', '\xAC', '\xAD', '\a', 'u', '\x2', '\x2', 
		'\xAD', '(', '\x3', '\x2', '\x2', '\x2', '\xAE', '\xAF', '\a', 'p', '\x2', 
		'\x2', '\xAF', '\xB0', '\a', 'q', '\x2', '\x2', '\xB0', '\xB1', '\a', 
		'v', '\x2', '\x2', '\xB1', '*', '\x3', '\x2', '\x2', '\x2', '\xB2', '\xB3', 
		'\a', 'q', '\x2', '\x2', '\xB3', '\xB4', '\a', 't', '\x2', '\x2', '\xB4', 
		',', '\x3', '\x2', '\x2', '\x2', '\xB5', '\xB6', '\a', 'z', '\x2', '\x2', 
		'\xB6', '\xB7', '\a', 'q', '\x2', '\x2', '\xB7', '\xB8', '\a', 't', '\x2', 
		'\x2', '\xB8', '.', '\x3', '\x2', '\x2', '\x2', '\xB9', '\xBA', '\a', 
		'\x63', '\x2', '\x2', '\xBA', '\xBB', '\a', 'p', '\x2', '\x2', '\xBB', 
		'\xBC', '\a', '\x66', '\x2', '\x2', '\xBC', '\x30', '\x3', '\x2', '\x2', 
		'\x2', '\xBD', '\xBE', '\a', '.', '\x2', '\x2', '\xBE', '\x32', '\x3', 
		'\x2', '\x2', '\x2', '\xBF', '\xC0', '\a', 'n', '\x2', '\x2', '\xC0', 
		'\xC1', '\a', 'g', '\x2', '\x2', '\xC1', '\xC2', '\a', 'v', '\x2', '\x2', 
		'\xC2', '\x34', '\x3', '\x2', '\x2', '\x2', '\xC3', '\xC4', '\a', '=', 
		'\x2', '\x2', '\xC4', '\x36', '\x3', '\x2', '\x2', '\x2', '\xC5', '\xC6', 
		'\a', 'k', '\x2', '\x2', '\xC6', '\xC7', '\a', 'h', '\x2', '\x2', '\xC7', 
		'\x38', '\x3', '\x2', '\x2', '\x2', '\xC8', '\xC9', '\a', 'v', '\x2', 
		'\x2', '\xC9', '\xCA', '\a', 'j', '\x2', '\x2', '\xCA', '\xCB', '\a', 
		'g', '\x2', '\x2', '\xCB', '\xCC', '\a', 'p', '\x2', '\x2', '\xCC', ':', 
		'\x3', '\x2', '\x2', '\x2', '\xCD', '\xCE', '\a', 'g', '\x2', '\x2', '\xCE', 
		'\xCF', '\a', 'n', '\x2', '\x2', '\xCF', '\xD0', '\a', 'u', '\x2', '\x2', 
		'\xD0', '\xD1', '\a', 'g', '\x2', '\x2', '\xD1', '<', '\x3', '\x2', '\x2', 
		'\x2', '\xD2', '\xD3', '\a', 'g', '\x2', '\x2', '\xD3', '\xD4', '\a', 
		'p', '\x2', '\x2', '\xD4', '\xD5', '\a', '\x66', '\x2', '\x2', '\xD5', 
		'\xD6', '\a', 'k', '\x2', '\x2', '\xD6', '\xD7', '\a', 'h', '\x2', '\x2', 
		'\xD7', '>', '\x3', '\x2', '\x2', '\x2', '\xD8', '\xD9', '\a', '\x30', 
		'\x2', '\x2', '\xD9', '@', '\x3', '\x2', '\x2', '\x2', '\xDA', '\xDB', 
		'\a', '/', '\x2', '\x2', '\xDB', '\xDC', '\a', '@', '\x2', '\x2', '\xDC', 
		'\x42', '\x3', '\x2', '\x2', '\x2', '\xDD', '\xDE', '\a', '$', '\x2', 
		'\x2', '\xDE', '\x44', '\x3', '\x2', '\x2', '\x2', '\xDF', '\xE0', '\a', 
		'}', '\x2', '\x2', '\xE0', '\x46', '\x3', '\x2', '\x2', '\x2', '\xE1', 
		'\xE2', '\a', '\x7F', '\x2', '\x2', '\xE2', 'H', '\x3', '\x2', '\x2', 
		'\x2', '\xE3', '\xE4', '\a', '\x30', '\x2', '\x2', '\xE4', '\xE5', '\a', 
		'\x30', '\x2', '\x2', '\xE5', 'J', '\x3', '\x2', '\x2', '\x2', '\xE6', 
		'\xE7', '\a', ']', '\x2', '\x2', '\xE7', 'L', '\x3', '\x2', '\x2', '\x2', 
		'\xE8', '\xE9', '\a', '_', '\x2', '\x2', '\xE9', 'N', '\x3', '\x2', '\x2', 
		'\x2', '\xEA', '\xEB', '\a', '~', '\x2', '\x2', '\xEB', 'P', '\x3', '\x2', 
		'\x2', '\x2', '\xEC', '\xED', '\a', 'r', '\x2', '\x2', '\xED', '\xEE', 
		'\a', 't', '\x2', '\x2', '\xEE', '\xEF', '\a', 'g', '\x2', '\x2', '\xEF', 
		'R', '\x3', '\x2', '\x2', '\x2', '\xF0', '\xF1', '\a', 'U', '\x2', '\x2', 
		'\xF1', '\xF2', '\a', 'g', '\x2', '\x2', '\xF2', '\xF3', '\a', 'v', '\x2', 
		'\x2', '\xF3', 'T', '\x3', '\x2', '\x2', '\x2', '\xF4', '\xF5', '\a', 
		'\x44', '\x2', '\x2', '\xF5', '\xF6', '\a', '\x63', '\x2', '\x2', '\xF6', 
		'\xF7', '\a', 'i', '\x2', '\x2', '\xF7', 'V', '\x3', '\x2', '\x2', '\x2', 
		'\xF8', '\xF9', '\a', 'U', '\x2', '\x2', '\xF9', '\xFA', '\a', 'g', '\x2', 
		'\x2', '\xFA', '\xFB', '\a', 's', '\x2', '\x2', '\xFB', '\xFC', '\a', 
		'w', '\x2', '\x2', '\xFC', '\xFD', '\a', 'g', '\x2', '\x2', '\xFD', '\xFE', 
		'\a', 'p', '\x2', '\x2', '\xFE', '\xFF', '\a', '\x65', '\x2', '\x2', '\xFF', 
		'\x100', '\a', 'g', '\x2', '\x2', '\x100', 'X', '\x3', '\x2', '\x2', '\x2', 
		'\x101', '\x102', '\a', '\x45', '\x2', '\x2', '\x102', '\x103', '\a', 
		'q', '\x2', '\x2', '\x103', '\x104', '\a', 'n', '\x2', '\x2', '\x104', 
		'\x105', '\a', 'n', '\x2', '\x2', '\x105', '\x106', '\a', 'g', '\x2', 
		'\x2', '\x106', '\x107', '\a', '\x65', '\x2', '\x2', '\x107', '\x108', 
		'\a', 'v', '\x2', '\x2', '\x108', '\x109', '\a', 'k', '\x2', '\x2', '\x109', 
		'\x10A', '\a', 'q', '\x2', '\x2', '\x10A', '\x10B', '\a', 'p', '\x2', 
		'\x2', '\x10B', 'Z', '\x3', '\x2', '\x2', '\x2', '\x10C', '\x10D', '\a', 
		'r', '\x2', '\x2', '\x10D', '\x10E', '\a', 't', '\x2', '\x2', '\x10E', 
		'\x117', '\a', 'g', '\x2', '\x2', '\x10F', '\x110', '\a', 'r', '\x2', 
		'\x2', '\x110', '\x111', '\a', 'q', '\x2', '\x2', '\x111', '\x112', '\a', 
		'u', '\x2', '\x2', '\x112', '\x117', '\a', 'v', '\x2', '\x2', '\x113', 
		'\x114', '\a', 'k', '\x2', '\x2', '\x114', '\x115', '\a', 'p', '\x2', 
		'\x2', '\x115', '\x117', '\a', 'x', '\x2', '\x2', '\x116', '\x10C', '\x3', 
		'\x2', '\x2', '\x2', '\x116', '\x10F', '\x3', '\x2', '\x2', '\x2', '\x116', 
		'\x113', '\x3', '\x2', '\x2', '\x2', '\x117', '\\', '\x3', '\x2', '\x2', 
		'\x2', '\x118', '\x119', '\x4', '\x63', '|', '\x2', '\x119', '^', '\x3', 
		'\x2', '\x2', '\x2', '\x11A', '\x11B', '\x4', '\x43', '\\', '\x2', '\x11B', 
		'`', '\x3', '\x2', '\x2', '\x2', '\x11C', '\x11D', '\x4', '\x32', ';', 
		'\x2', '\x11D', '\x62', '\x3', '\x2', '\x2', '\x2', '\x11E', '\x122', 
		'\x5', ']', '/', '\x2', '\x11F', '\x122', '\x5', '_', '\x30', '\x2', '\x120', 
		'\x122', '\a', '\x61', '\x2', '\x2', '\x121', '\x11E', '\x3', '\x2', '\x2', 
		'\x2', '\x121', '\x11F', '\x3', '\x2', '\x2', '\x2', '\x121', '\x120', 
		'\x3', '\x2', '\x2', '\x2', '\x122', '\x129', '\x3', '\x2', '\x2', '\x2', 
		'\x123', '\x128', '\x5', ']', '/', '\x2', '\x124', '\x128', '\x5', '_', 
		'\x30', '\x2', '\x125', '\x128', '\x5', '\x61', '\x31', '\x2', '\x126', 
		'\x128', '\a', '\x61', '\x2', '\x2', '\x127', '\x123', '\x3', '\x2', '\x2', 
		'\x2', '\x127', '\x124', '\x3', '\x2', '\x2', '\x2', '\x127', '\x125', 
		'\x3', '\x2', '\x2', '\x2', '\x127', '\x126', '\x3', '\x2', '\x2', '\x2', 
		'\x128', '\x12B', '\x3', '\x2', '\x2', '\x2', '\x129', '\x127', '\x3', 
		'\x2', '\x2', '\x2', '\x129', '\x12A', '\x3', '\x2', '\x2', '\x2', '\x12A', 
		'\x64', '\x3', '\x2', '\x2', '\x2', '\x12B', '\x129', '\x3', '\x2', '\x2', 
		'\x2', '\x12C', '\x130', '\x5', '\x61', '\x31', '\x2', '\x12D', '\x12F', 
		'\x5', '\x61', '\x31', '\x2', '\x12E', '\x12D', '\x3', '\x2', '\x2', '\x2', 
		'\x12F', '\x132', '\x3', '\x2', '\x2', '\x2', '\x130', '\x12E', '\x3', 
		'\x2', '\x2', '\x2', '\x130', '\x131', '\x3', '\x2', '\x2', '\x2', '\x131', 
		'\x13B', '\x3', '\x2', '\x2', '\x2', '\x132', '\x130', '\x3', '\x2', '\x2', 
		'\x2', '\x133', '\x134', '\a', '\x30', '\x2', '\x2', '\x134', '\x138', 
		'\x5', '\x61', '\x31', '\x2', '\x135', '\x137', '\x5', '\x61', '\x31', 
		'\x2', '\x136', '\x135', '\x3', '\x2', '\x2', '\x2', '\x137', '\x13A', 
		'\x3', '\x2', '\x2', '\x2', '\x138', '\x136', '\x3', '\x2', '\x2', '\x2', 
		'\x138', '\x139', '\x3', '\x2', '\x2', '\x2', '\x139', '\x13C', '\x3', 
		'\x2', '\x2', '\x2', '\x13A', '\x138', '\x3', '\x2', '\x2', '\x2', '\x13B', 
		'\x133', '\x3', '\x2', '\x2', '\x2', '\x13B', '\x13C', '\x3', '\x2', '\x2', 
		'\x2', '\x13C', '\x66', '\x3', '\x2', '\x2', '\x2', '\x13D', '\x13F', 
		'\t', '\x2', '\x2', '\x2', '\x13E', '\x13D', '\x3', '\x2', '\x2', '\x2', 
		'\x13F', '\x140', '\x3', '\x2', '\x2', '\x2', '\x140', '\x13E', '\x3', 
		'\x2', '\x2', '\x2', '\x140', '\x141', '\x3', '\x2', '\x2', '\x2', '\x141', 
		'\x142', '\x3', '\x2', '\x2', '\x2', '\x142', '\x143', '\b', '\x34', '\x2', 
		'\x2', '\x143', 'h', '\x3', '\x2', '\x2', '\x2', '\v', '\x2', '\x116', 
		'\x121', '\x127', '\x129', '\x130', '\x138', '\x13B', '\x140', '\x3', 
		'\b', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}