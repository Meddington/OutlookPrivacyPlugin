using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ManagedMAPI
{
    public class StoreInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        MAPISession session_;
      
        public StoreInfo(MessageStore store)
        {
            session_ = store.Session;
            Name = store.Name;
            EntryId = store.StoreID;
        }

        public StoreInfo(MAPISession session, string name, EntryID storeId)
        {
            session_ = session;
            Name = name;
            EntryId = storeId;
        }

        public string Name { get; private set; }
        public EntryID EntryId { get; private set; }

        public bool IsDefault
        {
            get
            {
                if (session_ == null)
                    return false;
                return Equals(session_.DefaultStore);
             }
        }

        public bool IsOpened
        {
            get
            {
                if (session_ == null)
                    return false;
                return Equals(new StoreInfo(session_.CurrentStore));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is StoreInfo && session_ != null)
            {
                StoreInfo st = obj as StoreInfo;
                if (session_ != st.session_)
                    return false;
                return session_.CompareEntryIDs(EntryId, st.EntryId);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void NotifyPropertiesChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("IsDefault"));
                PropertyChanged(this, new PropertyChangedEventArgs("IsOpened"));
            }
        }
    }
}
