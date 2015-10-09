blueprint xor;
	input a;
	input b;
	
	nor notA a;
	nor notB b;
	
	and aTrue a notB;
	and bTrue b notA;
	or onlyOneTrue aTrue bTrue;
	
	output result onlyOneTrue;
end_blueprint;

blueprint halfAdder;
	input a;
	input b;
	output sum;
	output carry;
	
	build xor xorGate;
	link xorGate.a a;
	link xorGate.b b;
	link sum xorGate.result;
	
	and carryCalc a b;
	link carry carryCalc;	
	
end_blueprint;

inputButton a;
inputButton b;
