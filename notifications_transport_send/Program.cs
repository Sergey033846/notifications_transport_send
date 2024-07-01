using notifications_transport_bd;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using System.Security.Cryptography;

namespace notifications_transport_send
{
    // для десериализации json 
    public class Data
    {
        public int id { get; set; }
        public string from { get; set; }
        public string number { get; set; }
        public string text { get; set; }
        public int status { get; set; }
        public string extendStatus { get; set; }
        public string channel { get; set; }
        public double cost { get; set; }
        public int dateCreate { get; set; }
        public int dateSend { get; set; }
    }

    public class SMSSendResponse
    {
        public bool success { get; set; }
        public Data data { get; set; }
        public object message { get; set; }
    }
    //--------------------------------------------------------

    public class SMSAERO
    {
        // Константы с параметрами отправки
        const bool DEBUG = false; // Вывод информации об отправке
        const string SMSAero_LOGIN = ""; // логин
        const string SMSAero_API_Key = ""; // ваш api-key
        const string From = "OKE38.RU"; // подпись отправителя

        /**
        * Отправка сообщения
        * @param numbers dynamic    - Номер телефона
        * @param text string        - Текст сообщения
        * @param channel string     - Канал отправки
        * @param dateSend integer   - Дата для отложенной отправки сообщения (в формате unixtime)
        * @param callbackUrl string - url для отправки статуса сообщения в формате http://mysite.com или https://mysite.com (в ответ система ждет статус 200)
        * @return string(json)
        */
        public string sms_send(Request request)
        {
            string Data = "";


            if (request.numbers != null)
            {
                foreach (string number in request.numbers)
                {
                    Data += "numbers[]=" + number + "&";
                }
            }
            if (request.number != null)
            {
                Data += "number=" + request.number + "&";
            }
            if (request.text != null)
            {
                Data += "text=" + request.text + "&";
            }
            if (request.channel != null)
            {
                Data += "channel=" + request.channel + "&";
            }
            /*if (request.dateSend != null)
            {
                Data += "dateSend=" + request.dateSend + "&";
            }*/
            if (request.callbackUrl != null)
            {
                Data += "callbackUrl=" + request.callbackUrl + "&";
            }
            Data += "sign=" + From;
            string method = "sms/send/";
            return send(Data, method);
        }

        /**
         * Проверка статуса SMS сообщения
         * @param   id int - Идентификатор сообщения
         * @return  string(json)
         */
        public string check_send(Request request)
        {
            string Data = "id=" + request.id;
            string method = "sms/status/";
            return send(Data, method);
        }

        /**
         * Получение списка отправленных sms сообщений
         * @param   number      string  - Фильтровать сообщения по номеру телефона
         * @param   text        string  - Фильтровать сообщения по тексту
         * @param   page        integer - Номер страницы
         * @return  string(json)
         */
        public string sms_list(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number + "&";
            }
            if (request.text != null)
            {
                Data += "text=" + request.text + "&";
            }
            if (request.page != null)
            {
                Data += "page=" + request.page + "&";
            }
            string method = "sms/list/";
            return send(Data, method);
        }

        /**
         * Запрос баланса
         * @return string(json)
         */
        public string balance()
        {
            string Data = "";
            string method = "balance";

            return send(Data, method);
        }

        /**
         * Тестовый метод для проверки авторизации
         * @return string(json)
         */
        public string auth()
        {
            string Data = "";
            string method = "auth";

            return send(Data, method);
        }

        /**
         * Получение платёжных карт
         * @return string(json)
         */
        public string cards()
        {
            string Data = "";
            string method = "cards";

            return send(Data, method);
        }

        /**
         * Пополнение баланса
         * @return string(json)
         */
        public string addbalance()
        {
            /*string Data = "sum" + request.sum;
            Data += "cardId" + request.cardId;
            string method = "balance/add/";

            return send(Data, method);*/
            return null;
        }

