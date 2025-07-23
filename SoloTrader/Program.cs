using ApiService;

//var mainProvider = new MainProvider();


// Отримуємо інформацію про біржу
// var exchangeInfo = await mainProvider.BinanceService.GetExchangeInfoAsync();
// Log.Information("Exchange Info отримано успішно!");
// Console.WriteLine(exchangeInfo);    
// // Отримуємо інформацію про аккаунт
// var accountInfo = await mainProvider.BinanceService.GetAccountInfoAsync();
// Log.Information("Account Info отримано успішно!");
// Console.WriteLine(accountInfo); 

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

ApiServices apiServices = new ApiServices();
apiServices.AddApiService("binance");

await apiServices.GetApiService("binance").GetSym();
await apiServices.GetApiService("binance").GetAccountBalanceAsync();
Console.WriteLine(apiServices.GetApiService("binance").Name);