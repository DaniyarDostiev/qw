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
    
    public partial class Список_фильтров_над_обработкой
    {
        public int id { get; set; }
        public Nullable<int> id_обработки_на_профиле { get; set; }
        public Nullable<int> id_фильтра_обработки { get; set; }
        public Nullable<System.DateTime> дата_добавления_записи { get; set; }
        public Nullable<System.DateTime> дата_последнего_изменения_записи { get; set; }
        public Nullable<bool> удален { get; set; }
    
        public virtual Обработки_на_профиле Обработки_на_профиле { get; set; }
        public virtual Фильтры_обработок Фильтры_обработок { get; set; }
    }
}
