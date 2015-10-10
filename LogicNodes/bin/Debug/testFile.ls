blueprint xor;

	input a;
	input b;
	
	nor notA a;
	nor notB b;
	
	and aTrue a notB;
	and bTrue b notA;
	
	output result aTrue bTrue;
end_blueprint;

build xor xor_instance;

input_button a;
input_button b;

link xor_instance.a a;
link xor_instance.b b;