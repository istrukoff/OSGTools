﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public class AvitoAd
    {
        public AvitoAd() { }

        public AvitoAd(int p_id, int p_idlogin, string p_name, string p_price, string p_description, int p_status, int p_categoryid)
        {
            id = p_id;
            idlogin = p_idlogin;
            name = p_name;
            price = p_price;
            description = p_description;
            status = p_status;
            categoryid = p_categoryid;
        }

        public int id { get; set; } // идентификатор в БД

        public int idlogin { get; set; } // идентификатор аккаунта

        public string name { get; set; } // заголовок объявления

        public string price { get; set; } // указанная цена

        public string description { get; set; } // описание объявления

        public int status { get; set; } // статус объявления

        public int categoryid { get; set; } // идентификатор категории

        public string city { get; set; } // указанный город

        public string picture { get; set; } // путь к изображениям

        public string url { get; set; } // адрес объявления

        public string date { get; set; } // дата публикации

        public string view_count { get; set; } // количество просмотров

        public string view_count_today { get; set; } // количество просмотров за день

        public string publish_count { get; set; } // количество публикаций объявления
    }
}