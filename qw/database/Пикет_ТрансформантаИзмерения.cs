//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace qw.database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Пикет_ТрансформантаИзмерения
    {
        public int id { get; set; }
        public Nullable<int> id_пикета { get; set; }
        public Nullable<int> id_трансформанты_измерения { get; set; }
        public Nullable<System.DateTime> дата_добавления_записи { get; set; }
        public Nullable<System.DateTime> дата_последнего_изменения_записи { get; set; }
    
        public virtual Пикет Пикет { get; set; }
        public virtual Трансформанта_измерения Трансформанта_измерения { get; set; }
    }
}
