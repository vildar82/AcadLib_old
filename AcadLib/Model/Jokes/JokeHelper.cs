using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.Jokes
{
    /// <summary>
    /// Шутки
    /// </summary>
    public static class JokeHelper
    {
        private static List<IJoke> jokes;
        private static Random random = new Random();

        /// <summary>
        /// Юзеры для шуток
        /// </summary>
        public static List<string> JokeUsers { get; set; } = new List<string> { "inkinli", "OstaninAM" }; //, AutoCAD_PIK_Manager.Settings.PikSettings.PikFileSettings.LoginCADManager };

        /// <summary>
        /// Текущий пользователь пригоден для шуток
        /// </summary>        
        public static bool IsJokeUser ()
        {
            return JokeUsers.Contains(Environment.UserName, StringComparer.OrdinalIgnoreCase);
        }

        public static void Show ()
        {
            var joke = GetJoke();
            try
            {
                joke.Show();
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex, "Шутка не удалась (:");
            }            
        }

        private static IJoke GetJoke ()
        {
            if (jokes == null)
            {
                jokes = LoadJokes();
            }
            var index = random.Next(jokes.Count - 1);
            var joke = jokes[index];
            return joke;
        }

        private static List<IJoke> LoadJokes ()
        {
            List<IJoke> res = new List<IJoke>();
            res.AddRange(JokeUrl.Load());            
            return res;
        }
    }
}
