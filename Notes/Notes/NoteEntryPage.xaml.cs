using System;
using Xamarin.Forms;
using Notes.Models;

namespace Notes
{

    public partial class NoteEntryPage : ContentPage
    {
        public NoteEntryPage()
        {
            InitializeComponent();

        }


        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            if (string.IsNullOrWhiteSpace(note.Text))
            {
                await DisplayAlert("Ủa", "Nội dung đâu mà lưu bạn ơi !", "OK, quên:)");
                return;
            }

            if (string.IsNullOrWhiteSpace(note.Title))
            {
                note.Title = "Hổng có tiêu đề ^-^";
            }

            note.Date = DateTime.Now;
            await App.Database.SaveNoteAsync(note);
            await Navigation.PopToRootAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is Note note)
            {
                if (string.IsNullOrWhiteSpace(note.Title))
                {
                    note.Title = "Hổng có tiêu đề ^-^";
                }

                if (!string.IsNullOrEmpty(note.Text))
                {
                    note.Date = DateTime.Now;
                    App.Database.SaveNoteAsync(note);
                }
                
                MessagingCenter.Send(this, "NoteUpdated");
            }
            else
            {
                Navigation.PopAsync();
            }
        }
    }
}
