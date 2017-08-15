using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.UI.Ribbon.Elements;
using Autodesk.Windows;

namespace AcadLib.UI.Ribbon
{
	/// <summary>
	/// Создает ленту
	/// </summary>
	public class RibbonBuilder
	{
		public void CreateRibbon(List<IRibbonElement> elements)
		{
			var ribbon = ComponentManager.Ribbon;
			// группировка элементов по вкладкам
			foreach (var tabElems in elements.GroupBy(g=>g.Tab))
			{
				var tab = CreateTab(tabElems.Key, tabElems.ToList());
				ribbon.Tabs.Add(tab);
			}
		}

		private RibbonTab CreateTab(string tabName, List<IRibbonElement> elements)
		{
			var tab = new RibbonTab
			{
				Name = tabName,
				Id = tabName
			};
			foreach (var panelElems in elements.GroupBy(g => g.Panel))
			{
				var panel = CreatePanel(panelElems.Key, panelElems.ToList());
				tab.Panels.Add(panel);
			}
			return tab;
		}

		private RibbonPanel CreatePanel(string panelName, List<IRibbonElement> elements)
		{
			var panelSource = new RibbonPanelSource();
			panelSource.Name = panelName;
			panelSource.Id = panelName;
			panelSource.Title = panelName;
			
			foreach (var element in elements)
			{
				var button = CreateButton(element);
			}

			var panel = new RibbonPanel {Source = panelSource};
			return panel;
		}

		private RibbonButton CreateButton(IRibbonElement element)
		{
			var button = new RibbonButton();
			button.CommandParameter = element.CommandName;
			button.Text = element.Title;
			button.LargeImage = element.LargeImage;
			button.Image = element.Image;
			button.ShowImage = element.LargeImage != null || element.Image != null;
			button.ShowText = string.IsNullOrEmpty(element.Title);
			return button;
		}
	}
}
