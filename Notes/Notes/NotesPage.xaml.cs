using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Notes.Models;

namespace Notes
{
    public partial class NotesPage : ContentPage
    {
        private string currentOrderBy;

        public NotesPage()
        {
            InitializeComponent();
            currentOrderBy = App.Database.GetSortOrder();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadNotesAsync();

            MessagingCenter.Subscribe<NoteEntryPage>(this, "NoteUpdated", async (sender) =>
            {
                await LoadNotesAsync();
            });
        }

        private async Task LoadNotesAsync(bool showDeleted = false)
        {
            List<Note> notes;

            if (showDeleted)
            {
                notes = await App.Database.GetDeletedNotesAsync();
            }
            else
            {
                notes = await App.Database.GetSortedNotesAsync(currentOrderBy);
            }
            notes = notes.Where(n => !n.IsDeleted).ToList();
            listView.ItemsSource = notes;

            emptyListMessage.IsVisible = !notes.Any();
            listView.IsVisible = notes.Any();
        }

        private void OnSearchButtonClicked(object sender, EventArgs e)
        {
            searchBar.IsVisible = !searchBar.IsVisible;
        }


        private async void OnNoteAddedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = new Note()
            });
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NoteDetailPage
                {
                    BindingContext = e.SelectedItem as Note
                });
            }
        }
        private async void OnSortButtonClicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("Sắp xếp theo", "Hủy", null, "Mới nhất (Mặc định)", "Cũ nhất", "A-Z", "Z-A");

            if (result != null && result != "Hủy")
            {
                currentOrderBy = result;
                await App.Database.SaveSortOrderAsync(currentOrderBy); 
                await LoadNotesAsync();
            }
        }
        private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchQuery = e.NewTextValue;
            listView.ItemsSource = await App.Database.GetNotesAsync(searchQuery);
        }

        private async void OnTrashButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TrashPage());
        }
    }
}
