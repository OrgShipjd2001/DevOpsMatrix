﻿using DevOpsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsDevOpsServer
{
    public class TfsChangeItemFactory : IChangeItemFactory
    {
        public IDevOpsChangeRequest GetChangeItem(string itemtype)
        {
            IDevOpsChangeRequest changeItem = new TfsWorkItem();
            changeItem.SetField<string>("System.WorkItemType", itemtype);

            return changeItem;
        }
    }
}