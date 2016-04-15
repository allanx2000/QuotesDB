using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using QuotesDB.DAO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace QuotesDB
{
    public class TagsEditorViewModel : ViewModel
    {
        private ObservableCollection<Tag> currentList = new ObservableCollection<Tag>();
        private ObservableCollection<Tag> allTags = new ObservableCollection<Tag>();
        private Window window;

        private readonly IQuoteStore ds = QuoteService.Instance.DataStore;

        private CollectionViewSource currentListView;
        public ICollectionView CurrentListView
        {
            get
            {
                return currentListView.View;
            }
        }

        public ObservableCollection<Tag> CurrentList
        {
            get
            {
                return currentList;
            }
        }

        private CollectionViewSource allTagsView;
        public ICollectionView AllTagsView
        {
            get
            {
                return allTagsView.View;
            }

        }

        public ObservableCollection<Tag> AllTags
        {
            get
            {
                return allTags;
            }
        }

        public TagsEditorViewModel(Window window, IEnumerable<Tag> existingTags)
        {
            this.window = window;
            Cancelled = true;

            LoadTags(existingTags);
        }

        private void LoadTags(IEnumerable<Tag> existingTags)
        {
            SortDescription sortId = new SortDescription("TagName", ListSortDirection.Ascending);

            currentListView = new CollectionViewSource();
            currentListView.Source = currentList;
            currentListView.SortDescriptions.Add(sortId);

            allTagsView = new CollectionViewSource();
            allTagsView.Source = AllTags;
            allTagsView.SortDescriptions.Add(sortId);


            var allTags = ds.GetTags();

            if (existingTags != null)
            {
                foreach (Tag t in existingTags)
                {
                    allTags.Remove(t);
                    this.CurrentList.Add(t);
                    //CurrentListView.Refresh();
                }
            }

            foreach (Tag t in allTags)
            {
                AllTags.Add(t);
            }
        }

        public Tag SelectedCurrent { get; set; }
        public Tag SelectedExisting { get; set; }

        private string newTagName;
        public string NewTagName
        {
            get { return newTagName; }
            set
            {
                newTagName = value;
                RaisePropertyChanged();
            }
        }

        #region Add/Remove Commands
        public ICommand AddExistingToCurrentCommand
        {
            get
            {
                return new CommandHelper(AddExistingToCurrent);
            }
        }

        private void AddExistingToCurrent()
        {
            if (SelectedExisting == null)
                return;

            var tmp = SelectedExisting;
            AllTags.Remove(tmp);
            CurrentList.Add(tmp);
        }

        public ICommand AddNewToCurrentCommand
        {
            get
            {
                return new CommandHelper(AddNewToCurrent);
            }
        }

        private void AddNewToCurrent()
        {
            try
            {
                if (string.IsNullOrEmpty(NewTagName))
                    return;

                var exists = ds.GetTags(NewTagName);
                if (exists.Count > 0)
                    throw new Exception("The tag already exists.");
                else
                {
                    CurrentList.Add(new Tag() { TagName = NewTagName });
                    NewTagName = "";
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand RemoveFromCurrentCommand
        {
            get
            {
                return new CommandHelper(RemoveFromCurrent);
            }
        }
        private void RemoveFromCurrent()
        {
            //Need to reassign as Remove will set SelectedCurrent to null

            var tmp = SelectedCurrent;
            if (tmp == null)
                return;

            bool removed = CurrentList.Remove(tmp);
            
            if (removed && tmp.ID > 0)
            {
                AllTags.Add(tmp);
            }
        }

        #endregion

        public ICommand OKCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    this.Cancelled = false;
                    window.Close();
                });
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    this.Cancelled = true;
                    window.Close();
                });
            }
        }

        public bool Cancelled { get; private set; }
    }
}
