using System.Text.RegularExpressions;

var regla = Console.ReadLine();
var texto = Console.ReadLine();
if (texto == null || regla == null)
{

    regla = @"(a+)+";
    texto = "a";
}

var respuesta = Regex.IsMatch(texto, regla);
Console.WriteLine(respuesta);