        /**
         * Запрос тарифа
         * @return string(json)
         */
        public string tariffs()
        {
            string Data = "";
            string method = "tariffs";

            return send(Data, method);
        }

        /**
         * Добавление подписи
         * @param name - Имя подписи
         * @return string(json)
         */
        public string sign_add(Request request)
        {
            string Data = "name=" + request.name;
            string method = "sign/add/";

            return send(Data, method);
        }

        /**
         * Получить список подписей
         * @return string(json)
         */
        public string sign_list()
        {
            string Data = "";
            string method = "sign/list/";

            return send(Data, method);
        }

        /**
        * Добавление группы
        * @param name string - Имя  группы
        * @return string(json)
        */
        public string group_add(Request request)
        {
            string Data = "name=" + request.name;
            string method = "group/add/";

            return send(Data, method);
        }

        /**
        * Удаление группы
        * @param id int - Идентификатор группы
        * @return string(json)
        */
        public string group_delete(Request request)
        {
            string Data = "id=" + request.id;
            string method = "group/delete/";

            return send(Data, method);
        }

        /**
         * Получение списка групп
         * @param page int - Пагинация
         * @return string(json)
         */
        public string group_list(Request request)
        {
            string Data = "";
            if (request.page != null)
            {
                Data += "page=" + request.page;
            }
            string method = "group/list/";

            return send(Data, method);
        }

