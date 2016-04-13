using QuotesDB.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuotesDB
{
    class QuoteService
    {
        public static readonly QuoteService Instance = new QuoteService();
        private QuoteService()
        {
        }

        private Timer popupTimer;
        private IQuoteStore dataStore;
        private readonly Properties.Settings settings = Properties.Settings.Default;

        public IQuoteStore DataStore
        {
            get
            {
                return dataStore;
            }
        }
        
        public void SetDataStore(IQuoteStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public void StartTimer()
        {
            if (dataStore == null)
                throw new Exception("The datastore is not set");

            if (dataStore.GetTotalQuotes() == 0)
                return;

            if (settings.PopupOn && settings.PopupFrequency.TotalMilliseconds != 0)
            {
                var waitTime = Convert.ToInt32(settings.PopupFrequency.TotalMilliseconds);
                popupTimer = new Timer(DoPopup, null, waitTime, waitTime );               
            }
        }

        private bool opened = false;

        private void DoPopup(object state)
        {
            if (opened)
                return;

            App.Current.Dispatcher.Invoke(() =>
            {
                QuotesPopupWindow window = new QuotesPopupWindow(dataStore);
                window.Closed += (sender, args) => opened = false;

                opened = true;
                window.Show();
            });
        }

        public void StopTimer()
        {
            if (popupTimer == null)
                return;
            
            popupTimer.Dispose();
            popupTimer = null;
        }

        public void RestartTimer()
        {
            StopTimer();
            StartTimer();
        }
    }
}
