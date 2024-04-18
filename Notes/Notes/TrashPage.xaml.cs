using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Notes.Models;

namespace Notes
{
    public partial class TrashPage : ContentPage
    {
        public TrashPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadNotesAsync();
        }

        private async Task LoadNotesAsync()
        {
            var deletedNotes = await App.Database.GetDeletedNotesAsync();
            listView.ItemsSource = deletedNotes;

            emptyTrashMessage.IsVisible = !deletedNotes.Any();
            listView.IsVisible = deletedNotes.Any();
        }

        private async void OnNoteSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var selectedNote = (Note)e.SelectedItem;

                var action = await DisplayActionSheet("Quyết định đi:", "Thôi", null, "Quay xe ↩️", "Hủy diệt 💀");

                switch (action)
                {
                    case "Quay xe ↩️":
                        await App.Database.RestoreNoteAsync(selectedNote);
                        break;
                    case "Hủy diệt 💀":
                        var confirmDelete = await DisplayAlert("Không thể quay đầu:", "Có chắc muốn hủy diệt ghi chú này không?", "Giết!", "Ziị thôi");
                        if (confirmDelete)
                        {
                            await App.Database.DeleteNotePermanentlyAsync(selectedNote);
                        }
                        break;
                }

                await LoadNotesAsync();
                listView.SelectedItem = null;
            }
        }

        private async void OnDeleteAllClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Hỏi nhẹ:", "Có chắc muốn hủy diệt tất cả hông pa?", "Yé", "Hoii hoii");

            if (confirm)
            {
                await App.Database.DeleteAllNotesPermanentlyAsync(); 
                await LoadNotesAsync();
            }
        }

        private async void OnRestoreAllClicked(object sender, EventArgs e)
        {
                await App.Database.RestoreAllNotesAsync();
                await LoadNotesAsync();

            await DisplayAlert("AMen", "Tất cả đã được hồi sinh", "Good job!");
        }
    }
}