        /**
        * Добавление контакта
        * @param number     string  - Номер абонента
        * @param groupId    int     - Идентификатор группы
        * @param birthday   int     - Дата рождения абонента (в формате unixtime)
        * @param sex        string  - Пол
        * @param lname      string  - Фамилия абонента
        * @param fname      string  - Имя абонента
        * @param sname      string  - Отчество абонента
        * @param param1     string  - Свободный параметр
        * @param param2     string  - Свободный параметр
        * @param param3     string  - Свободный параметр
        * @return string(json)
        */
        public string contact_add(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number + "&";
            }
            if (request.groupId != null)
            {
                Data += "groupId=" + request.groupId + "&";
            }
            if (request.birthday != null)
            {
                Data += "birthday=" + request.birthday + "&";
            }
            if (request.sex != null)
            {
                Data += "sex=" + request.sex + "&";
            }
            if (request.lname != null)
            {
                Data += "lname=" + request.lname + "&";
            }
            if (request.fname != null)
            {
                Data += "fname=" + request.fname + "&";
            }
            if (request.sname != null)
            {
                Data += "sname=" + request.sname + "&";
            }
            if (request.param1 != null)
            {
                Data += "param1=" + request.param1 + "&";
            }
            if (request.param2 != null)
            {
                Data += "param2=" + request.param2 + "&";
            }
            if (request.param3 != null)
            {
                Data += "param3=" + request.param3 + "&";
            }
            string method = "contact/add/";
            return send(Data, method);
        }

        /**
        * Удаление контакта
        * @param id int - Идентификатор абонента
        * @return string(json)
        */
        public string contact_delete(Request request)
        {
            string Data = "id=" + request.id;
            string method = "contact/delete/";

            return send(Data, method);
        }

        /**
         * Список контактов
         * @param number    string  - Номер абонента
         * @param groupId   int     - Идентификатор группы
         * @param birthday  int     - Дата рождения абонента (в формате unixtime)
         * @param sex       string  - Пол
         * @param operat    string  - Оператор
         * @param lname     string  - Фамилия абонента
         * @param fname     string  - Имя абонента
         * @param sname     string  - Отчество абонента
         * @param param1    string  - Свободный параметр
         * @param param2    string  - Свободный параметр
         * @param param3    string  - Свободный параметр
         * @param page      integer - Пагинация
         * @return string(json)
         */
        public string contact_list(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number + "&";
            }
            if (request.groupId != null)
            {
                Data += "groupId=" + request.groupId + "&";
            }
            if (request.birthday != null)
            {
                Data += "birthday=" + request.birthday + "&";
            }
            if (request.sex != null)
            {
                Data += "sex=" + request.sex + "&";
            }
            if (request.operat != null)
            {
                Data += "operator=" + request.operat + "&";
            }
            if (request.lname != null)
            {
                Data += "lname=" + request.lname + "&";
            }
            if (request.fname != null)
            {
                Data += "fname=" + request.fname + "&";
            }
            if (request.sname != null)
            {
                Data += "sname=" + request.sname + "&";
            }
            if (request.param1 != null)
            {
                Data += "param1=" + request.param1 + "&";
            }
            if (request.param2 != null)
            {
                Data += "param2=" + request.param2 + "&";
            }
            if (request.param3 != null)
            {
                Data += "param3=" + request.param3 + "&";
            }
            if (request.page != null)
            {
                Data += "page=" + request.page + "&";
            }
            string method = "contact/list/";
            return send(Data, method);
        }

        /**
         * Добавление в чёрный список
         * @param number    string  - Номер абонента
         * @param numbers   array   - Номера телефонов
         * @return string(json)
         */
        public string blacklist_add(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number;
            }
            if (request.numbers != null)
            {
                foreach (string number in request.numbers)
                {
                    Data += "numbers[]=" + number + "&";
                }
            }
            string method = "blacklist/add/";
            return send(Data, method);
        }

        /**
         * Удаление из черного списка
         * @param id int - Идентификатор абонента
         * @return string(json)
         */
        public string blacklist_delete(Request request)
        {
            string Data = "id=" + request.id;
            string method = "blacklist/delete/";

            return send(Data, method);
        }

        /**
         * Список контактов в черном списке
         * @param number    string  - Номер абонента
         * @param page      int     - Пагинация
         * @return string(json)
         */
        public string blacklist_list(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number;
            }
            if (request.page != null)
            {
                Data += "page=" + request.page;
            }
            string method = "blacklist/list/";
            return send(Data, method);
        }

        /**
         * Создание запроса на проверку HLR
         * @param number    string  - Номер абонента
         * @param numbers   array   - Номера телефонов
         * @return string(json)
         */
        public string hlr_check(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number;
            }
            if (request.numbers != null)
            {
                foreach (string number in request.numbers)
                {
                    Data += "numbers[]=" + number + "&";
                }
            }
            string method = "hlr/status/";
            return send(Data, method);
        }

        /**
         * Определение оператора
         * @param number    string  - Номер абонента
         * @param numbers   array   - Номера телефонов
         * @return string(json)
         */
        public string number_operator(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number;
            }
            if (request.numbers != null)
            {
                foreach (string number in request.numbers)
                {
                    Data += "numbers[]=" + number + "&";
                }
            }
            string method = "number/operator/";
            return send(Data, method);
        }

        /**
         * Отправка Viber-рассылок
         * @param number        string  - Номер абонента
         * @param numbers       array   - Номера телефонов. Максимальное количество 50
         * @param groupId       dynamic - ID группы по которой будет произведена рассылка. Для выбора всех контактов необходимо передать значение "all"
         * @param sign          string  - Подпись отправителя
         * @param channel       string  - Канал отправки Viber
         * @param text          string  - Текст сообщения
         * @param imageSource   string  - Картинка кодированная в base64 формат, не должна превышать размер 300 kb.
         * Отправка поддерживается только в 3 форматах: png, jpg, gif. Перед кодированной картинкой необходимо указывать её формат.
         * Пример: jpg#TWFuIGlzIGRpc3Rpbmd1aXNoZ. Отправка доступна только методом POST. Параметр передается совместно с textButton и linkButton
         * @param textButton    string  - Текст кнопки. Максимальная длина 30 символов. Параметр передается совместно с imageSource и linkButton
         * @param linkButton    string  - Ссылки для перехода при нажатие кнопки. Ссылка должна быть с указанием http:// или https://. Параметр передается совместно с imageSource и textButton
         * @param dateSend      int     - Дата для отложенной отправки рассылки (в формате unixtime)
         * @param signSms       string  - Подпись для SMS-рассылки. Используется при выборе канала "Viber-каскад" (channel=CASCADE). Параметр обязателен
         * @param channelSms    string  - Канал отправки SMS-рассылки. Используется при выборе канала "Viber-каскад" (channel=CASCADE). Параметр обязателен
         * @param textSms       string  - Текст сообщения для SMS-рассылки. Используется при выборе канала "Viber-каскад" (channel=CASCADE). Параметр обязателен
         * @param priceSms      int     - Максимальная стоимость SMS-рассылки. Используется при выборе канала "Viber-каскад" (channel=CASCADE). Если параметр не передан, максимальная стоимость будет рассчитана автоматически
         * @return string(json)
         */
        public string viber_send(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number;
            }
            if (request.numbers != null)
            {
                foreach (string number in request.numbers)
                {
                    Data += "numbers[]=" + number + "&";
                }
            }
            if (request.groupId != null)
            {
                Data += "groupId=" + request.groupId;
            }
            if (request.sign != null)
            {
                Data += "sign=" + request.sign;
            }
            if (request.channel != null)
            {
                Data += "channel=" + request.channel;
            }
            if (request.text != null)
            {
                Data += "text=" + request.text;
            }
            if (request.imageSource != null)
            {
                Data += "imageSource=" + request.imageSource;
            }
            if (request.textButton != null)
            {
                Data += "textButton=" + request.textButton;
            }
            if (request.linkButton != null)
            {
                Data += "linkButton=" + request.linkButton;
            }
            if (request.dateSend != null)
            {
                Data += "dateSend=" + request.dateSend;
            }
            if (request.signSms != null)
            {
                Data += "signSms=" + request.signSms;
            }
            if (request.channelSms != null)
            {
                Data += "channelSms=" + request.channelSms;
            }
            if (request.textSms != null)
            {
                Data += "textSms=" + request.textSms;
            }
            if (request.priceSms != null)
            {
                Data += "priceSms=" + request.priceSms;
            }

            string method = "viber/send/";
            return send(Data, method);
        }

        /**
        * Статистика по Viber-рассылке
        * @param sendingId int  - Идентификатор Viber-рассылки в системе
        * @param page int       - Пагинация
        * @return string(json)
        */
        public string viber_statistic(Request request)
        {
            string Data = "";
            if (request.sendingId != null)
            {
                Data += "sendingId=" + request.sendingId;
            }
            if (request.page != null)
            {
                Data += "priceSms=" + request.page;
            }
            string method = "viber/statistic/";
            return send(Data, method);
        }

        /**
         * Список Viber-рассылок
         * @return string(json)
         */
        public string viber_list()
        {
            string Data = "";
            string method = "viber/list/";
            return send(Data, method);
        }

        /**
        * Список доступных подписей для Viber-рассылок
        * @return string(json)
        */
        public string viber_sign_list()
        {
            string Data = "";
            string method = "viber/sign/list/";
            return send(Data, method);
        }

        // ПРИВАТНЫЕ МЕТОДЫ

        // вывод сообщений в консоль

        private void _print_debug(string str)
        {
            Console.WriteLine(str);
            Console.ReadLine();
        }

        // отправка

        private string send(string Data, string method)
        {

            string basic_auth_data = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(SMSAero_LOGIN + ":" + SMSAero_API_Key));

            string url = ("https://gate.smsaero.ru/v2/" + method);

            WebRequest req = WebRequest.Create(url + "?" + Data);
            req.Headers.Add("Authorization", "Basic " + basic_auth_data);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();

            //отладка включена
            if (DEBUG)
            {
                _print_debug(Out);
            }
            return Out;
        }
    }

    public class Request
    {
        public int id { get; set; }
        public string number { get; set; }
        public string[] numbers { get; set; }
        public string text { get; set; }
        public string channel { get; set; }
        public int dateSend { get; set; }
        public string callbackUrl { get; set; }
        public int page { get; set; }
        public string name { get; set; }
        public dynamic groupId { get; set; }
        public int birthday { get; set; }
        public string sex { get; set; }
        public string lname { get; set; }
        public string fname { get; set; }
        public string sname { get; set; }
        public string param1 { get; set; }
        public string param2 { get; set; }
        public string param3 { get; set; }
        public string operat { get; set; } // оператор
        public string sign { get; set; }
        public string imageSource { get; set; }
        public string textButton { get; set; }
        public string linkButton { get; set; }
        public string signSms { get; set; }
        public string channelSms { get; set; }
        public string textSms { get; set; }
        public string priceSms { get; set; }
        public int sendingId { get; set; }
        public int sum { get; set; }
        public int cardId { get; set; }
    }

    class Program
        {
        static NotificationsBDEntities notificationsBDAdapter;

        public static void SendNotifiesByEmail(object obj)
        {
            Console.WriteLine("Начата отправка пакетов сообщений");

            /*Notifications tempN = new Notifications();// notificationsBDAdapter.Notifications.Select(p => p).First();
            tempN.docNumber = "csi";
            tempN.docDate = DateTime.Now;
            tempN.codeIESBK = "csi";
            tempN.codeOKE = "csi";
            tempN.checkDate = DateTime.Now;
            tempN.checkTime = "csi";
            tempN.dateAdded = DateTime.Now;
            tempN.sendStatus = "В очереди";
            notificationsBDAdapter.Notifications.Add(tempN);            
            notificationsBDAdapter.SaveChanges();*/
            
            if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 20)
            {

                // пакет1 уведомлений для потребителей по email

                // выбираем не более maxCount записей "в очереди" с непустым полем "emailClient" до планируемой даты отправки            
                int maxCountEmail = 13;
                DateTime currentDate = DateTime.Now.Date;
                IQueryable<Notifications> notifiesListEmail =
                    notificationsBDAdapter.Notifications
                        //.Where(p => String.Equals(p.sendStatus, "В очереди") && !String.IsNullOrEmpty(p.emailClient) && currentDate <= p.sendDatePlan)
                        .Where(p => String.Equals(p.sendStatus, "В очереди") && !String.IsNullOrEmpty(p.emailClient) && !String.Equals(p.emailClient, "e@mail.net") && currentDate <= p.checkDate)
                        .Select(p => p).OrderBy(p => p.dateAdded).Take(maxCountEmail);

                Console.WriteLine("Пакет 1 (email). В очереди {0} сообщений", notifiesListEmail.Count());

                int i1 = 0;
                foreach (Notifications notifyTemp in notifiesListEmail)
                {
                    Console.WriteLine("Сообщение {0} - дата добавления {1}:", ++i1, notifyTemp.dateAdded);

                    MailMessage mail = new MailMessage();

                    string userEmail = notifyTemp.emailClient;                    
                    mail.To.Add(new MailAddress(userEmail, userEmail));

                    mail.CC.Add(new MailAddress("admin@oke38.ru", "admin@oke38.ru")); // копия письма на ящик администратора
                    //mail.CC.Add(new MailAddress("oke38operators@oblkomenergo.ru", "oke38operators"));

                    mail.IsBodyHtml = true;
                    mail.BodyEncoding = Encoding.UTF8;
                    mail.SubjectEncoding = Encoding.GetEncoding(1251);

                    mail.Subject = "Уведомление о проведении проверки прибора учёта электрической энергии";

                    mail.Body = "";
                    string clientType = notifyTemp.clientType;
                    if (String.Equals(clientType, "ФЛ"))
                    {
                        mail.Body = String.Concat(
                            "<p>Уважаемый потребитель!</p>",                            
                            String.Format("<p>Территориальная сетевая организация ОГУЭП «Облкоммунэнерго» уведомляет Вас о проведении <b>{0:d}</b> <b>{1}</b> местного времени (время иркутское) проверки расчетного прибора учёта электрической энергии, установленного в отношении электроустановки,  расположенной по адресу: <b>{2}</b>, представителями ОГУЭП «Облкоммунэнерго» филиала <b>\"{3}\"</b> в соответствии с п. 170  Основных положений функционирования розничных рынков электрической энергии, утверждённых Постановлением Правительства РФ от 04.05.2012г. № 442, с п.82 Правил предоставления коммунальных услуг собственникам и пользователям помещений в многоквартирных домах и жилых домов, утвержденных  Постановлением Правительства РФ от 06.05.2011 № 354.</p>", notifyTemp.checkDate, notifyTemp.checkTime, notifyTemp.addressEPU, notifyTemp.filial),
                            "<p>Если Вы не сможете обеспечить представителям филиала ОГУЭП «Облкоммунэнерго» доступ в указанную электроустановку для проведения проверки расчетного прибора учета электрической энергии, в указанные в настоящем уведомлении дату и (или) время, Вы обязаны направить в адрес указанного филиала ОГУЭП «Облкоммунэнерго» ответное извещение в срок не позднее 2-х дней до даты проведения проверки, указанной в настоящем уведомлении, в котором предложить иные возможные дату (даты) и время допуска для проведения проверки, удобные для Вас, при этом предлагаемая Вами дата проверки не может быть ранее 2-х дней с даты, когда в адрес указанного филиала ОГУЭП «Облкоммунэнерго» поступит от Вас такое сообщение, и позднее 3-х дней с даты, указанной в извещении о проведении проверки (подпункт б) п. 85 Правил № 354).</p>",
                            "<p>Если Вы не обеспечили допуск представителей сетевой организации в занимаемое потребителем жилое помещение в дату и время, указанные в извещении о проведении проверки или в иные указанные потребителем в сообщении дату (даты) и время, и при этом в отношении потребителя, проживающего в жилом помещении, у указанных лиц отсутствует информация о его временном отсутствии в занимаемом жилом помещении, то сетевая организация составляет акт об отказе в допуске к прибору учета электрической энергии (подпункт «г)» п. 85 Правил № 354.</p>",
                            "<p>В случае двукратного недопуска Вами представителей филиала ОГУЭП «Облкоммунэнерго» в занимаемое Вами жилое и (или) нежилое помещение для установки индивидуальных, общих (квартирных) приборов учета электрической энергии, ввода их в эксплуатацию, проверки состояния установленных и введенных в эксплуатацию приборов учета, а также для проведения работ по обслуживанию приборов учета и их подключения к интеллектуальной системе учета электрической энергии (мощности) плата за коммунальную услугу по электроснабжению рассчитывается исходя из нормативов потребления коммунальных услуг с применением к стоимости повышающего коэффициента, величина которого принимается равной 1,5, начиная с расчетного периода, когда гарантирующим поставщиком (сетевой организацией - в отношении жилого дома (домовладения) был составлен повторный акт об отказе в допуске к прибору учета и (или) месту установки прибора учета п. 60(3) Правил № 354.</p>",
                            "<p>Дополнительно сообщаем:<br/>При недопуске 2 и более раз потребителем в занимаемое им жилое и (или) нежилое помещение представителя сетевой организации для проверки состояния установленных и введенных в эксплуатацию индивидуальных, общих (квартирных) приборов учета, проверки достоверности представленных сведений о показаниях таких приборов учета и при условии составления исполнителем акта об отказе в допуске к прибору учета показания такого прибора учета, предоставленные потребителем, не учитываются при расчете платы за коммунальные услуги до даты подписания акта проведения указанной проверки. В случае непредоставления потребителем допуска в занимаемое им жилое помещение, домовладение сетевой организации по истечении указанного в подпункте \"в\" пункта 59 настоящих Правил предельного количества расчетных периодов, за которые плата за коммунальную услугу определяется по данным, предусмотренным указанным пунктом, размер платы за коммунальные услуги рассчитывается с учетом повышающих коэффициентов в соответствии с приведенными в приложении N 2 к настоящим Правилам формулами расчета размера платы за коммунальные услуги холодного водоснабжения, горячего водоснабжения, электроснабжения, предусматривающими применение повышающих коэффициентов, начиная с расчетного периода, следующего за расчетным периодом, указанным в подпункте \"в\" пункта 59 настоящих Правил, до даты составления акта проверки (абзац 4 п. . 60 (1). Правил № 354).</p>",
                            String.Format("<p>Адрес электронной почты для связи: {0}</p>", notifyTemp.emailFilial),
                            String.Format("<p>Телефон для связи: {0}</p>", notifyTemp.phoneFilial)
                            );
                    }
                    else if (String.Equals(clientType, "ЮЛ"))
                    {                        
                        mail.Body = String.Concat(
                            "<p>Уважаемый потребитель!</p>",
                            String.Format("<p>Территориальная сетевая организация ОГУЭП «Облкоммунэнерго» уведомляет Вас о проведении <b>{0:d}</b> <b>{1}</b> местного времени (время иркутское) проверки расчетного прибора учёта электрической энергии, установленного в отношении электроустановки,  расположенной по адресу: <b>{2}</b>, представителями ОГУЭП «Облкоммунэнерго» филиала <b>\"{3}\"</b> в соответствии с п. 170 Основных положений функционирования розничных рынков электрической энергии, утверждённых Постановлением Правительства РФ от 04.05.2012г. № 442.</p>", notifyTemp.checkDate, notifyTemp.checkTime, notifyTemp.addressEPU, notifyTemp.filial),
                            "<p>Если Вы не сможете обеспечить представителям филиала ОГУЭП «Облкоммунэнерго» доступ в указанную электроустановку для проведения проверки расчетного прибора учета электрической энергии, в указанные в настоящем уведомлении дату и (или) время, <b<Вы обязаны не позднее 10 рабочих дней со дня предложенной сетевой организацией даты</b> направить в адрес указанного филиала ОГУЭП «Облкоммунэнерго» предложение (извещение) об иных дате и (или) времени проверки расчетного прибора учета, которые подлежат обязательному согласованию сетевой организацией.</p>",
                            String.Format("<p>Адрес электронной почты для связи: {0}</p>", notifyTemp.emailFilial),
                            String.Format("<p>Телефон для связи: {0}</p>", notifyTemp.phoneFilial)
                            );
                    }

                    mail.From = new MailAddress("robot70@oke38.ru", "ОГУЭП Облкоммунэнерго", Encoding.GetEncoding(1251));
                    SmtpClient smtp = new SmtpClient("mail.nic.ru", 2525);
                    smtp.Credentials = new NetworkCredential("robot70@oke38.ru", "P7pHmW5TgPZ3k");

                    try
                    {
                        Console.WriteLine(mail.To[0].ToString());

                        smtp.Send(mail);
                        notifyTemp.sendDateFact = DateTime.Now;
                        notifyTemp.sendStatus = "Отправлено";
                        //notificationsBDAdapter.SaveChanges();

                        Console.WriteLine("отправлено");
                        Console.WriteLine();
                    }
                    catch (SmtpFailedRecipientException)
                    {
                        notifyTemp.sendStatus = "Не отправлено";
                        //notificationsBDAdapter.SaveChanges();
                    }
                    catch (InvalidOperationException)
                    { }
                    catch (ArgumentNullException)
                    { }
                    //---------------------------

                    //notificationsBDAdapter.SaveChanges();
                    Thread.Sleep(15000);

                } // foreach (tblNotify notifyTemp in notifiesList)

                notificationsBDAdapter.SaveChanges();
                Console.WriteLine("Отправка Пакета 1 (email) завершена");
                Console.WriteLine();

                //---------------------

                // пакет2 уведомлений для потребителей по SMS
                SMSAERO smsc = new SMSAERO();

                // выбираем не более maxCount записей "в очереди" с пустым полем "emailClient" до планируемой даты отправки
                int maxCountSMS = 80; // 40
                currentDate = DateTime.Now.Date;
                IQueryable<Notifications> notifiesListSMS =
                    notificationsBDAdapter.Notifications
                        //.Where(p => String.Equals(p.sendStatus, "В очереди") && String.IsNullOrEmpty(p.emailClient) && currentDate <= p.sendDatePlan)
                        .Where(p => String.Equals(p.sendStatus, "В очереди") && (String.IsNullOrEmpty(p.emailClient) || String.Equals(p.emailClient, "e@mail.net")) 
                        && currentDate <= p.checkDate && !String.IsNullOrEmpty(p.phoneClient))
                        .Select(p => p).OrderBy(p => p.dateAdded).Take(maxCountSMS);
                
                Console.WriteLine("Пакет 2 (sms). В очереди {0} сообщений", notifiesListSMS.Count());

                int i2 = 0;
                foreach (Notifications notifyTemp in notifiesListSMS)
                {
                    Console.WriteLine("Сообщение {0} - дата добавления {1}:", ++i2, notifyTemp.dateAdded);
                                        
                    notifyTemp.phoneClient = notifyTemp.phoneClient.Replace(" ", String.Empty).Replace("+", String.Empty);

                    string[] phoneClientList = notifyTemp.phoneClient.Split(',');

                    foreach (string phoneClientTemp in phoneClientList)
                    {

                        var request = new Request
                        {
                            number = phoneClientTemp,
                            //numbers = notifyTemp.phoneClient.Split(','),
                            //text = String.Format("{0:d} будет проведена проверка электросчетчика.Тел.{1}", notifyTemp.checkDate, notifyTemp.phoneFilial),
                            text = String.Format("{0:dd-MM-yy} будет проведена проверка электросчетчика,т.{1}", notifyTemp.checkDate, notifyTemp.phoneFilial),
                            channel = "DIRECT"
                        };

                        try
                        {
                            var send_sms_status = smsc.sms_send(request);
                            Console.WriteLine(send_sms_status);
                            Console.WriteLine();

                            SMSSendResponse smsSendResponse = JsonConvert.DeserializeObject<SMSSendResponse>(send_sms_status);

                            if (smsSendResponse.success)
                            {
                                notifyTemp.sendDateFact = DateTime.Now;
                                notifyTemp.sendStatus = "Отправлено";
                                notifyTemp.comment = String.Concat(notifyTemp.comment, smsSendResponse.data.id.ToString(), ",");
                            }

                            Thread.Sleep(8000);
                        }
                        catch
                        {

                        }

                        //notificationsBDAdapter.SaveChanges();
                        //Thread.Sleep(8000);
                    }

                } // foreach (tblNotify notifyTemp in notifiesList)

                notificationsBDAdapter.SaveChanges();
                Console.WriteLine("Отправка Пакета 2 (sms) завершена");
                Console.WriteLine();

            } // if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 20)

        } // public static void SendNotifiesByEmail(object obj)

        static void Main()
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            Console.WriteLine("Для запуска отправки уведомлений о проверках электроустановок нажмите Enter");

            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                Console.WriteLine("Процесс отправки email запущен! (для выхода нажмите Esc)");

                notificationsBDAdapter = new NotificationsBDEntities();
                
                // устанавливаем метод обратного вызова
                TimerCallback tm = new TimerCallback(SendNotifiesByEmail);
                // создаем таймер на каждые 15 минут
                Timer timer = new Timer(tm, null, 0, 15 * 60000);

                //---------------------

                //Console.Write("Все уведомления отправлены");
                while (cki.Key != ConsoleKey.Escape)
                {
                    //Console.Write("Текст ");

                    if (Console.KeyAvailable == true)
                    {
                        cki = Console.ReadKey(true);
                    }
                }
            }
            Console.Write("Цикл завершился");
            Console.ReadKey();
        }

    } // class Program
}
