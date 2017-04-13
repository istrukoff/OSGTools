using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools.Objects
{
    public static class AvitoAd
    {
        public static int id { get; set; } // идентификатор в БД

        public static int idlogin { get; set; } // идентификатор аккаунта

        public static string name { get; set; } // заголовок объявления

        public static string price { get; set; } // указанная цена

        public static string description { get; set; } // описание объявления

        public static int status { get; set; } // статус объявления

        public static int categoryid { get; set; } // идентификатор категории

        public static string city { get; set; } // указанный город

        public static string picture { get; set; } // путь к изображениям

        public static string url { get; set; } // адрес объявления

        public static string date { get; set; } // дата публикации

        public static string view_count { get; set; } // количество просмотров

        public static string view_count_today { get; set; } // количество просмотров за день

        public static string publish_count { get; set; } // количество публикаций объявления
    }
}