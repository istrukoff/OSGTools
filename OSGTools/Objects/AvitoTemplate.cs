using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public class AvitoTemplate
    {
        public AvitoTemplate() { }

        public AvitoTemplate(int p_id, int p_target, string p_name, string p_description, string p_price, string p_size, string p_picturespath, string p_city)
        {
            id = p_id;
            target = p_target;
            name = p_name;
            description = p_description;
            price = p_price;
            size = p_size;
            picturespath = p_picturespath;
            city = p_city;
        }

        public int id { get; set; } // идентификатор в БД

        public int target { get; set; } // идентификатор целевой аудитории

        public string name { get; set; } // заголовок

        public string description { get; set; } // описание

        public string price { get; set; } // указанная цена

        public string size { get; set; } // указанный размер

        public string picturespath { get; set; } // путь к картинкам

        public string city { get; set; } // указанный город
    }
}