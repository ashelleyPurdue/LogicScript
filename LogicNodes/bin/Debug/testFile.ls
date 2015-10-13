include_blueprints exampleBlueprints.ls;

input_button a;
input_button b;

build xor xorGate;

link xorGate.a a;
link xorGate.b b;