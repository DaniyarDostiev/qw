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
    
    public partial class Профиль_ЭлектромагнитныеИзмерения
    {
        public int id { get; set; }
        public Nullable<int> id_профиля { get; set; }
        public Nullable<int> id_эл_измерений { get; set; }
        public Nullable<System.DateTime> дата_добавления_записи { get; set; }
        public Nullable<System.DateTime> дата_последнего_изменения_записи { get; set; }
        public Nullable<bool> удален { get; set; }
    
        public virtual Профиль Профиль { get; set; }
        public virtual Электромагнитные_измерения Электромагнитные_измерения { get; set; }
    }
}
