Program Teste(input, output);
var
   x, y: integer;
var
   a: array [1..10] of integer;

procedure inicializa;
var
   i : integer;
begin
   i := (1 + i) * 2 / a;
   while i <= 10 do
       begin
           a[i] := a[0];
           i := i + 1
       end
end;

function max(a, b: integer; i, j: real) : integer;
begin
   if a > b then max := a else max := b
end;

begin
   inicializa;
   x := 10;
   y := 20;
   if max(x, y) = x then
       x := y
   else
       y := x
end.

