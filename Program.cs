using TelegramBot;
using Newtonsoft.Json;
using Dadata;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


//Словарь для /last
Dictionary<long,string> lastBotMessage= new Dictionary<long,string>();

Console.WriteLine("Запуск бота");

//Получение апи ключей из конфига
string configPath = "D:\\Coding C#\\TelegramBot\\Config.json";

//Получение конфига
string configJson = System.IO.File.ReadAllText(configPath);
Configuration config = JsonConvert.DeserializeObject<Configuration>(configJson);

//Подключение различных API
var botClient = new TelegramBotClient(config.telegramToken);
var daDataApi = new SuggestClientAsync(config.daDataToken);

//Токен остановки бота
using CancellationTokenSource cts = new();
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() 
};

//Настройки бота
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

//Запуск бота
var me = await botClient.GetMeAsync();

Console.WriteLine($"Бот @{me.Username} работает!");
Console.ReadLine();
Console.WriteLine($"Остановка бота @{me.Username}");

//Остановка бота после нажатия клавиши в консоли
cts.Cancel();

void SaveLastMessage(long chatId, string botText)
{
    lastBotMessage[chatId] = botText;
}

//Обработка входящих сообщений
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    //Обработка только текстовых сообщений
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    var recievedMessage = message.Text.Split(" ");

    string replyMessage = "";

    switch (recievedMessage[0])
    {
        case "/start":
            replyMessage = $"Привет! {message.From.FirstName} {message.From.LastName}";
            SaveLastMessage(chatId, replyMessage);
            break;

        case "/help":
            replyMessage = "Список доступных команд:\n" +
                "/start – начать общение с ботом.\n" +
                "/help – вывод справки о доступных командах.\n" +
                "/hello - выводит имя, фамилию, email и ссылку на github.\n" +
                "/inn ИНН - выдаёт название и адрессс компании по ИНН.\n" +
                "/inn ИНН1 ИНН2 ... - выдаёт названия и адрессса компаний по ИНН.\n" +
                "/full ИНН - выдаёт полную информацию о компании по ИНН. Поддерживает несколько ИНН в 1 команде\n" +
                "/last - выполняет последнюю команду.";
            SaveLastMessage(chatId, replyMessage);
            break;

        case "/hello":
            replyMessage = "Сделал: Стеценко Матвей \n"+
                "Email: kotyanyan42@gmail.com \n" +
                "GitHub: https://github.com/KotyaNyan";
            SaveLastMessage(chatId,replyMessage);
            break;

        case "/inn":
            //Получение ИНН и формаирование сообщения
            replyMessage = await BotCommands.FindINNAsync(recievedMessage, daDataApi);
            SaveLastMessage(chatId, replyMessage);
            break;

        case "/full":
            replyMessage = await BotCommands.FindFullINNAsync(recievedMessage, daDataApi);
            SaveLastMessage(chatId, replyMessage);
            break;

        case "/last":
            if (lastBotMessage.ContainsKey(chatId))
            {
                replyMessage = lastBotMessage[chatId];
            }
            break;

        default:
            replyMessage = "Неизвестная команда, пожалуйста воспользуйтесь /help";
            SaveLastMessage(chatId, replyMessage);
            break;
    }

    //Console.WriteLine($"Полученно сообщение '{messageText}' в чате {chatId}.");

    //Если почему-то сообщение ответ пустое, то не оно отправляется
    if (!replyMessage.Equals(""))
    {
        Message sentMessage = await botClient.SendTextMessageAsync(
           chatId: chatId,
           text: replyMessage,
           cancellationToken: cancellationToken);
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Ошибка Telegram Api::\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    }; ;

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
