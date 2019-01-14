namespace AcadLib.UI.Ribbon.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows.Media.Imaging;
    using AcadLib.UI.Ribbon.Data;
    using AutoCAD_PIK_Manager.Settings;
    using Data;
    using Elements;
    using NetLib;
    using NetLib.WPF;
    using NetLib.WPF.Data;
    using ReactiveUI;

    public class RibbonVM : BaseViewModel
    {
        internal static RibbonVM ribbonVm;
        internal static string userGroup;

        public RibbonVM()
        {
            ribbonVm = this;
            UserGroup = PikSettings.UserGroup;
            BlockFiles = BlockFile.GetFiles();
            this.WhenAnyValue(v => v.UserGroup).Subscribe(s =>
            {
                userGroup = UserGroup;
                RibbonGroup = LoadRibbonGroup(UserGroup);
                Tabs = new ObservableCollection<RibbonTabDataVM>(RibbonGroup.Tabs?.Select(t=> new RibbonTabDataVM
                {
                    Name = t.Name,
                    Panels = new ObservableCollection<RibbonPanelDataVM>(t.Panels.Select(p => new RibbonPanelDataVM
                    {
                        Name = p.Name,
                        Items = new ObservableCollection<RibbonItemDataVM>(p.Items?.Select(GetItemVM) ?? new List<RibbonItemDataVM>())
                    }))
                }));
                FreeItems = new ObservableCollection<RibbonItemDataVM>(
                    RibbonGroup.FreeItems?.Select(GetItemVM) ?? new List<RibbonItemDataVM>());
            });

            Save = new RelayCommand(SaveExec);
            SelectImage = new RelayCommand(SelectImageExec);
            DeleteSelectedItem = new RelayCommand(DeleteSelectedItemExec);
        }

        public List<string> UserGroups => PikSettings.UserGroups;

        public string UserGroup { get; set; }

        public RibbonGroupData RibbonGroup { get; set; }

        public ObservableCollection<RibbonTabDataVM> Tabs { get; set; }

        public ObservableCollection<RibbonItemDataVM> FreeItems { get; set; }

        public RelayCommand Save { get; set; }

        public RibbonItemDataVM SelectedItem { get; set; }

        public RelayCommand SelectImage { get; set; }

        public RelayCommand DeleteSelectedItem { get; set; }

        public List<BlockFile> BlockFiles { get; set; }

        private static void SaveRibbonGroup(RibbonGroupData ribbonGroup, string userGroup)
        {
            var ribbonFile = RibbonGroupData.GetRibbonFile(userGroup);
            ribbonGroup?.Save(ribbonFile);
        }

        private static RibbonGroupData LoadRibbonGroup(string userGroup)
        {
            if (userGroup.IsNullOrEmpty())
                return null;
            var ribbonFile = RibbonGroupData.GetRibbonFile(userGroup);
            if (File.Exists(ribbonFile))
            {
                return RibbonGroupData.Load(ribbonFile);
            }

            var ribbonGroup = new RibbonGroupData
            {
                Tabs = new List<RibbonTabData>
                {
                    new RibbonTabData
                    {
                        Name = userGroup,
                        Panels = new List<RibbonPanelData>()
                    }
                }
            };
            return ribbonGroup;
        }

        private void SaveExec()
        {
            // Сохранить текущую группу настроек
            try
            {
                if (RibbonGroup != null)
                {
                    ClearImages();
                    RibbonGroup.Tabs = Tabs?.Select(t => new RibbonTabData
                    {
                        Name = t.Name,
                        Panels = t.Panels.Select(p => new RibbonPanelData
                        {
                            Name = p.Name,
                            Items = p.Items.Select(s => s.GetItem()).ToList()
                        }).ToList()
                    }).ToList();
                    RibbonGroup.FreeItems = FreeItems.Select(s => s.GetItem()).ToList();
                    SaveRibbonGroup(RibbonGroup, UserGroup);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearImages()
        {
            var imageDir = RibbonGroupData.GetImagesFolder(UserGroup);
            NetLib.IO.Path.DeleteDir(imageDir);
        }

        private void SelectImageExec()
        {
            try
            {
                var dlg = new OpenFileDialog { Title = "Выбор картинки", Multiselect = false };
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                var image = new BitmapImage(new Uri(dlg.FileName));
                SelectedItem.Image = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public RibbonItemDataVM GetItemVM(RibbonItemData item)
        {
            RibbonItemDataVM itemVm;
            switch (item)
            {
                case RibbonBreak ribbonBreak:
                    itemVm = new RibbonBreakVM(ribbonBreak);
                    break;
                case RibbonToggle ribbonToggle:
                    itemVm = new RibbonToggleVM(ribbonToggle);
                    break;
                case RibbonCommand ribbonCommand:
                    itemVm = new RibbonCommandVM(ribbonCommand);
                    break;
                case RibbonVisualGroupInsertBlock ribbonVisualGroupInsertBlock:
                    itemVm = new RibbonVisualGroupInsertBlockVM(ribbonVisualGroupInsertBlock, BlockFiles);
                    break;
                case RibbonVisualInsertBlock ribbonVisualInsertBlock:
                    itemVm = new RibbonVisualInsertBlockVM(ribbonVisualInsertBlock, BlockFiles);
                    break;
                case RibbonInsertBlock ribbonInsertBlock:
                    itemVm = new RibbonInsertBlockVM(ribbonInsertBlock, BlockFiles);
                    break;
                case RibbonSplit ribbonSplit:
                    itemVm = new RibbonSplitVM(ribbonSplit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(item));
            }

            itemVm.Name = item.Name;
            itemVm.Access = new ObservableCollection<AccessItem>(item.Access?.Select(s => new AccessItem
            {
                Access = s
            }) ?? new List<AccessItem>());
            itemVm.Description = item.Description;
            itemVm.IsTest = item.IsTest;
            var imageName = RibbonGroupData.GetImageName(item.Name);
            itemVm.Image = RibbonGroupData.LoadImage(userGroup, imageName);
            return itemVm;
        }

        private void DeleteSelectedItemExec()
        {
            var isFind = false;
            var item = SelectedItem;
            foreach (var tab in Tabs)
            {
                foreach (var panel in tab.Panels)
                {
                    if (panel.Items.Contains(item))
                    {
                        isFind = true;
                        panel.Items.Remove(item);
                    }
                }
            }

            if (isFind)
                FreeItems.Add(item);
            else
                FreeItems.Remove(item);
        }
    }
}
