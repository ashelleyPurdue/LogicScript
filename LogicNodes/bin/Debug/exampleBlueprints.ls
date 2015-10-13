blueprint xor;
	
	input a;
	input b;
	
	nor notA a;
	nor notB b;
	
	and aTrue a notB;
	and bTrue b notA;
	
	output result aTrue bTrue;
	
end_blueprint;