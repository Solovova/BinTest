using ApiBin_01;
//https://www.binance.com/uk-UA/binance-api

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

var m1 = new ProgramSecrets();
m1.Run();

var m2 = new ProgramApiGet();
await m2.Run();