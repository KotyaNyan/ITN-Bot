using Dadata;
namespace TelegramBot
{
    public class BotCommands
    {
        //Возвращает название компании и адрес компании
        public static async Task<string> FindFullINNAsync(string[] recievedMessage, SuggestClientAsync daDataApi)
        {
            if (recievedMessage.Length >= 2)
            {
                string replyMessage = "";
                for (int i = 1; i <= recievedMessage.Length - 1; i++)
                {
                    var company = await daDataApi.SuggestParty(recievedMessage[i]); //Получение название компании
                    if (company.suggestions.Count > 0 && recievedMessage[i].Length==10)
                    {
                        string companyName = company.suggestions[0].value;
                        string companyAdress = company.suggestions[0].data.address.value;
                        string companyOwner = company.suggestions[0].data.management.name;
                        string okvedCodes = company.suggestions[0].data.okved;
                        replyMessage += $"ИНН: {recievedMessage[i]}\n" +
                            $"Название компании: {companyName}\n" +
                            $"Адресс: {companyAdress}\n" +
                            $"Владелец: {companyOwner}\n" +
                            $"Код ОКВЕД: {okvedCodes}\n" +
                            " \n";
                    }
                    else
                    {
                        replyMessage += $"Я не смог найти компанию с ИНН {recievedMessage[i]}, проверьте правильно ли введён ИНН.\n \n";
                    }
                } return replyMessage;

            }
            else
            {
                return "Введите инн комапании, например /inn 1111111111 или /inn 1111111111 2222222222.";
            }
        }

        public static async Task<string> FindINNAsync(string[] recievedMessage, SuggestClientAsync daDataApi)
        {
            if (recievedMessage.Length >= 2)
            {
                string replyMessage = "";
                for (int i = 1; i <= recievedMessage.Length - 1; i++)
                {
                    var company = await daDataApi.SuggestParty(recievedMessage[i]); //Получение название компании
                    if (company.suggestions.Count > 0 && recievedMessage[i].Length == 10)
                    {
                        string companyName = company.suggestions[0].value;
                        string companyAdress = company.suggestions[0].data.address.value;
                        replyMessage += $"ИНН: {recievedMessage[i]}\n" +
                            $"Название компании: {companyName}\n" +
                            $"Адресс: {companyAdress}\n" +
                            " \n";
                    }
                    else
                    {
                        replyMessage += $"Я не смог найти компанию с ИНН {recievedMessage[i]}, проверьте правильно ли введён ИНН.\n \n";
                    }
                }
                return replyMessage;

            }
            else
            {
                return "Введите инн комапании, например /inn 1111111111 или /inn 1111111111 2222222222.";
            }
        }
    }
}
