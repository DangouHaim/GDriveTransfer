using System;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public interface ISiteSaverBehavior
    {
        void PressSavingCombination();
        void PressAddressCombination(string address);
        void PressTypeSelectionCombination();
        void PressEnter();
    }
}
