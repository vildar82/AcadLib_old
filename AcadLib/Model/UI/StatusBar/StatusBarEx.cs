using System;
using System.Collections.Generic;
using System.Drawing;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.Windows;
using JetBrains.Annotations;
using NetLib;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using StatusBarMenu = AcadLib.UI.StatusBar.View.StatusBarMenu;

namespace AcadLib.UI.StatusBar
{
    /// <summary>
    /// Статусная строка
    /// </summary>
    [PublicAPI]
    public static class StatusBarEx
    {
        /// <summary>
        /// Добавление панели с выпадающим списком значений.
        /// </summary>
        /// <param name="value">текщее знасчение</param>
        /// <param name="values">Значения</param>
        /// <param name="toolTip">Описание</param>
        /// <param name="selectValue">Действие при выборе значения</param>
        /// <param name="showMenu">Показ меню - текущее значение</param>
        /// <param name="minWidth"></param>
        /// <param name="maxWidth"></param>
        [NotNull]
        public static Pane AddMenuPane(string value, List<string> values, string toolTip, Action<string> selectValue,
            Func<string> showMenu, int minWidth =0, int maxWidth =0)
        {
            var pane = new Pane {Text = value, Style = PaneStyles.PopUp | PaneStyles.Normal, ToolTipText = toolTip};
            pane.MouseDown += (o, e) =>
            {
                new StatusBarMenu(showMenu(), values, selectValue).Show();
            };
            pane.Visible = false;
            Application.StatusBar.Panes.Insert(0, pane);
            if (minWidth != 0) pane.MinimumWidth = minWidth;
            if (maxWidth != 0) pane.MinimumWidth = maxWidth;
            pane.Visible = true;
            Application.StatusBar.Update();
            return pane;
        }

        public static void AddPane(string name, string toolTip,
            [CanBeNull] Action<StatusBarMouseDownEventArgs> onClick = null,
            int minWith = 20, int maxWith = 200, [CanBeNull] Icon icon = null, PaneStyles style = PaneStyles.Normal)
        {
            var pane = new Pane
            {
                Text = name,
                ToolTipText = toolTip,
                Visible = false,
                MinimumWidth = minWith,
                MaximumWidth = maxWith,
                Style = style
            };
            if (icon != null) pane.Icon = icon;
            pane.MouseDown += (s, e) => onClick?.Invoke(e);
            Application.StatusBar.Panes.Insert(0, pane);
            pane.Visible = true;
            Application.StatusBar.Update();
        }

        public static void AddPaneUserGroup()
        {
            AddPane(PikSettings.UserGroup,
                $"{PikSettings.Versions.JoinToString(GetGroupVersionInfo, "\n")}",
                onClick: e=> AcadHelper.Doc.SendStringToExecute("_ToolPalettes ", true, false, true));
        }

        [NotNull]
        private static string GetGroupVersionInfo([NotNull] GroupInfo groupInfo)
        {
            var info = $"{groupInfo.GroupName}, вер: {groupInfo.VersionLocal}";
            if (groupInfo.UpdateRequired)
            {
                info +=$", на сервере: {groupInfo.VersionServer} ({groupInfo.VersionServerDate:dd.MM.yy hh:mm})";
            }
            return info;
        }
    }
}
