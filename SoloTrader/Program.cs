// See https://aka.ms/new-console-template for more information

using Serilog;
using SoloTrader;

var mainProvider = new MainProvider();


// Отримуємо інформацію про біржу
// var exchangeInfo = await mainProvider.BinanceService.GetExchangeInfoAsync();
// Log.Information("Exchange Info отримано успішно!");
// Console.WriteLine(exchangeInfo);    
// // Отримуємо інформацію про аккаунт
// var accountInfo = await mainProvider.BinanceService.GetAccountInfoAsync();
// Log.Information("Account Info отримано успішно!");
// Console.WriteLine(accountInfo); 