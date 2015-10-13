blueprint xor;
	
	input a;
	input b;
	
	nor notA a;
	nor notB b;
	
	and aTrue a notB;
	and bTrue b notA;
	
	output result aTrue bTrue;
	
end_blueprint;

blueprint xnor;

	input a;
	input b;
	
	build xor xorGate;
	
	link xorGate.a a;
	link xorGate.b b;
	
	nor xorOutput xorGate.result;
	
	output result xorOutput;
	
end_blueprint;