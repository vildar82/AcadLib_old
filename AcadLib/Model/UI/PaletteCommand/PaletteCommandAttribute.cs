using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.PaletteCommands
{
    [AttributeUsage(AttributeTargets.Method)]
    /// <summary>
    /// Атрибут для команды которая будет добавлена на палитру ПИК.
    /// </summary>    
    public class PaletteCommandAttribute : Attribute
    {
        /// <summary>
        /// Человеческое короткое название команды
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description { get; set; }    
        /// <summary>
        /// Группа комманд - для объединения команд - или в отдельные палитры или еще как-то.
        /// </summary>
        public string Group { get; set; }
        
        public PaletteCommandAttribute(string name, string desc = "", string group = "")
        {
            Name = name;
            Description = desc;
            Group = group;
        }    
    }
}
